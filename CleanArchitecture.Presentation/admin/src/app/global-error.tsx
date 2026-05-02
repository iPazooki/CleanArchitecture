"use client";

interface GlobalErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function GlobalError({ error, reset }: GlobalErrorProps) {
  return (
    <html lang="en">
      <body className="antialiased">
        <div className="flex min-h-screen items-center justify-center bg-gray-50 p-6">
          <div className="w-full max-w-md rounded-lg border border-red-200 bg-white p-8 text-center shadow-sm">
            <h1 className="mb-2 text-xl font-semibold text-red-800">
              Something went wrong
            </h1>
            <p className="mb-6 text-sm text-red-600">{error.message}</p>
            <button
              type="button"
              onClick={reset}
              className="inline-flex items-center justify-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700"
            >
              Try again
            </button>
          </div>
        </div>
      </body>
    </html>
  );
}
