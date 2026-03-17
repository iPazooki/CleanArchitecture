import { NextRequest, NextResponse } from "next/server";
import { getToken } from "next-auth/jwt";

function getBaseUrl(request: NextRequest): string {
  return process.env.NEXTAUTH_URL?.trim().replace(/\/+$/, "") ?? request.nextUrl.origin;
}

function createLogoutUrl(issuer: string): URL {
  const normalizedIssuer = issuer.trim().replace(/\/+$/, "");
  return new URL(`${normalizedIssuer}/protocol/openid-connect/logout`);
}

export async function GET(request: NextRequest) {
  const signInUrl = new URL("/signin", getBaseUrl(request));
  const issuer = process.env.KEYCLOAK_ISSUER?.trim();

  if (!issuer) {
    return NextResponse.json({ logoutUrl: signInUrl.toString() });
  }

  const token = await getToken({
    req: request,
    secret: process.env.NEXTAUTH_SECRET,
  });

  const logoutUrl = createLogoutUrl(issuer);
  logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString());

  if (typeof token?.idToken === "string" && token.idToken.length > 0) {
    logoutUrl.searchParams.set("id_token_hint", token.idToken);
  }

  return NextResponse.json({ logoutUrl: logoutUrl.toString() });
}
