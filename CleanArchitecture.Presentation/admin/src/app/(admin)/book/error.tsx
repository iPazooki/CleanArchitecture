"use client";

import { useLanguage } from "@/context/LanguageContext";

interface BookErrorProps {
  error: Error & { digest?: string };
  reset: () => void;
}

export default function BookError({ error, reset }: BookErrorProps) {
  const { t } = useLanguage();

  return (
    <div className="rounded-md border border-red-200 bg-red-50 p-6 text-center">
      <h2 className="mb-2 text-lg font-semibold text-red-800">{t("something_went_wrong")}</h2>
      <p className="mb-4 text-sm text-red-700">{error.message}</p>
      <button
        type="button"
        onClick={reset}
        className="inline-flex items-center justify-center rounded-lg bg-red-600 px-4 py-2 text-sm font-medium text-white hover:bg-red-700"
      >
        {t("try_again")}
      </button>
    </div>
  );
}
