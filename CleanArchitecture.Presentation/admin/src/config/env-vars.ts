import "server-only";

import { z } from "zod";

const baseEnvShape = {
  API_BASE_URL: z.string().min(1),
  NEXTAUTH_URL: z.string().min(1),
  NEXTAUTH_SECRET: z.string().min(1),
};

const keycloakEnvSchema = z.object({
  ...baseEnvShape,
  AUTH_PROVIDER: z.literal("keycloak"),
  KEYCLOAK_ISSUER: z.string().min(1),
  KEYCLOAK_CLIENT_ID: z.string().min(1),
  KEYCLOAK_CLIENT_SECRET: z.string().min(1),
  KEYCLOAK_SCOPES: z.string().min(1).default("openid profile email"),
});

const entraEnvSchema = z.object({
  ...baseEnvShape,
  AUTH_PROVIDER: z.literal("entra"),
  ENTRA_CLIENT_ID: z.string().min(1),
  ENTRA_CLIENT_SECRET: z.string().min(1),
  ENTRA_TENANT_ID: z.string().min(1),
  ENTRA_SCOPES: z.string().min(1).default("openid profile email"),
  ENTRA_OPENID_CONNECT: z.url().optional(),
});

const envSchema = z.discriminatedUnion("AUTH_PROVIDER", [keycloakEnvSchema, entraEnvSchema]);

export type EnvVars = z.infer<typeof envSchema>;
export type KeycloakEnvVars = z.infer<typeof keycloakEnvSchema>;
export type EntraEnvVars = z.infer<typeof entraEnvSchema>;

function readRawEnv(): Record<string, string | undefined> {
  return {
    AUTH_PROVIDER: process.env["AUTH_PROVIDER"]?.trim().toLowerCase(),
    API_BASE_URL: process.env["API_BASE_URL"],
    NEXTAUTH_URL: process.env["NEXTAUTH_URL"],
    NEXTAUTH_SECRET: process.env["NEXTAUTH_SECRET"],
    KEYCLOAK_ISSUER: process.env["KEYCLOAK_ISSUER"],
    KEYCLOAK_CLIENT_ID: process.env["KEYCLOAK_CLIENT_ID"],
    KEYCLOAK_CLIENT_SECRET: process.env["KEYCLOAK_CLIENT_SECRET"],
    KEYCLOAK_SCOPES: process.env["KEYCLOAK_SCOPES"],
    ENTRA_CLIENT_ID: process.env["ENTRA_CLIENT_ID"],
    ENTRA_CLIENT_SECRET: process.env["ENTRA_CLIENT_SECRET"],
    ENTRA_TENANT_ID: process.env["ENTRA_TENANT_ID"],
    ENTRA_SCOPES: process.env["ENTRA_SCOPES"],
    ENTRA_OPENID_CONNECT: process.env["ENTRA_OPENID_CONNECT"],
  };
}

function describeFailure(error: z.ZodError): string {
  const issues = error.issues
    .map((issue) => `  ${issue.path.join(".") || "AUTH_PROVIDER"}: ${issue.message}`)
    .join("\n");

  return `Invalid environment configuration:\n${issues}`;
}

let cachedEnvVars: EnvVars | undefined;

/**
 * Selecting a provider makes that provider's variables required, so call sites
 * never have to re-check them.
 */
export function getEnvVars(): EnvVars {
  if (cachedEnvVars) {
    return cachedEnvVars;
  }

  const result = envSchema.safeParse(readRawEnv());

  if (!result.success) {
    throw new Error(describeFailure(result.error));
  }

  cachedEnvVars = result.data;
  return cachedEnvVars;
}
