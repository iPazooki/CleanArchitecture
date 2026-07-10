import type { DefaultSession } from "next-auth";

declare module "next-auth" {
  /**
   * Anything placed here is served to the browser by `GET /api/auth/session`.
   * The backend access token must never appear on it — it lives only on the JWT,
   * and only the proxy at /api/v1/[...path] attaches it to outbound requests.
   */
  interface Session {
    error?: "RefreshAccessTokenError";
    user?: DefaultSession["user"] & {
      id?: string;
      roles?: string[];
    };
  }
}

declare module "next-auth/jwt" {
  interface JWT {
    accessToken?: string;
    refreshToken?: string;
    idToken?: string;
    /** Access-token expiry as epoch milliseconds. */
    expiresAt?: number;
    roles?: string[];
    error?: "RefreshAccessTokenError";
  }
}
