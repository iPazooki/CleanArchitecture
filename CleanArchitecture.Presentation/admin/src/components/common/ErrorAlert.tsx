"use client";

import { useTranslateMessage } from "@/context/LanguageContext";
import type { DomainError } from "@/lib/utils/error-handler";

interface ErrorAlertProps {
  errors: DomainError[];
  /** Heading shown above the list. Omit for a bare list. */
  title?: string;
}

export default function ErrorAlert({ errors, title }: ErrorAlertProps) {
  const translateMessage = useTranslateMessage();

  if (errors.length === 0) {
    return null;
  }

  return (
    <div
      className="rounded-md border border-red-200 bg-red-50 p-4 dark:border-red-500/30 dark:bg-red-500/10"
      role="alert"
      aria-live="assertive"
    >
      {title ? (
        <h3 className="text-sm font-medium text-red-800 dark:text-red-300">{title}</h3>
      ) : null}
      <ul
        className={`space-y-1 text-sm text-red-700 dark:text-red-400 ${
          title ? "mt-2 list-disc pl-5" : ""
        }`}
      >
        {errors.map((error) => (
          <li key={`${error.code}-${error.message}`}>{translateMessage(error.message)}</li>
        ))}
      </ul>
    </div>
  );
}
