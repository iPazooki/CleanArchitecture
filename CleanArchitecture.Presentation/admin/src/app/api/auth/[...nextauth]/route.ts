import type { NextRequest } from "next/server";
import NextAuth from "next-auth";
import { getAuthOptions } from "@/lib/auth/auth-options";

type RouteHandlerContext = {
  params: Promise<{ nextauth: string[] }>;
};

async function handleAuth(request: NextRequest, context: RouteHandlerContext) {
  return NextAuth(request, context, getAuthOptions());
}

export function GET(request: NextRequest, context: RouteHandlerContext) {
  return handleAuth(request, context);
}

export function POST(request: NextRequest, context: RouteHandlerContext) {
  return handleAuth(request, context);
}