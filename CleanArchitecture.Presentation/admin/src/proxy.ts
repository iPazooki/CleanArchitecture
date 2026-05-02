import { withAuth } from "next-auth/middleware";

export default withAuth({
  callbacks: {
    authorized: ({ token }) => Boolean(token),
  },
});

export const config = {
  matcher: [
    "/((?!api/auth|_next/static|_next/image|signin|signup|.*\\..*$).*)",
  ],
};
