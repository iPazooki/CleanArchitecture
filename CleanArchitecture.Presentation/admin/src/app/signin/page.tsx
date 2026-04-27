"use client";

import { useSearchParams } from "next/navigation";
import { signIn } from "next-auth/react";
import { Suspense, useEffect, useState } from "react";
import { getAuthProviderAction } from "./actions";

function resolveCallbackUrl(value: string | null): string {
  if (!value) {
    return "/";
  }

  if (value.startsWith("/")) {
    return value;
  }

  if (!URL.canParse(value)) {
    return "/";
  }

  const parsedUrl = new URL(value);
  return `${parsedUrl.pathname}${parsedUrl.search}${parsedUrl.hash}` || "/";
}

function SignInContent() {
  const searchParams = useSearchParams();
  const callbackUrl = resolveCallbackUrl(searchParams.get("callbackUrl"));
  const [provider, setProvider] = useState<string | null>(null);

  useEffect(() => {
    getAuthProviderAction().then(providerName => {
      setProvider(providerName.toLowerCase() === "entra" ? "azure-ad" : "keycloak");
    });
  }, []);

  useEffect(() => {
    if (provider) {
      void signIn(provider, { callbackUrl });
    }
  }, [provider, callbackUrl]);

  return (
    <div className="flex min-h-screen items-center justify-center">
      <div className="text-center">
        <h1 className="mb-2 text-xl font-semibold">Redirecting to Sign In...</h1>
        <p className="text-gray-500">Please wait while we redirect you to the login page.</p>
      </div>
    </div>
  );
}

export default function SignIn() {
  return (
    <Suspense fallback={<div>Loading...</div>}>
      <SignInContent />
    </Suspense>
  );
}
