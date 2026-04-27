import type {NextAuthOptions} from "next-auth";
import KeycloakProvider from "next-auth/providers/keycloak";
import AzureADProvider from "next-auth/providers/azure-ad";
import {getEnvVars} from "@/config/env-vars";

type JwtPayload = {
    roles?: string[];
    realm_access?: {
        roles?: string[];
    };
};

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

export function getAuthOptions(): NextAuthOptions {
    const {
        KEYCLOAK_ISSUER,
        KEYCLOAK_CLIENT_ID,
        KEYCLOAK_CLIENT_SECRET,
        NEXTAUTH_SECRET,
        KEYCLOAK_SCOPES,
        ENTRA_CLIENT_ID,
        ENTRA_CLIENT_SECRET,
        ENTRA_TENANT_ID,
        ENTRA_SCOPES,
        ENTRA_OPENID_CONNECT,
    } = getEnvVars();

    return {
        providers: [
            KeycloakProvider({
                clientId: KEYCLOAK_CLIENT_ID!,
                clientSecret: KEYCLOAK_CLIENT_SECRET!,
                issuer: KEYCLOAK_ISSUER!.replace(/\/+$/, ""),
                authorization: {
                    params: {
                        scope: KEYCLOAK_SCOPES,
                    },
                },
            }),
            AzureADProvider({
                clientId: ENTRA_CLIENT_ID!,
                clientSecret: ENTRA_CLIENT_SECRET!,
                tenantId: ENTRA_TENANT_ID,
                wellKnown: ENTRA_OPENID_CONNECT,
                authorization: {
                    params: {
                        scope: ENTRA_SCOPES,
                        prompt: "select_account",
                    },
                }
            }),
        ],
        pages: {
            signIn: "/signin",
        },
        secret: NEXTAUTH_SECRET,
        session: {
            strategy: "jwt",
        },
        callbacks: {
            async jwt({token, account}) {
                if (account?.access_token) {
                    token.accessToken = account.access_token;
                    token.roles = extractRoles(account.access_token);
                }

                // Fallback: Entra ID often puts App Roles in the id_token for the BFF
                if (account?.id_token && (!token.roles || token.roles.length === 0)) {
                    token.roles = extractRoles(account.id_token);
                }

                if (account?.id_token) {
                    token.idToken = account.id_token;
                }

                return token;
            },
            async session({session, token}) {
                if (session.user && token.sub) {
                    session.user.id = token.sub;
                    session.user.roles = Array.isArray(token.roles) ? token.roles : [];
                }

                if (typeof token.accessToken === "string") {
                    session.accessToken = token.accessToken;
                }

                return session;
            },
        },
    } satisfies NextAuthOptions;
}
