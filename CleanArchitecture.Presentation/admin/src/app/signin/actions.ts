"use server";

import { getEnvVars } from "@/config/env-vars";

export async function getAuthProviderAction(): Promise<string> {
  const { AUTH_PROVIDER } = getEnvVars();
  return AUTH_PROVIDER;
}
