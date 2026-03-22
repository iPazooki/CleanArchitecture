import { NextRequest, NextResponse } from "next/server";
import { getToken } from "next-auth/jwt";
import { getEnvVars } from "@/config/env-vars";

function getBaseUrl(request: NextRequest, nextAuthUrl: string): string {
  const normalizedNextAuthUrl = nextAuthUrl.trim().replace(/\/+$/, "");
  return normalizedNextAuthUrl || request.nextUrl.origin;
}

function createLogoutUrl(issuer: string): URL {
  const normalizedIssuer = issuer.trim().replace(/\/+$/, "");
  return new URL(`${normalizedIssuer}/protocol/openid-connect/logout`);
}

export async function GET(request: NextRequest) {
  const { NEXTAUTH_URL, KEYCLOAK_ISSUER, NEXTAUTH_SECRET } = getEnvVars();
  const signInUrl = new URL("/signin", getBaseUrl(request, NEXTAUTH_URL));
  const issuer = KEYCLOAK_ISSUER.trim();

  if (!issuer) {
    return NextResponse.json({ logoutUrl: signInUrl.toString() });
  }

  const token = await getToken({
    req: request,
    secret: NEXTAUTH_SECRET,
  });

  const logoutUrl = createLogoutUrl(issuer);
  logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString());

  if (typeof token?.idToken === "string" && token.idToken.length > 0) {
    logoutUrl.searchParams.set("id_token_hint", token.idToken);
  }

  return NextResponse.json({ logoutUrl: logoutUrl.toString() });
}
