"use client";
import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  useGetApiV1BooksId,
  usePostApiV1Books,
  usePutApiV1BooksId,
} from "@/lib/api/books/books";
import { PostApiV1BooksBody } from "@/lib/api/zod/books/books";
import type { CreateBookCommand, UpdateBookCommand } from "@/lib/api/model";
import {
  extractApiErrors,
  getFieldErrors,
  type DomainError,
} from "@/lib/utils/error-handler";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import ComponentCard from "../common/ComponentCard";

interface BookFormProps {
  id?: string;
}

export default function BookForm({ id }: BookFormProps) {
  const router = useRouter();
  const [serverErrors, setServerErrors] = useState<DomainError[]>([]);

  const {
    register,
    handleSubmit,
    setValue,
    setError,
    formState: { errors },
  } = useForm<CreateBookCommand>({
    resolver: zodResolver(PostApiV1BooksBody),
    mode: "onBlur",
  });

  // --- Fetch existing book in edit mode ---
  const {
    data: bookResponse,
    isLoading: fetching,
    isError: fetchError,
  } = useGetApiV1BooksId(id!, { query: { enabled: !!id } });

  // Populate form when book data arrives
  useEffect(() => {
    if (bookResponse?.status === 200 && bookResponse.data.isSuccess) {
      const book = bookResponse.data.value!;
      setValue("title", book.title || "");
      setValue("genre", book.genre || "");
    }
  }, [bookResponse, setValue]);

  useEffect(() => {
    if (fetchError) {
      setServerErrors([
        { code: "FetchError", message: "Failed to load book data" },
      ]);
    }
  }, [fetchError]);

  // --- Mutations ---
  const applyFieldErrors = (domainErrors: DomainError[]) => {
    const fieldErrors = getFieldErrors(domainErrors, {
      Title: "title",
      Genre: "genre",
    });
    Object.entries(fieldErrors).forEach(([field, message]) => {
      setError(field as keyof CreateBookCommand, { type: "server", message });
    });
  };

  const createMutation = usePostApiV1Books({
    mutation: {
      onSuccess: (response) => {
        if (response.status === 201) {
          router.push("/book/list");
        }
      },
      onError: (error) => {
        const domainErrors = extractApiErrors(error);
        setServerErrors(domainErrors);
        applyFieldErrors(domainErrors);
      },
    },
  });

  const updateMutation = usePutApiV1BooksId({
    mutation: {
      onSuccess: (response) => {
        if (response.status === 204) {
          router.push("/book/list");
        }
      },
      onError: (error) => {
        const domainErrors = extractApiErrors(error);
        setServerErrors(domainErrors);
        applyFieldErrors(domainErrors);
      },
    },
  });

  const loading = createMutation.isPending || updateMutation.isPending;

  const onSubmit = (data: CreateBookCommand) => {
    setServerErrors([]);
    if (id) {
      updateMutation.mutate({ id, data: { id, ...data } as UpdateBookCommand });
    } else {
      createMutation.mutate({ data });
    }
  };

  if (fetching) return <div>Loading book data...</div>;

  return (
    <ComponentCard title={id ? "Edit Book" : "Add New Book"}>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        {/* Display server errors */}
        {serverErrors.length > 0 && (
          <div className="rounded-md bg-red-50 p-4">
            <div className="flex">
              <div className="ml-3">
                <h3 className="text-sm font-medium text-red-800">
                  Validation Errors
                </h3>
                <div className="mt-2 text-sm text-red-700">
                  <ul className="list-disc space-y-1 pl-5">
                    {serverErrors.map((error, index) => (
                      <li key={index}>{error.message}</li>
                    ))}
                  </ul>
                </div>
              </div>
            </div>
          </div>
        )}

        <div>
          <Label htmlFor="title">
            Title <span className="text-red-500">*</span>
          </Label>
          <Input
            id="title"
            type="text"
            placeholder="Book Title"
            {...register("title")}
            aria-invalid={errors.title ? "true" : "false"}
          />
          {errors.title && (
            <p className="mt-1 text-sm text-red-600">{errors.title.message}</p>
          )}
        </div>

        <div>
          <Label htmlFor="genre">
            Genre <span className="text-red-500">*</span>
          </Label>
          <select
            id="genre"
            {...register("genre")}
            className="w-full rounded-lg border border-stroke bg-transparent py-3 px-5 outline-none transition focus:border-primary active:border-primary disabled:cursor-default disabled:bg-whiter dark:border-form-strokedark dark:bg-form-input dark:focus:border-primary"
            aria-invalid={errors.genre ? "true" : "false"}
          >
            <option value="">Select Genre</option>
            <option value="F">Fiction</option>
            <option value="NF">Non-Fiction</option>
            <option value="M">Mystery</option>
          </select>
          {errors.genre && (
            <p className="mt-1 text-sm text-red-600">{errors.genre.message}</p>
          )}
          <p className="mt-1 text-sm text-gray-500">
            Select a genre: Fiction (F), Non-Fiction (NF), or Mystery (M)
          </p>
        </div>

        <div className="flex items-center gap-3">
          <Button disabled={loading}>
            {loading ? "Saving..." : id ? "Update Book" : "Create Book"}
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => router.back()}
            disabled={loading}
          >
            Cancel
          </Button>
        </div>
      </form>
    </ComponentCard>
  );
}
