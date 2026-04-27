/**
 * <summary>
 * Provides access to environment variables used in this client app.
 * This module centralizes all environment variable lookups for type safety and
 * easier refactoring.
 * </summary>
 *
 * <remarks>
 * Reads from process.env at runtime and validates that required variables are
 * present. This module is intended for server-side execution (e.g. Next.js
 * server runtime) where process.env is available. Consumers should call
 * getEnvVars() early in application startup to fail-fast when configuration
 * is incomplete.
 * </remarks>
 */

/**
 * <summary>
 * Represents the environment variables required by the application.
 * </summary>
 */
export interface EnvVars {
    /** <summary>Base URL for the API the client talks to.</summary> */
    API_BASE_URL: string;
    /** <summary>Application URL used by NextAuth for callbacks.</summary> */
    NEXTAUTH_URL: string;
    /** <summary>Secret used by NextAuth to sign session tokens.</summary> */
    NEXTAUTH_SECRET: string;
    /** <summary>Authentication provider to use.</summary> */
    AUTH_PROVIDER: string;
    /** <summary>OpenID Connect issuer URL for Keycloak.</summary> */
    KEYCLOAK_ISSUER?: string;
    /** <summary>OAuth client identifier registered in Keycloak.</summary> */
    KEYCLOAK_CLIENT_ID?: string;
    /** <summary>OAuth client secret for Keycloak (sensitive).</summary> */
    KEYCLOAK_CLIENT_SECRET?: string;
    /** <summary>OAuth scopes requested from Keycloak.</summary> */
    KEYCLOAK_SCOPES?: string;
    /** <summary>OAuth client identifier registered in Entra ID.</summary> */
    ENTRA_CLIENT_ID?: string;
    /** <summary>OAuth client secret for Entra ID (sensitive).</summary> */
    ENTRA_CLIENT_SECRET?: string;
    /** <summary>Tenant ID for Entra ID.</summary> */
    ENTRA_TENANT_ID?: string;
    /** <summary>OAuth scopes requested from Entra ID.</summary> */
    ENTRA_SCOPES?: string;
    /** <summary>OpenID Connect issuer URL for Entra ID. (OpenID Connect metadata document)</summary> */
    ENTRA_OPENID_CONNECT?: string;
}

/**
 * <summary>
 * Reads and validates required environment variables from process.env and
 * returns them as a strongly-typed object.
 * </summary>
 *
 * <returns>
 * An <see cref="EnvVars"/> object containing validated environment values.
 * </returns>
 *
 * <exception cref="Error">
 * Thrown when any required environment variable is missing. Error messages
 * include the name of the missing variable to make configuration issues easy
 * to diagnose.
 * </exception>
 */
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
