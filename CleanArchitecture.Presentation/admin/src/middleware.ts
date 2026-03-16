import { withAuth } from "next-auth/middleware"

export default withAuth({
  callbacks: {
    authorized: ({ token }) => {
      // If there's no session token, user is not authenticated -> redirect to sign-in
      console.log("[DEBUG_LOG] authorized check. Token:", token ? "exists" : "missing");
      return !!token
    },
  },
})

export const config = {
  matcher: [
    /*
     * Match all routes except:
     * - api (including next-auth)
     * - _next (static files)
     * - Next.js internals
     * - public/static asset files (anything with a file extension)
     * - signin, signup
     */
    "/((?!api|_next/static|_next/image|signin|signup|.*\\..*$).*)",
  ],
}
