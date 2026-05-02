export interface EnvVars {
  API_BASE_URL: string;
  NEXTAUTH_URL: string;
  NEXTAUTH_SECRET: string;
  AUTH_PROVIDER: string;
  KEYCLOAK_ISSUER?: string;
  KEYCLOAK_CLIENT_ID?: string;
  KEYCLOAK_CLIENT_SECRET?: string;
  KEYCLOAK_SCOPES?: string;
  ENTRA_CLIENT_ID?: string;
  ENTRA_CLIENT_SECRET?: string;
  ENTRA_TENANT_ID?: string;
  ENTRA_SCOPES?: string;
  ENTRA_OPENID_CONNECT?: string;
}

// Validates required env vars at call time so misconfiguration fails fast.
export function getEnvVars(): EnvVars {
  const API_BASE_URL = process.env["API_BASE_URL"];
  const NEXTAUTH_URL = process.env["NEXTAUTH_URL"];
  const NEXTAUTH_SECRET = process.env["NEXTAUTH_SECRET"];
  const AUTH_PROVIDER = process.env["AUTH_PROVIDER"];
  const KEYCLOAK_ISSUER = process.env["KEYCLOAK_ISSUER"];
  const KEYCLOAK_CLIENT_ID = process.env["KEYCLOAK_CLIENT_ID"];
  const KEYCLOAK_CLIENT_SECRET = process.env["KEYCLOAK_CLIENT_SECRET"];
  const KEYCLOAK_SCOPES = process.env["KEYCLOAK_SCOPES"];
  const ENTRA_CLIENT_ID = process.env["ENTRA_CLIENT_ID"];
  const ENTRA_CLIENT_SECRET = process.env["ENTRA_CLIENT_SECRET"];
  const ENTRA_TENANT_ID = process.env["ENTRA_TENANT_ID"];
  const ENTRA_SCOPES = process.env["ENTRA_SCOPES"];
  const ENTRA_OPENID_CONNECT = process.env["ENTRA_OPENID_CONNECT"];

  if (!API_BASE_URL) {
    throw new Error("Missing required environment variable: API_BASE_URL");
  }

  if (!NEXTAUTH_URL) {
    throw new Error("Missing required environment variable: NEXTAUTH_URL");
  }

  if (!NEXTAUTH_SECRET) {
    throw new Error("Missing required environment variable: NEXTAUTH_SECRET");
  }

  if (!AUTH_PROVIDER) {
    throw new Error("Missing required environment variable: AUTH_PROVIDER");
  }

  return {
    API_BASE_URL,
    NEXTAUTH_URL,
    NEXTAUTH_SECRET,
    AUTH_PROVIDER,
    KEYCLOAK_ISSUER,
    KEYCLOAK_CLIENT_ID,
    KEYCLOAK_CLIENT_SECRET,
    KEYCLOAK_SCOPES,
    ENTRA_CLIENT_ID,
    ENTRA_CLIENT_SECRET,
    ENTRA_TENANT_ID,
    ENTRA_SCOPES,
    ENTRA_OPENID_CONNECT,
  };
}
