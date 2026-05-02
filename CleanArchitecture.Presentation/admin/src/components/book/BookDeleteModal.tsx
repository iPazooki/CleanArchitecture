"use client";

import { useQueryClient } from "@tanstack/react-query";
import {
  getGetApiV1BooksQueryKey,
  useDeleteApiV1BooksId,
} from "@/lib/api/books/books";
import type { BookResponse } from "@/lib/api/model";
import {
  extractApiErrors,
  formatErrorMessages,
} from "@/lib/utils/error-handler";
import { useLanguage } from "@/context/LanguageContext";
import Button from "../ui/button/Button";
import { Modal } from "../ui/modal";

interface BookDeleteModalProps {
  book: BookResponse | null;
  isOpen: boolean;
  onClose: () => void;
}

export default function BookDeleteModal({
  book,
  isOpen,
  onClose,
}: BookDeleteModalProps) {
  const queryClient = useQueryClient();
  const { t } = useLanguage();

  const deleteMutation = useDeleteApiV1BooksId({
    mutation: {
      onSuccess: async () => {
        await queryClient.invalidateQueries({
          queryKey: getGetApiV1BooksQueryKey(),
        });
        onClose();
      },
    },
  });

  function handleConfirm(): void {
    if (!book?.id) return;
    deleteMutation.mutate({ id: book.id });
  }

  const errorMessage = deleteMutation.error
    ? formatErrorMessages(extractApiErrors(deleteMutation.error))
    : null;

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="m-4 max-w-175">
      <div className="p-6">
        <h2 className="mb-4 text-xl font-semibold text-gray-900 dark:text-white">
          {t("confirm_delete")}
        </h2>
        {errorMessage ? (
          <p className="mb-4 text-sm text-red-600">{errorMessage}</p>
        ) : null}
        <p className="mb-6 text-gray-600 dark:text-gray-300">
          {t("confirm_delete_message_part1")}
          {book ? ` "${book.title}"` : t("confirm_delete_message_part2")}
          {t("confirm_delete_message_part3")}
        </p>
        <div className="flex justify-end gap-3">
          <Button
            type="button"
            onClick={onClose}
            variant="outline"
            disabled={deleteMutation.isPending}
          >
            {t("cancel")}
          </Button>
          <Button
            type="button"
            onClick={handleConfirm}
            variant="primary"
            disabled={deleteMutation.isPending}
          >
            {deleteMutation.isPending ? t("deleting") : t("delete")}
          </Button>
        </div>
      </div>
    </Modal>
  );
}
