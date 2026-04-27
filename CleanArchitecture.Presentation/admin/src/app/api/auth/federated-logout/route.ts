import { NextRequest, NextResponse } from "next/server";
import { getToken } from "next-auth/jwt";
import { getEnvVars } from "@/config/env-vars";

function getBaseUrl(request: NextRequest, nextAuthUrl: string): string {
  const normalizedNextAuthUrl = nextAuthUrl.trim().replace(/\/+$/, "");
  return normalizedNextAuthUrl || request.nextUrl.origin;
}

function createKeycloakLogoutUrl(issuer: string): URL {
  const normalizedIssuer = issuer.trim().replace(/\/+$/, "");
  return new URL(`${normalizedIssuer}/protocol/openid-connect/logout`);
}

function createEntraLogoutUrl(tenantId: string): URL {
  return new URL(`https://login.microsoftonline.com/${tenantId}/oauth2/v2.0/logout`);
}

export async function GET(request: NextRequest) {
  const { NEXTAUTH_URL, KEYCLOAK_ISSUER, ENTRA_TENANT_ID, AUTH_PROVIDER, NEXTAUTH_SECRET } = getEnvVars();
  const signInUrl = new URL("/signin", getBaseUrl(request, NEXTAUTH_URL));

  const token = await getToken({
    req: request,
    secret: NEXTAUTH_SECRET,
  });

  const provider = AUTH_PROVIDER?.toLowerCase();

  if (provider === "entra" || provider === "azure-ad") {
    if (!ENTRA_TENANT_ID) {
      return NextResponse.json({ logoutUrl: signInUrl.toString() });
    }

    const logoutUrl = createEntraLogoutUrl(ENTRA_TENANT_ID);
    logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString());

    if (typeof token?.idToken === "string" && token.idToken.length > 0) {
      logoutUrl.searchParams.set("id_token_hint", token.idToken);
    }

    return NextResponse.json({ logoutUrl: logoutUrl.toString() });
  }

  // Default to Keycloak
  const issuer = KEYCLOAK_ISSUER?.trim();
  if (!issuer) {
    return NextResponse.json({ logoutUrl: signInUrl.toString() });
  }

  const logoutUrl = createKeycloakLogoutUrl(issuer);
  logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString());

  if (typeof token?.idToken === "string" && token.idToken.length > 0) {
    logoutUrl.searchParams.set("id_token_hint", token.idToken);
  }

  return NextResponse.json({ logoutUrl: logoutUrl.toString() });
}
