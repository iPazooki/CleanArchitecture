import type { Account, NextAuthOptions } from "next-auth";
import type { JWT } from "next-auth/jwt";
import type { Provider } from "next-auth/providers/index";
import KeycloakProvider from "next-auth/providers/keycloak";
import AzureADProvider from "next-auth/providers/azure-ad";
import { getEnvVars } from "@/config/env-vars";
import { getOAuthClientCredentials, getOAuthEndpoints } from "@/lib/auth/provider";

type JwtPayload = {
    roles?: string[];
    realm_access?: {
        roles?: string[];
    };
};

type RefreshTokenResponse = {
    access_token?: string;
    refresh_token?: string;
    expires_in?: number;
    id_token?: string;
};

/**
 * Refresh a little early so a token cannot expire while a request is in flight.
 */
const refreshSkewMs = 30_000;

function parseJwtPayload(token: string): JwtPayload | null {
    try {
        const payload = token.split(".")[1];

        if (!payload) {
            return null;
        }

        return JSON.parse(Buffer.from(payload, "base64url").toString("utf-8")) as JwtPayload;
    } catch {
        return null;
    }
}

function extractRoles(accessToken: string): string[] {
    const payload = parseJwtPayload(accessToken);
    const roles = payload?.roles ?? payload?.realm_access?.roles ?? [];

    return Array.isArray(roles) ? roles : [];
}

function buildProviders(): Provider[] {
    const env = getEnvVars();

    if (env.AUTH_PROVIDER === "entra") {
        return [
            AzureADProvider({
                clientId: env.ENTRA_CLIENT_ID,
                clientSecret: env.ENTRA_CLIENT_SECRET,
                tenantId: env.ENTRA_TENANT_ID,
                wellKnown: env.ENTRA_OPENID_CONNECT,
                authorization: {
                    params: {
                        scope: env.ENTRA_SCOPES,
                        prompt: "select_account",
                    },
                },
            }),
        ];
    }

    return [
        KeycloakProvider({
            clientId: env.KEYCLOAK_CLIENT_ID,
            clientSecret: env.KEYCLOAK_CLIENT_SECRET,
            issuer: env.KEYCLOAK_ISSUER.replace(/\/+$/, ""),
            authorization: {
                params: {
                    scope: env.KEYCLOAK_SCOPES,
                },
            },
        }),
    ];
}

/** Seed the JWT from the tokens the IdP returned on sign-in. */
function applyAccount(token: JWT, account: Account): JWT {
    const next: JWT = { ...token };

    if (account.access_token) {
        next.accessToken = account.access_token;
        next.roles = extractRoles(account.access_token);
    }

    // Entra ID typically puts App Roles on the id_token rather than the access token.
    if (account.id_token && !next.roles?.length) {
        next.roles = extractRoles(account.id_token);
    }

    if (account.id_token) {
        next.idToken = account.id_token;
    }

    if (account.refresh_token) {
        next.refreshToken = account.refresh_token;
    }

    if (typeof account.expires_at === "number") {
        next.expiresAt = account.expires_at * 1000;
    }

    return next;
}

function isAccessTokenValid(token: JWT): boolean {
    if (!token.accessToken) {
        return false;
    }

    // An IdP that omits expires_at gives us nothing to check against; trust the token.
    if (typeof token.expiresAt !== "number") {
        return true;
    }

    return Date.now() < token.expiresAt - refreshSkewMs;
}

/**
 * Never return a token we know to be expired: on any failure the caller gets an
 * `error` and no access token, which the proxy turns into a 401.
 */
async function refreshAccessToken(token: JWT): Promise<JWT> {
    if (!token.refreshToken) {
        return { ...token, accessToken: undefined, error: "RefreshAccessTokenError" };
    }

    try {
        const { clientId, clientSecret } = getOAuthClientCredentials();

        const response = await fetch(getOAuthEndpoints().tokenEndpoint, {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: new URLSearchParams({
                grant_type: "refresh_token",
                refresh_token: token.refreshToken,
                client_id: clientId,
                client_secret: clientSecret,
            }),
        });

        if (!response.ok) {
            throw new Error(`Token endpoint returned ${response.status}`);
        }

        const refreshed = (await response.json()) as RefreshTokenResponse;

        if (!refreshed.access_token) {
            throw new Error("Token endpoint returned no access_token");
        }

        return {
            ...token,
            accessToken: refreshed.access_token,
            // Providers that rotate refresh tokens invalidate the previous one.
            refreshToken: refreshed.refresh_token ?? token.refreshToken,
            idToken: refreshed.id_token ?? token.idToken,
            expiresAt: refreshed.expires_in
                ? Date.now() + refreshed.expires_in * 1000
                : undefined,
            roles: extractRoles(refreshed.access_token),
            error: undefined,
        };
    } catch {
        return { ...token, accessToken: undefined, error: "RefreshAccessTokenError" };
    }
}

export function getAuthOptions(): NextAuthOptions {
    const { NEXTAUTH_SECRET } = getEnvVars();

    return {
        providers: buildProviders(),
        pages: {
            signIn: "/signin",
        },
        secret: NEXTAUTH_SECRET,
        session: {
            strategy: "jwt",
        },
        callbacks: {
            async jwt({ token, account }) {
                if (account) {
                    return applyAccount(token, account);
                }

                if (isAccessTokenValid(token)) {
                    return token;
                }

                return refreshAccessToken(token);
            },
            async session({ session, token }) {
                if (session.user && token.sub) {
                    session.user.id = token.sub;
                    session.user.roles = Array.isArray(token.roles) ? token.roles : [];
                }

                // `token.accessToken` is deliberately NOT copied here. Everything on the
                // session is readable by the browser via GET /api/auth/session; the access
                // token stays on the encrypted JWT cookie and is attached server-side only.
                if (token.error) {
                    session.error = token.error;
                }

                return session;
            },
        },
    } satisfies NextAuthOptions;
}
