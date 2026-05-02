"use client";

import Link from "next/link";
import { useState } from "react";
import { useGetApiV1Books } from "@/lib/api/books/books";
import type { BookResponse } from "@/lib/api/model";
import {
  extractApiErrors,
  formatErrorMessages,
} from "@/lib/utils/error-handler";
import { useModal } from "@/hooks/useModal";
import { useUserPermissions } from "@/hooks/useUserPermissions";
import { getGenreLabel } from "@/lib/books/genre";
import BookDeleteModal from "./BookDeleteModal";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../ui/table";
import { useLanguage } from "@/context/LanguageContext";

export default function BookTable() {
  const { data: response, error, isLoading } = useGetApiV1Books();
  const { isOpen, openModal, closeModal } = useModal();
  const [bookToDelete, setBookToDelete] = useState<BookResponse | null>(null);
  const { canEdit, canDelete } = useUserPermissions();
  const { t } = useLanguage();

  const books =
    response?.status === 200 &&
    response.data.isSuccess &&
    Array.isArray(response.data.value?.items)
      ? response.data.value.items
      : [];

  function handleDeleteClick(book: BookResponse): void {
    setBookToDelete(book);
    openModal();
  }

  function handleCloseModal(): void {
    closeModal();
    setBookToDelete(null);
  }

  const loadErrorMessage = error
    ? formatErrorMessages(extractApiErrors(error))
    : null;

  if (isLoading) {
    return <div>{t("loading_books")}</div>;
  }

  if (loadErrorMessage) {
    return (
      <div className="rounded-md border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        {loadErrorMessage}
      </div>
    );
  }

  return (
    <>
      <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-gray-800 dark:bg-white/5">
        <div className="max-w-full overflow-x-auto">
          <Table>
            <TableHeader className="border-b border-gray-100 dark:border-white/5">
              <TableRow>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  {t("title")}
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  {t("genre")}
                </TableCell>
                <TableCell
                  isHeader
                  className="px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400"
                >
                  {t("actions")}
                </TableCell>
              </TableRow>
            </TableHeader>
            <TableBody className="divide-y divide-gray-100 dark:divide-white/5">
              {books.map((book) => (
                <TableRow key={book.id}>
                  <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                    {book.title}
                  </TableCell>
                  <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                    {getGenreLabel(book.genre)}
                  </TableCell>
                  <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                    <div className="flex items-center gap-3">
                      <Link
                        href={`/book/detail/${book.id}`}
                        className="text-blue-500 hover:text-blue-700"
                      >
                        {t("view")}
                      </Link>
                      {canEdit ? (
                        <Link
                          href={`/book/edit/${book.id}`}
                          className="text-orange-500 hover:text-orange-700"
                        >
                          {t("edit")}
                        </Link>
                      ) : null}
                      {canDelete ? (
                        <button
                          type="button"
                          onClick={() => handleDeleteClick(book)}
                          className="text-red-500 hover:text-red-700"
                        >
                          {t("delete")}
                        </button>
                      ) : null}
                    </div>
                  </TableCell>
                </TableRow>
              ))}
              {books.length === 0 ? (
                <TableRow>
                  <TableCell
                    className="px-5 py-4 text-center text-sm text-gray-800 dark:text-white/90"
                    colSpan={3}
                  >
                    {t("no_books_found")}
                  </TableCell>
                </TableRow>
              ) : null}
            </TableBody>
          </Table>
        </div>
      </div>

      <BookDeleteModal
        book={bookToDelete}
        isOpen={isOpen}
        onClose={handleCloseModal}
      />
    </>
  );
}
