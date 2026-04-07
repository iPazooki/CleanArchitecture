import type { NextAuthOptions } from "next-auth";
import KeycloakProvider from "next-auth/providers/keycloak";
import { getEnvVars } from "@/config/env-vars";

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
  const { KEYCLOAK_ISSUER, KEYCLOAK_CLIENT_ID, KEYCLOAK_CLIENT_SECRET, NEXTAUTH_SECRET } = getEnvVars();
  const keycloakIssuer = KEYCLOAK_ISSUER.replace(/\/+$/, "");

  return {
    providers: [
      KeycloakProvider({
        clientId: KEYCLOAK_CLIENT_ID,
        clientSecret: KEYCLOAK_CLIENT_SECRET,
        issuer: keycloakIssuer,
        authorization: {
          params: {
            scope: "openid profile email permissions",
          },
        },
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
      async jwt({ token, account }) {
        if (account?.access_token) {
          token.accessToken = account.access_token;
          token.roles = extractRoles(account.access_token);
        }

        if (account?.id_token) {
          token.idToken = account.id_token;
        }

        return token;
      },
      async session({ session, token }) {
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