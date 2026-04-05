"use client";

import { useQueryClient } from "@tanstack/react-query";
import Link from "next/link";
import { useState } from "react";
import {
  getGetApiV1BooksQueryKey,
  useDeleteApiV1BooksId,
  useGetApiV1Books,
} from "@/lib/api/books/books";
import type { BookResponse } from "@/lib/api/model";
import {
  extractApiErrors,
  formatErrorMessages,
} from "@/lib/utils/error-handler";
import { useModal } from "@/hooks/useModal";
import { useUserPermissions } from "@/hooks/useUserPermissions";
import { getGenreLabel } from "@/lib/books/genre";
import Button from "../ui/button/Button";
import { Modal } from "../ui/modal";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../ui/table";
import { useLanguage } from "@/context/LanguageContext";

export default function BookTable() {
  const queryClient = useQueryClient();
  const { data: response, error, isLoading } = useGetApiV1Books();
  const { isOpen, openModal, closeModal } = useModal();
  const [bookToDelete, setBookToDelete] = useState<BookResponse | null>(null);
  const { canEdit, canDelete } = useUserPermissions();
  const { t } = useLanguage();

  const books =
    response?.status === 200 && response.data.isSuccess && Array.isArray(response.data.value?.items)
      ? response.data.value.items
      : [];

  function handleDeleteClick(book: BookResponse): void {
    setBookToDelete(book);
    openModal();
  }

  function handleCancelDelete(): void {
    closeModal();
    setBookToDelete(null);
  }

  const deleteMutation = useDeleteApiV1BooksId({
    mutation: {
      onSuccess: async () => {
        await queryClient.invalidateQueries({ queryKey: getGetApiV1BooksQueryKey() });
        handleCancelDelete();
      },
    },
  });

  function handleConfirmDelete(): void {
    if (!bookToDelete?.id) {
      return;
    }

    deleteMutation.mutate({ id: bookToDelete.id });
  }

  const loadErrorMessage = error ? formatErrorMessages(extractApiErrors(error)) : null;
  const deleteErrorMessage = deleteMutation.error
    ? formatErrorMessages(extractApiErrors(deleteMutation.error))
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
      {deleteErrorMessage ? (
        <div className="mb-4 rounded-md border border-red-200 bg-red-50 p-4 text-sm text-red-700">
          {deleteErrorMessage}
        </div>
      ) : null}

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
                      <Link href={`/book/detail/${book.id}`} className="text-blue-500 hover:text-blue-700">
                        {t("view")}
                      </Link>
                      {canEdit ? (
                        <Link href={`/book/edit/${book.id}`} className="text-orange-500 hover:text-orange-700">
                          {t("edit")}
                        </Link>
                      ) : null}
                      {canDelete ? (
                        <button
                          type="button"
                          onClick={() => handleDeleteClick(book)}
                          className="text-red-500 hover:text-red-700 disabled:cursor-not-allowed disabled:opacity-50"
                          disabled={deleteMutation.isPending}
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

      <Modal isOpen={isOpen} onClose={handleCancelDelete} className="m-4 max-w-175">
        <div className="p-6">
          <h2 className="mb-4 text-xl font-semibold text-gray-900 dark:text-white">
            {t("confirm_delete")}
          </h2>
          <p className="mb-6 text-gray-600 dark:text-gray-300">
            {t("confirm_delete_message_part1")}
            {bookToDelete ? ` \"${bookToDelete.title}\"` : t("confirm_delete_message_part2")}
            {t("confirm_delete_message_part3")}
          </p>
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              onClick={handleCancelDelete}
              variant="outline"
              disabled={deleteMutation.isPending}
            >
              {t("cancel")}
            </Button>
            <Button
              type="button"
              onClick={handleConfirmDelete}
              variant="primary"
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? t("deleting") : t("delete")}
            </Button>
          </div>
        </div>
      </Modal>
    </>
  );
}
