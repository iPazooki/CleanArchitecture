"use client";

import Link from "next/link";
import ComponentCard from "@/components/common/ComponentCard";
import ErrorAlert from "@/components/common/ErrorAlert";
import useGoBack from "@/hooks/useGoBack";
import { useUserPermissions } from "@/hooks/useUserPermissions";
import { useBook } from "@/lib/books/book-queries";
import { getGenreLabel } from "@/lib/books/genre";
import { extractApiErrors } from "@/lib/utils/error-handler";
import { useLanguage } from "@/context/LanguageContext";

interface PageContentProps {
  id: string;
}

export default function PageContent({ id }: PageContentProps) {
  const goBack = useGoBack();
  const { canEdit } = useUserPermissions();
  const { t } = useLanguage();
  const { book, error, isLoading } = useBook(id);

  if (isLoading) {
    return <div>{t("loading_book_details")}</div>;
  }

  if (error) {
    return <ErrorAlert errors={extractApiErrors(error)} />;
  }

  if (!book) {
    return <div>{t("book_not_found")}</div>;
  }

  return (
    <ComponentCard title={t("book_information")}>
      <div className="space-y-4">
        <div>
          <h4 className="text-sm font-medium text-gray-500 dark:text-gray-400">{t("title")}</h4>
          <p className="text-lg font-semibold text-gray-800 dark:text-white/90">{book.title}</p>
        </div>
        <div>
          <h4 className="text-sm font-medium text-gray-500 dark:text-gray-400">{t("genre")}</h4>
          <p className="text-lg font-semibold text-gray-800 dark:text-white/90">
            {getGenreLabel(book.genre)}
          </p>
        </div>
        <div className="flex gap-4 pt-4">
          {canEdit ? (
            <Link
              href={`/book/edit/${book.id}`}
              className="inline-flex items-center justify-center rounded-lg bg-orange-500 px-4 py-2 text-sm font-medium text-white hover:bg-orange-600"
            >
              {t("edit_book")}
            </Link>
          ) : null}
          <button
            type="button"
            onClick={goBack}
            className="inline-flex items-center justify-center rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
          >
            {t("back_to_list")}
          </button>
        </div>
      </div>
    </ComponentCard>
  );
}
