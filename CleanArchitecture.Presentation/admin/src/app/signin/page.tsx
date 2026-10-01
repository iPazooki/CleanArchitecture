"use client";

import { useSearchParams } from "next/navigation";
import { signIn } from "next-auth/react";
import { Suspense, useEffect, useState } from "react";
import Button from "@/components/ui/button/Button";
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
  const error = searchParams.get("error");
  const [provider, setProvider] = useState<string | null>(null);

  useEffect(() => {
    void getAuthProviderAction().then(setProvider);
  }, []);

  // NextAuth lands back here with `error` when sign-in fails. Redirecting again
  // would loop forever while the IdP session is still live, so wait for the user.
  useEffect(() => {
    if (provider && !error) {
      void signIn(provider, { callbackUrl });
    }
  }, [provider, error, callbackUrl]);

  if (error) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="mb-2 text-xl font-semibold">Sign-in failed</h1>
          <p className="mb-6 text-gray-500">
            Something went wrong while signing you in ({error}). Please try again.
          </p>
          <Button
            disabled={!provider}
            onClick={() => {
              if (provider) {
                void signIn(provider, { callbackUrl });
              }
            }}
          >
            Try again
          </Button>
        </div>
      </div>
    );
  }

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
