"use client";

import type { BookResponse } from "@/lib/api/model";
import { useDeleteBook } from "@/hooks/useBookMutation";
import { extractApiErrors } from "@/lib/utils/error-handler";
import { useLanguage } from "@/context/LanguageContext";
import ErrorAlert from "../common/ErrorAlert";
import Button from "../ui/button/Button";
import { Modal } from "../ui/modal";

interface BookDeleteModalProps {
  book: BookResponse | null;
  isOpen: boolean;
  onClose: () => void;
}

export default function BookDeleteModal({ book, isOpen, onClose }: BookDeleteModalProps) {
  const { t } = useLanguage();
  const { deleteBook, isDeleting, error } = useDeleteBook({ onDeleted: onClose });

  function handleConfirm(): void {
    if (book?.id) {
      deleteBook(book.id);
    }
  }

  return (
    <Modal isOpen={isOpen} onClose={onClose} className="m-4 max-w-175">
      <div className="p-6">
        <h2 className="mb-4 text-xl font-semibold text-gray-900 dark:text-white">
          {t("confirm_delete")}
        </h2>
        {error ? (
          <div className="mb-4">
            <ErrorAlert errors={extractApiErrors(error)} />
          </div>
        ) : null}
        <p className="mb-6 text-gray-600 dark:text-gray-300">
          {t("confirm_delete_message_part1")}
          {book ? ` "${book.title}"` : t("confirm_delete_message_part2")}
          {t("confirm_delete_message_part3")}
        </p>
        <div className="flex justify-end gap-3">
          <Button type="button" onClick={onClose} variant="outline" disabled={isDeleting}>
            {t("cancel")}
          </Button>
          <Button type="button" onClick={handleConfirm} variant="primary" disabled={isDeleting}>
            {isDeleting ? t("deleting") : t("delete")}
          </Button>
        </div>
      </div>
    </Modal>
  );
}
