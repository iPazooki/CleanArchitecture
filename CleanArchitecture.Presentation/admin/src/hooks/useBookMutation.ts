"use client";

import { useCallback, useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import type { UseFormSetError } from "react-hook-form";
import {
  useDeleteApiV1BooksId,
  usePostApiV1Books,
  usePutApiV1BooksId,
} from "@/lib/api/books/books";
import { bookQueryKeys } from "@/lib/books/book-queries";
import type { CreateBookCommand } from "@/lib/api/model";
import { extractApiErrors, getFieldErrors, type DomainError } from "@/lib/utils/error-handler";
import type { BookFormValues } from "@/lib/validations/book";

const fieldErrorMap = {
  Title: "title",
  Genre: "genre",
} satisfies Record<string, keyof CreateBookCommand>;

const unexpectedResponseError: DomainError = {
  code: "UnexpectedResponse",
  message: "unexpected_response",
};

/** Invalidates the list, plus the detail entry when one book was touched. */
function useInvalidateBooks() {
  const queryClient = useQueryClient();

  return useCallback(
    async (bookId?: string): Promise<void> => {
      await queryClient.invalidateQueries({ queryKey: bookQueryKeys.list() });

      if (bookId) {
        await queryClient.invalidateQueries({ queryKey: bookQueryKeys.detail(bookId) });
      }
    },
    [queryClient],
  );
}

interface UseBookMutationOptions {
  id?: string;
  setError: UseFormSetError<BookFormValues>;
}

export function useBookMutation({ id, setError }: UseBookMutationOptions) {
  const isEditMode = Boolean(id);
  const bookId = id ?? "";
  const router = useRouter();
  const invalidateBooks = useInvalidateBooks();
  const [serverErrors, setServerErrors] = useState<DomainError[]>([]);

  function applyDomainErrors(domainErrors: DomainError[]): void {
    const fieldErrors = getFieldErrors(domainErrors, fieldErrorMap);

    (Object.keys(fieldErrors) as Array<keyof CreateBookCommand>).forEach((fieldName) => {
      const message = fieldErrors[fieldName];
      if (message) {
        setError(fieldName, { type: "server", message });
      }
    });
  }

  function handleMutationError(error: unknown): void {
    const domainErrors = extractApiErrors(error);
    setServerErrors(domainErrors);
    applyDomainErrors(domainErrors);
  }

  async function handleSaved(status: number, expectedStatus: number): Promise<void> {
    if (status !== expectedStatus) {
      setServerErrors([unexpectedResponseError]);
      return;
    }

    await invalidateBooks(isEditMode ? bookId : undefined);
    router.push("/book/list");
  }

  const createMutation = usePostApiV1Books({
    mutation: {
      onSuccess: (response) => handleSaved(response.status, 201),
      onError: handleMutationError,
    },
  });

  const updateMutation = usePutApiV1BooksId({
    mutation: {
      onSuccess: (response) => handleSaved(response.status, 204),
      onError: handleMutationError,
    },
  });

  function submit(formData: BookFormValues): void {
    setServerErrors([]);

    if (isEditMode) {
      updateMutation.mutate({ id: bookId, data: { id: bookId, ...formData } });
      return;
    }

    createMutation.mutate({ data: formData });
  }

  return {
    submit,
    isSaving: createMutation.isPending || updateMutation.isPending,
    serverErrors,
    setServerErrors,
  };
}

interface UseDeleteBookOptions {
  onDeleted: () => void;
}

export function useDeleteBook({ onDeleted }: UseDeleteBookOptions) {
  const invalidateBooks = useInvalidateBooks();

  const deleteMutation = useDeleteApiV1BooksId({
    mutation: {
      onSuccess: async () => {
        await invalidateBooks();
        onDeleted();
      },
    },
  });

  return {
    deleteBook: (bookId: string) => deleteMutation.mutate({ id: bookId }),
    isDeleting: deleteMutation.isPending,
    error: deleteMutation.error,
  };
}
