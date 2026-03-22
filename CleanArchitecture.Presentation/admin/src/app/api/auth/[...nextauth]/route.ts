import type { NextRequest } from "next/server";
import NextAuth, { type NextAuthOptions } from "next-auth";
import KeycloakProvider from "next-auth/providers/keycloak";
import { getEnvVars } from "@/config/env-vars";

type RouteHandlerContext = {
  params: Promise<{ nextauth: string[] }>;
};

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
        }

        if (account?.id_token) {
          token.idToken = account.id_token;
        }

        return token;
      },
      async session({ session, token }) {
        if (session.user && token.sub) {
          session.user.id = token.sub;
        }

        if (typeof token.accessToken === "string") {
          session.accessToken = token.accessToken;
        }

        return session;
      },
    },
  } satisfies NextAuthOptions;
}

async function handleAuth(request: NextRequest, context: RouteHandlerContext) {
  return NextAuth(request, context, getAuthOptions());
}

export function GET(request: NextRequest, context: RouteHandlerContext) {
  return handleAuth(request, context);
}

export function POST(request: NextRequest, context: RouteHandlerContext) {
  return handleAuth(request, context);
}
