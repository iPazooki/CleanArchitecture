"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useGetApiV1BooksId } from "@/lib/api/books/books";
import { extractApiErrors } from "@/lib/utils/error-handler";
import { useBookMutation } from "@/hooks/useBookMutation";
import ComponentCard from "../common/ComponentCard";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import { bookSchema, type BookFormValues } from "@/lib/validations/book";
import { genreOptions } from "@/lib/books/genre";
import { useLanguage } from "@/context/LanguageContext";

interface BookFormProps {
  id?: string;
}

export default function BookForm({ id }: BookFormProps) {
  const isEditMode = Boolean(id);
  const bookId = id ?? "";
  const router = useRouter();
  const { t } = useLanguage();

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<BookFormValues>({
    resolver: zodResolver(bookSchema),
    mode: "onBlur",
    defaultValues: {
      title: "",
      genre: "",
    },
  });

  const { submit, isSaving, serverErrors } = useBookMutation({ id, setError });

  const {
    data: bookResponse,
    error: bookError,
    isLoading: isFetchingBook,
  } = useGetApiV1BooksId(bookId, {
    query: {
      enabled: isEditMode,
    },
  });

  useEffect(() => {
    if (
      !isEditMode ||
      bookResponse?.status !== 200 ||
      !bookResponse.data.isSuccess ||
      !bookResponse.data.value
    ) {
      return;
    }

    reset({
      title: bookResponse.data.value.title,
      genre: bookResponse.data.value.genre,
    });
  }, [bookResponse, isEditMode, reset]);

  const displayedServerErrors = bookError ? extractApiErrors(bookError) : serverErrors;

  if (isFetchingBook) {
    return <div>{t("loading_book_data")}</div>;
  }

  return (
    <ComponentCard title={isEditMode ? t("edit_book") : t("add_new_book")}>
      <form onSubmit={handleSubmit(submit)} className="space-y-6">
        {displayedServerErrors.length > 0 ? (
          <div className="rounded-md bg-red-50 p-4" aria-live="assertive">
            <h3 className="text-sm font-medium text-red-800">{t("validation_errors")}</h3>
            <div className="mt-2 text-sm text-red-700">
              <ul className="list-disc space-y-1 pl-5">
                {displayedServerErrors.map((error) => (
                  <li key={`${error.code}-${error.message}`}>{error.message}</li>
                ))}
              </ul>
            </div>
          </div>
        ) : null}

        <div>
          <Label htmlFor="title">
            {t("title")} <span className="text-red-500">*</span>
          </Label>
          <Input
            id="title"
            type="text"
            placeholder={t("book_title_placeholder")}
            aria-invalid={errors.title ? "true" : "false"}
            disabled={isSaving}
            {...register("title")}
          />
          {errors.title ? (
            <p className="mt-1 text-sm text-red-600">{errors.title.message}</p>
          ) : null}
        </div>

        <div>
          <Label htmlFor="genre">
            {t("genre")} <span className="text-red-500">*</span>
          </Label>
          <select
            id="genre"
            aria-invalid={errors.genre ? "true" : "false"}
            className="w-full rounded-lg border border-stroke bg-transparent px-5 py-3 outline-none transition focus:border-primary active:border-primary disabled:cursor-default disabled:bg-whiter dark:border-form-strokedark dark:bg-form-input dark:focus:border-primary"
            disabled={isSaving}
            {...register("genre")}
          >
            <option value="">{t("select_genre")}</option>
            {genreOptions.map((genreOption) => (
              <option key={genreOption.value} value={genreOption.value}>
                {genreOption.label}
              </option>
            ))}
          </select>
          {errors.genre ? (
            <p className="mt-1 text-sm text-red-600">{errors.genre.message}</p>
          ) : null}
          <p className="mt-1 text-sm text-gray-500">
            {t("genre_help_text")}
          </p>
        </div>

        <div className="flex items-center gap-3">
          <Button type="submit" disabled={isSaving}>
            {isSaving ? t("saving") : isEditMode ? t("update_book") : t("create_book")}
          </Button>
          <Button
            type="button"
            variant="outline"
            onClick={() => router.back()}
            disabled={isSaving}
          >
            {t("cancel")}
          </Button>
        </div>
      </form>
    </ComponentCard>
  );
}
