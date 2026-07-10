"use client";

import { MutationCache, QueryCache, QueryClient, QueryClientProvider } from "@tanstack/react-query";
import dynamic from "next/dynamic";
import { signIn } from "next-auth/react";
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

/**
 * The proxy answers 401 when the session is gone or its refresh token failed.
 * Re-authentication is handled here, once, rather than inside the fetch layer.
 */
function handleApiError(error: unknown): void {
  if (!isApiError(error) || error.status !== 401) {
    return;
  }

  const callbackUrl = `${window.location.pathname}${window.location.search}`;
  void signIn(undefined, { callbackUrl });
}

function createQueryClient(): QueryClient {
  return new QueryClient({
    queryCache: new QueryCache({ onError: handleApiError }),
    mutationCache: new MutationCache({ onError: handleApiError }),
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
