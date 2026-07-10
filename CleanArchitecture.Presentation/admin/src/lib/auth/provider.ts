import "server-only";

import { getEnvVars } from "@/config/env-vars";

/** The provider as this app names it. Mirrors the `AUTH_PROVIDER` env var. */
export type AuthProviderId = "keycloak" | "entra";

/** The id NextAuth registers the provider under, which differs for Entra. */
export type NextAuthProviderId = "keycloak" | "azure-ad";

export interface OAuthEndpoints {
  tokenEndpoint: string;
  endSessionEndpoint: string;
}

export interface OAuthClientCredentials {
  clientId: string;
  clientSecret: string;
}

const entraDefaultDomain = "login.microsoftonline.com";

function stripTrailingSlashes(value: string): string {
  return value.trim().replace(/\/+$/, "");
}

/**
 * Entra tenants may sit on a CIAM domain rather than login.microsoftonline.com;
 * the discovery document URL is the only place that domain appears.
 */
function resolveEntraDomain(openIdConnectUrl: string | undefined): string {
  if (!openIdConnectUrl) {
    return entraDefaultDomain;
  }

  return new URL(openIdConnectUrl).hostname;
}

export function getAuthProviderId(): AuthProviderId {
  return getEnvVars().AUTH_PROVIDER;
}

export function getNextAuthProviderId(): NextAuthProviderId {
  return getAuthProviderId() === "entra" ? "azure-ad" : "keycloak";
}

export function getOAuthEndpoints(): OAuthEndpoints {
  const env = getEnvVars();

  if (env.AUTH_PROVIDER === "entra") {
    const domain = resolveEntraDomain(env.ENTRA_OPENID_CONNECT);
    const base = `https://${domain}/${env.ENTRA_TENANT_ID}/oauth2/v2.0`;

    return {
      tokenEndpoint: `${base}/token`,
      endSessionEndpoint: `${base}/logout`,
    };
  }

  const base = `${stripTrailingSlashes(env.KEYCLOAK_ISSUER)}/protocol/openid-connect`;

  return {
    tokenEndpoint: `${base}/token`,
    endSessionEndpoint: `${base}/logout`,
  };
}

export function getOAuthClientCredentials(): OAuthClientCredentials {
  const env = getEnvVars();

  if (env.AUTH_PROVIDER === "entra") {
    return {
      clientId: env.ENTRA_CLIENT_ID,
      clientSecret: env.ENTRA_CLIENT_SECRET,
    };
  }

  return {
    clientId: env.KEYCLOAK_CLIENT_ID,
    clientSecret: env.KEYCLOAK_CLIENT_SECRET,
  };
}
