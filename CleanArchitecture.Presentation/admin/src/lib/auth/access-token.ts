import "server-only";

import { cookies, headers } from "next/headers";
import { getToken } from "next-auth/jwt";
import type { GetTokenParams } from "next-auth/jwt";
import type { NextRequest } from "next/server";
import { getEnvVars } from "@/config/env-vars";

/**
 * The one place the backend access token is read. Everything that needs to call
 * the .NET API server-side goes through here; nothing hands it to the browser.
 *
 * Route handlers pass their `NextRequest`. Server Components have no request
 * object, so we hand `getToken` the ambient cookies/headers instead — its
 * `SessionStore` accepts anything exposing `getAll()`, which `cookies()` does.
 */
export async function getAccessToken(request?: NextRequest): Promise<string | undefined> {
  const req = request ?? ({
    cookies: await cookies(),
    headers: await headers(),
  } as unknown as GetTokenParams["req"]);

  const token = await getToken({
    req,
    secret: getEnvVars().NEXTAUTH_SECRET,
  });

  // A failed refresh clears the access token and sets `error`; treat both as no token.
  if (!token || token.error) {
    return undefined;
  }

  return token.accessToken;
}
