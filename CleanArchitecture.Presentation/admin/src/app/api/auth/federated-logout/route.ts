import { NextRequest, NextResponse } from "next/server"
import { getToken } from "next-auth/jwt"

function getBaseUrl(request: NextRequest) {
    return process.env.NEXTAUTH_URL ?? request.nextUrl.origin
}

export async function GET(request: NextRequest) {
    const baseUrl = getBaseUrl(request)
    const signInUrl = new URL("/signin", baseUrl)
    const issuer = process.env.KEYCLOAK_ISSUER

    if (!issuer) {
        return NextResponse.json({ logoutUrl: signInUrl.toString() })
    }

    const token = await getToken({
        req: request,
        secret: process.env.NEXTAUTH_SECRET,
    })

    const logoutUrl = new URL(`${issuer}/protocol/openid-connect/logout`)
    logoutUrl.searchParams.set("post_logout_redirect_uri", signInUrl.toString())

    if (typeof token?.idToken === "string" && token.idToken.length > 0) {
        logoutUrl.searchParams.set("id_token_hint", token.idToken)
    }

    return NextResponse.json({ logoutUrl: logoutUrl.toString() })
}
