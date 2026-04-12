"use client";

import { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import { useRouter } from "next/navigation";
import type { UseFormSetError } from "react-hook-form";
import {
  getGetApiV1BooksIdQueryKey,
  getGetApiV1BooksQueryKey,
  usePostApiV1Books,
  usePutApiV1BooksId,
} from "@/lib/api/books/books";
import type { CreateBookCommand, UpdateBookCommand } from "@/lib/api/model";
import {
  extractApiErrors,
  getFieldErrors,
  type DomainError,
} from "@/lib/utils/error-handler";
import type { BookFormValues } from "@/lib/validations/book";

const fieldErrorMap = {
  Title: "title",
  Genre: "genre",
} satisfies Record<string, keyof CreateBookCommand>;

interface UseBookMutationOptions {
  id?: string;
  setError: UseFormSetError<BookFormValues>;
}

export function useBookMutation({ id, setError }: UseBookMutationOptions) {
  const isEditMode = Boolean(id);
  const bookId = id ?? "";
  const router = useRouter();
  const queryClient = useQueryClient();
  const [serverErrors, setServerErrors] = useState<DomainError[]>([]);

  async function invalidateBookQueries(): Promise<void> {
    await queryClient.invalidateQueries({
      queryKey: getGetApiV1BooksQueryKey(),
    });

    if (isEditMode) {
      await queryClient.invalidateQueries({
        queryKey: getGetApiV1BooksIdQueryKey(bookId),
      });
    }
  }

  function applyDomainErrors(domainErrors: DomainError[]): void {
    const fieldErrors = getFieldErrors(domainErrors, fieldErrorMap);

    (Object.keys(fieldErrors) as Array<keyof CreateBookCommand>).forEach((fieldName) => {
      const message = fieldErrors[fieldName];
      if (message) {
        setError(fieldName, {
          type: "server",
          message,
        });
      }
    });
  }

  function handleMutationError(error: unknown): void {
    const domainErrors = extractApiErrors(error);
    setServerErrors(domainErrors);
    applyDomainErrors(domainErrors);
  }

  const createMutation = usePostApiV1Books({
    mutation: {
      onSuccess: async (response) => {
        if (response.status === 201) {
          await invalidateBookQueries();
          router.push("/book/list");
        }
      },
      onError: handleMutationError,
    },
  });

  const updateMutation = usePutApiV1BooksId({
    mutation: {
      onSuccess: async (response) => {
        if (response.status === 204) {
          await invalidateBookQueries();
          router.push("/book/list");
        }
      },
      onError: handleMutationError,
    },
  });

  const isSaving = createMutation.isPending || updateMutation.isPending;

  function submit(formData: BookFormValues): void {
    setServerErrors([]);

    if (isEditMode) {
      const updateCommand: UpdateBookCommand = {
        id: bookId,
        title: formData.title,
        genre: formData.genre,
      };

      updateMutation.mutate({
        id: bookId,
        data: updateCommand,
      });
      return;
    }

    createMutation.mutate({ data: formData as CreateBookCommand });
  }

  return {
    submit,
    isSaving,
    serverErrors,
    setServerErrors,
  };
}