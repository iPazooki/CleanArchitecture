import { withAuth } from "next-auth/middleware";

export default withAuth({
  callbacks: {
    authorized: ({ token }) => Boolean(token),
  },
});

// Page routes only. Route handlers under /api gate themselves: NextAuth owns
// /api/auth, and /api/v1 answers 401 when there is no access token. Redirecting
// a fetch() to the HTML sign-in page would reach the caller as a 200 full of markup.
export const config = {
  matcher: [
    "/((?!api/|_next/static|_next/image|signin|signup|.*\\..*$).*)",
  ],
};
