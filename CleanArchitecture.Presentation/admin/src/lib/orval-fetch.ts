import { getServerSession } from "next-auth";
import { getSession, signIn } from "next-auth/react";
import { getAuthOptions } from "@/lib/auth/auth-options";
import { ApiError } from "@/lib/utils/api-error";
import { getEnvVars } from "@/config/env-vars";

const apiRoutePrefix = "/api/v1";

function isAbsoluteUrl(url: string): boolean {
  return /^https?:\/\//i.test(url);
}

function normalizeRequestPath(url: string): string {
  return url.startsWith("/") ? url : `/${url}`;
}

function buildServerApiUrl(path: string): string {
  const apiBaseUrl = getEnvVars().API_BASE_URL.trim().replace(/\/+$/, "");

  if (!apiBaseUrl) {
    throw new Error("API_BASE_URL must be configured for server-side API requests.");
  }

  const normalizedPath = normalizeRequestPath(path);

  if (apiBaseUrl.endsWith(apiRoutePrefix) && normalizedPath === apiRoutePrefix) {
    return apiBaseUrl;
  }

  if (apiBaseUrl.endsWith(apiRoutePrefix) && normalizedPath.startsWith(`${apiRoutePrefix}/`)) {
    return `${apiBaseUrl}${normalizedPath.slice(apiRoutePrefix.length)}`;
  }

  return `${apiBaseUrl}${normalizedPath}`;
}

async function resolveAccessToken(): Promise<string | undefined> {
  if (typeof window === "undefined") {
    const session = await getServerSession(getAuthOptions());
    return session?.accessToken;
  }

  const session = await getSession();
  return session?.accessToken;
}

function resolveRequestUrl(url: string): string {
  if (isAbsoluteUrl(url)) {
    return url;
  }

  const normalizedPath = normalizeRequestPath(url);

  if (typeof window !== "undefined") {
    return normalizedPath;
  }

  return buildServerApiUrl(normalizedPath);
}

async function parseResponseBody(response: Response): Promise<unknown> {
  if (response.status === 204 || response.status === 205) {
    return undefined;
  }

  const body = await response.text();
  if (!body) {
    return undefined;
  }

  const contentType = response.headers.get("content-type") ?? "";
  if (contentType.includes("application/json")) {
    return JSON.parse(body);
  }

  return body;
}

export const orvalFetch = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  const accessToken = await resolveAccessToken();
  const headers = new Headers(options.headers);

  if (accessToken) {
    headers.set("Authorization", `Bearer ${accessToken}`);
  }

  const response = await fetch(resolveRequestUrl(url), {
    ...options,
    headers,
  });

  const data = await parseResponseBody(response);

  if (!response.ok) {
    if (response.status === 401 && typeof window !== "undefined") {
      const callbackUrl = `${window.location.pathname}${window.location.search}`;
      void signIn("keycloak", { callbackUrl });
    }

    throw new ApiError(response.status, data, response.headers);
  }

  return {
    data,
    status: response.status,
    headers: response.headers,
  } as T;
};

export default orvalFetch;
