import { getServerSession } from "next-auth";
import { getSession, signIn } from "next-auth/react";
import { authOptions } from "@/app/api/auth/[...nextauth]/route";

/**
 * Typed error for non-2xx API responses.
 * Preserves status, parsed body, and headers so callers (including
 * React Query `onError`) can inspect validation / ProblemDetails payloads.
 */
export class ApiError extends Error {
  status: number;
  data: unknown;
  headers: Headers;

  constructor(status: number, data: unknown, headers: Headers) {
    super(`API request failed with status ${status}`);
    this.name = "ApiError";
    this.status = status;
    this.data = data;
    this.headers = headers;
  }
}

/**
 * Custom fetch mutator for Orval.
 * It uses the appropriate session retrieval method based on the environment.
 * It automatically attaches the Authorization: Bearer <token> header to all outgoing requests.
 */
export const orvalFetch = async <T>(
    url: string,
    options: RequestInit
): Promise<T> => {
    let accessToken: string | undefined;
    // Check if we are on the server or client
    if (typeof window === "undefined") {
        // Server side: getServerSession with authOptions
        const session = await getServerSession(authOptions);
        accessToken = (session as any)?.accessToken;
    } else {
        // Client side: getSession from next-auth/react
        const session = await getSession();
        accessToken = (session as any)?.accessToken;
    }
    const headers = new Headers(options.headers);
    if (accessToken) {
        headers.set("Authorization", `Bearer ${accessToken}`);
    }
    // Ensure Base URL is used if provided in environment (server-side only)
    const baseUrl = process.env.API_BASE_URL || "";
    const cleanBaseUrl = baseUrl.endsWith("/") ? baseUrl.slice(0, -1) : baseUrl;
    const cleanUrl = url.startsWith("/") ? url : `/${url}`;
    // BFF/Proxy logic:
    // When on the client, use '/api/v1' as base to trigger Next.js rewrites.
    // When on the server, use the direct backend URL.
    let fullUrl: string;
    if (typeof window !== "undefined") {
        fullUrl = `/api/v1${cleanUrl}`;
    } else {
        // Backend API doesn't use the /api/v1 prefix
        fullUrl = url.startsWith("http") ? url : `${cleanBaseUrl}${cleanUrl}`;
    }
    // console.log(`[DEBUG_LOG] Fetching URL: ${fullUrl}`);
    const response = await fetch(fullUrl, {
        ...options,
        headers,
    });
    const data = await response.json().catch(() => ({}));
    if (!response.ok) {
        // Handle 401 Unauthorized - session expired or invalid token
        if (response.status === 401 && typeof window !== "undefined") {
            // Redirect to login page on client-side
            signIn("keycloak");
            // Throw error anyway for proper error handling in UI
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
