import { NextRequest, NextResponse } from "next/server";
import { getEnvVars } from "@/config/env-vars";

export const dynamic = "force-dynamic";

const apiRoutePrefix = "/api/v1";
const methods = ["GET", "POST", "PUT", "PATCH", "DELETE", "HEAD", "OPTIONS"] as const;

function normalizeRequestPath(path: readonly string[]): string {
  return path.join("/");
}

function createTargetUrl(request: NextRequest, path: readonly string[]): URL {
  const apiBaseUrl = getEnvVars().API_BASE_URL.trim().replace(/\/+$/, "");

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URL must be configured for API proxy requests.");
  }

  const normalizedBaseUrl = apiBaseUrl.endsWith(apiRoutePrefix)
    ? apiBaseUrl
    : `${apiBaseUrl}${apiRoutePrefix}`;

  return new URL(`${normalizedBaseUrl}/${normalizeRequestPath(path)}${request.nextUrl.search}`);
}

function createProxyHeaders(requestHeaders: Headers): Headers {
  const headers = new Headers(requestHeaders);

  headers.delete("connection");
  headers.delete("content-length");
  headers.delete("host");

  return headers;
}

async function proxyRequest(request: NextRequest, context: { params: Promise<{ path: string[] }> }) {
  const { path } = await context.params;
  const targetUrl = createTargetUrl(request, path);
  const body = request.method === "GET" || request.method === "HEAD"
    ? undefined
    : await request.arrayBuffer();

  const response = await fetch(targetUrl, {
    method: request.method,
    headers: createProxyHeaders(request.headers),
    body,
    cache: "no-store",
  });

  return new NextResponse(response.body, {
    status: response.status,
    headers: response.headers,
  });
}

export const GET = proxyRequest;
export const POST = proxyRequest;
export const PUT = proxyRequest;
export const PATCH = proxyRequest;
export const DELETE = proxyRequest;
export const HEAD = proxyRequest;
export const OPTIONS = proxyRequest;
