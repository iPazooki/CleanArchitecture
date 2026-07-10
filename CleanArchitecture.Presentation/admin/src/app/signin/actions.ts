"use server";

import { getNextAuthProviderId, type NextAuthProviderId } from "@/lib/auth/provider";

export async function getAuthProviderAction(): Promise<NextAuthProviderId> {
  return getNextAuthProviderId();
}
