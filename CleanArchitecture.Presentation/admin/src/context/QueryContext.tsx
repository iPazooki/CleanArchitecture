"use client";

import { QueryClient, QueryClientProvider } from "@tanstack/react-query";
import dynamic from "next/dynamic";
import type { PropsWithChildren } from "react";
import { useState } from "react";
import { isApiError } from "@/lib/utils/api-error";

const ReactQueryDevtools = dynamic(
  () => import("@tanstack/react-query-devtools").then((module) => module.ReactQueryDevtools),
  { ssr: false },
);

const queryStaleTime = 60_000;
const queryGcTime = 5 * 60_000;
const maxQueryRetries = 2;

function shouldRetryQuery(failureCount: number, error: unknown): boolean {
  if (isApiError(error)) {
    return error.status >= 500 && failureCount < maxQueryRetries;
  }

  return failureCount < maxQueryRetries;
}

function createQueryClient(): QueryClient {
  return new QueryClient({
    defaultOptions: {
      queries: {
        staleTime: queryStaleTime,
        gcTime: queryGcTime,
        retry: shouldRetryQuery,
        refetchOnWindowFocus: false,
      },
      mutations: {
        retry: false,
      },
    },
  });
}

export default function QueryProvider({ children }: PropsWithChildren) {
  const [queryClient] = useState(createQueryClient);

  return (
    <QueryClientProvider client={queryClient}>
      {children}
      {process.env.NODE_ENV === "development" ? (
        <ReactQueryDevtools initialIsOpen={false} />
      ) : null}
    </QueryClientProvider>
  );
}
