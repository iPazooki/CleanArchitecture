import { NextRequest, NextResponse } from "next/server";
import { getToken } from "next-auth/jwt";
import { getEnvVars } from "@/config/env-vars";
import { getOAuthEndpoints } from "@/lib/auth/provider";

function getBaseUrl(request: NextRequest, nextAuthUrl: string): string {
  return nextAuthUrl.trim().replace(/\/+$/, "") || request.nextUrl.origin;
}

export async function GET(request: NextRequest) {
  const { NEXTAUTH_URL, NEXTAUTH_SECRET } = getEnvVars();
  const signInUrl = new URL("/signin", getBaseUrl(request, NEXTAUTH_URL));

  const token = await getToken({ req: request, secret: NEXTAUTH_SECRET });

  const logoutUrl = new URL(getOAuthEndpoints().endSessionEndpoint);
  logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString());

  if (typeof token?.idToken === "string" && token.idToken.length > 0) {
    logoutUrl.searchParams.set("id_token_hint", token.idToken);
  }

  return NextResponse.json({ logoutUrl: logoutUrl.toString() });
}
