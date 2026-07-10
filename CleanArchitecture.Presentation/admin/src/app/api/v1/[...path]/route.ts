import { NextRequest, NextResponse } from "next/server";
import { getEnvVars } from "@/config/env-vars";
import { getAccessToken } from "@/lib/auth/access-token";

export const dynamic = "force-dynamic";

const apiRoutePrefix = "/api/v1";

type RouteHandlerContext = {
  params: Promise<{ path: string[] }>;
};

/**
 * Headers the browser must not be able to dictate, plus hop-by-hop headers that
 * are meaningless to the upstream connection.
 *
 * `authorization` is stripped because this proxy is the sole issuer of it — a
 * client-supplied bearer token is never forwarded. `cookie` is stripped because
 * the NextAuth session cookie is ours, not the backend's.
 */
const strippedRequestHeaders = [
  "authorization",
  "cookie",
  "connection",
  "content-length",
  "host",
  "accept-encoding",
];

const strippedResponseHeaders = ["content-encoding", "content-length", "transfer-encoding"];

function createTargetUrl(request: NextRequest, path: readonly string[]): URL {
  const baseUrl = getEnvVars().API_BASE_URL.trim().replace(/\/+$/, "");
  const prefixedBaseUrl = baseUrl.endsWith(apiRoutePrefix)
    ? baseUrl
    : `${baseUrl}${apiRoutePrefix}`;

  return new URL(`${prefixedBaseUrl}/${path.join("/")}${request.nextUrl.search}`);
}

function createProxyHeaders(requestHeaders: Headers, accessToken: string): Headers {
  const headers = new Headers(requestHeaders);

  strippedRequestHeaders.forEach((header) => headers.delete(header));
  headers.set("Authorization", `Bearer ${accessToken}`);

  return headers;
}

async function proxyRequest(request: NextRequest, context: RouteHandlerContext) {
  const accessToken = await getAccessToken(request);

  if (!accessToken) {
    return NextResponse.json(
      { title: "Unauthorized", status: 401, detail: "No active session." },
      { status: 401 },
    );
  }

  const { path } = await context.params;
  const body =
    request.method === "GET" || request.method === "HEAD"
      ? undefined
      : await request.arrayBuffer();

  const response = await fetch(createTargetUrl(request, path), {
    method: request.method,
    headers: createProxyHeaders(request.headers, accessToken),
    body,
    cache: "no-store",
  });

  const responseHeaders = new Headers(response.headers);
  strippedResponseHeaders.forEach((header) => responseHeaders.delete(header));

  return new NextResponse(response.body, {
    status: response.status,
    headers: responseHeaders,
  });
}

export function GET(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function POST(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function PUT(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function PATCH(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function DELETE(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function HEAD(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}

export function OPTIONS(request: NextRequest, context: RouteHandlerContext) {
  return proxyRequest(request, context);
}
