"use client";

import type { PropsWithChildren } from "react";
import { SessionProvider } from "next-auth/react";

export default function AuthProvider({ children }: PropsWithChildren) {
  return <SessionProvider refetchOnWindowFocus={false}>{children}</SessionProvider>;
}
