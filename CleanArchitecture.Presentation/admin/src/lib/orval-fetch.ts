import { ApiError } from "@/lib/utils/api-error";

const requestTimeoutMs = 10_000;

function resolveRequestUrl(url: string): string {
  if (/^https?:\/\//i.test(url)) {
    return url;
  }

  return url.startsWith("/") ? url : `/${url}`;
}

function resolveLocale(): string | null {
  return localStorage.getItem("locale") || document.documentElement.lang || null;
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
  return contentType.includes("application/json") ? JSON.parse(body) : body;
}

/**
 * The mutator every orval-generated client calls. It runs in the browser and
 * targets this app's own /api/v1 proxy — which is what attaches the backend
 * access token. Nothing here reads or forwards credentials, and nothing here
 * navigates: a 401 surfaces as an ApiError and QueryProvider decides what to do.
 */
export const orvalFetch = async <T>(url: string, options: RequestInit = {}): Promise<T> => {
  if (typeof window === "undefined") {
    throw new Error(
      "orvalFetch runs in the browser only. Server Components must call the .NET API " +
        "directly with an access token from lib/auth/access-token, not through the proxy.",
    );
  }

  const headers = new Headers(options.headers);
  const locale = resolveLocale();

  if (locale) {
    headers.set("Accept-Language", locale);
  }

  const response = await fetch(resolveRequestUrl(url), {
    ...options,
    headers,
    signal: options.signal ?? AbortSignal.timeout(requestTimeoutMs),
  });

  const data = await parseResponseBody(response);

  if (!response.ok) {
    throw new ApiError(response.status, data, response.headers);
  }

  return {
    data,
    status: response.status,
    headers: response.headers,
  } as T;
};

export default orvalFetch;
