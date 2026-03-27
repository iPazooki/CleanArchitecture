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
  /** <summary>OpenID Connect issuer URL for Keycloak.</summary> */
  KEYCLOAK_ISSUER: string;
  /** <summary>OAuth client identifier registered in Keycloak.</summary> */
  KEYCLOAK_CLIENT_ID: string;
  /** <summary>OAuth client secret for Keycloak (sensitive).</summary> */
  KEYCLOAK_CLIENT_SECRET: string;
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
  const KEYCLOAK_ISSUER = process.env["KEYCLOAK_ISSUER"];
  const KEYCLOAK_CLIENT_ID = process.env["KEYCLOAK_CLIENT_ID"];
  const KEYCLOAK_CLIENT_SECRET = process.env["KEYCLOAK_CLIENT_SECRET"];

  if (!API_BASE_URL) {
    throw new Error("Missing required environment variable: API_BASE_URL");
  }

  if (!NEXTAUTH_URL) {
    throw new Error("Missing required environment variable: NEXTAUTH_URL");
  }

  if (!NEXTAUTH_SECRET) {
    throw new Error("Missing required environment variable: NEXTAUTH_SECRET");
  }

  if (!KEYCLOAK_ISSUER) {
    throw new Error("Missing required environment variable: KEYCLOAK_ISSUER");
  }

  if (!KEYCLOAK_CLIENT_ID) {
    throw new Error("Missing required environment variable: KEYCLOAK_CLIENT_ID");
  }

  if (!KEYCLOAK_CLIENT_SECRET) {
    throw new Error("Missing required environment variable: KEYCLOAK_CLIENT_SECRET");
  }

  return {
    API_BASE_URL,
    NEXTAUTH_URL,
    NEXTAUTH_SECRET,
    KEYCLOAK_ISSUER,
    KEYCLOAK_CLIENT_ID,
    KEYCLOAK_CLIENT_SECRET
  };
}
