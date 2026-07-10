"use client";

import Link from "next/link";
import { useState } from "react";
import ComponentCard from "@/components/common/ComponentCard";
import ErrorAlert from "@/components/common/ErrorAlert";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import Pagination from "@/components/tables/Pagination";
import BookDeleteModal from "@/components/book/BookDeleteModal";
import BookTable from "@/components/book/BookTable";
import { useModal } from "@/hooks/useModal";
import { useUserPermissions } from "@/hooks/useUserPermissions";
import { buildBookListHref } from "@/lib/books/book-pagination";
import { useBooks } from "@/lib/books/book-queries";
import type { BookResponse } from "@/lib/api/model";
import { extractApiErrors } from "@/lib/utils/error-handler";
import { useLanguage } from "@/context/LanguageContext";

interface PageContentProps {
  canCreate: boolean;
  page: number;
  pageSize: number;
}

export default function PageContent({ canCreate, page, pageSize }: PageContentProps) {
  const { t } = useLanguage();
  const { canEdit, canDelete } = useUserPermissions();
  const { books, pagination, isLoading, error } = useBooks({ page, pageSize });
  const { isOpen, openModal, closeModal } = useModal();
  const [bookToDelete, setBookToDelete] = useState<BookResponse | null>(null);

  function handleDeleteClick(book: BookResponse): void {
    setBookToDelete(book);
    openModal();
  }

  function handleCloseModal(): void {
    closeModal();
    setBookToDelete(null);
  }

  return (
    <div>
      <PageBreadcrumb pageTitle={t("Books")} />
      <div className="space-y-6">
        <div className="flex justify-end">
          {canCreate ? (
            <Link
              href="/book/edit"
              className="inline-flex items-center justify-center rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t("add_new_book")}
            </Link>
          ) : null}
        </div>
        <ComponentCard title={t("all_books")}>
          {isLoading ? <div>{t("loading_books")}</div> : null}

          {error ? <ErrorAlert errors={extractApiErrors(error)} /> : null}

          {!isLoading && !error ? (
            <div className="space-y-4">
              <BookTable
                books={books}
                canEdit={canEdit}
                canDelete={canDelete}
                onDelete={handleDeleteClick}
              />
              <div className="flex justify-end">
                <Pagination
                  currentPage={pagination.page}
                  totalPages={pagination.totalPages}
                  buildHref={(targetPage) => buildBookListHref({ page: targetPage, pageSize })}
                  previousLabel={t("previous")}
                  nextLabel={t("next")}
                />
              </div>
            </div>
          ) : null}
        </ComponentCard>
      </div>

      <BookDeleteModal book={bookToDelete} isOpen={isOpen} onClose={handleCloseModal} />
    </div>
  );
}
