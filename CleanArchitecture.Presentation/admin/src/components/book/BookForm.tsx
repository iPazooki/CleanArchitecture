"use client";

import { zodResolver } from "@hookform/resolvers/zod";
import { useEffect } from "react";
import { useForm } from "react-hook-form";
import { useBook } from "@/lib/books/book-queries";
import { extractApiErrors } from "@/lib/utils/error-handler";
import { useBookMutation } from "@/hooks/useBookMutation";
import useGoBack from "@/hooks/useGoBack";
import ComponentCard from "../common/ComponentCard";
import ErrorAlert from "../common/ErrorAlert";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import { bookSchema, type BookFormValues } from "@/lib/validations/book";
import { genreOptions } from "@/lib/books/genre";
import { useLanguage, useTranslateMessage } from "@/context/LanguageContext";

interface BookFormProps {
  id?: string;
}

const selectClassName =
  "w-full rounded-lg border border-stroke bg-transparent px-5 py-3 outline-none transition focus:border-primary active:border-primary disabled:cursor-default disabled:bg-whiter dark:border-form-strokedark dark:bg-form-input dark:focus:border-primary";

export default function BookForm({ id }: BookFormProps) {
  const isEditMode = Boolean(id);
  const goBack = useGoBack();
  const { t } = useLanguage();
  const translateMessage = useTranslateMessage();

  const {
    register,
    handleSubmit,
    reset,
    setError,
    formState: { errors },
  } = useForm<BookFormValues>({
    resolver: zodResolver(bookSchema),
    mode: "onBlur",
  });

  const { submit, isSaving, serverErrors } = useBookMutation({ id, setError });
  const { book, error: bookError, isLoading: isFetchingBook } = useBook(id ?? "");

  useEffect(() => {
    if (book) {
      reset({ title: book.title, genre: book.genre as BookFormValues["genre"] });
    }
  }, [book, reset]);

  const displayedServerErrors = bookError ? extractApiErrors(bookError) : serverErrors;

  if (isEditMode && isFetchingBook) {
    return <div>{t("loading_book_data")}</div>;
  }

  return (
    <ComponentCard title={isEditMode ? t("edit_book") : t("add_new_book")}>
      <form onSubmit={handleSubmit(submit)} className="space-y-6">
        <ErrorAlert errors={displayedServerErrors} title={t("validation_errors")} />

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
          {errors.title?.message ? (
            <p className="mt-1 text-sm text-red-600">{translateMessage(errors.title.message)}</p>
          ) : null}
        </div>

        <div>
          <Label htmlFor="genre">
            {t("genre")} <span className="text-red-500">*</span>
          </Label>
          <select
            id="genre"
            aria-invalid={errors.genre ? "true" : "false"}
            className={selectClassName}
            disabled={isSaving}
            defaultValue=""
            {...register("genre")}
          >
            <option value="">{t("select_genre")}</option>
            {genreOptions.map((genreOption) => (
              <option key={genreOption.value} value={genreOption.value}>
                {genreOption.label}
              </option>
            ))}
          </select>
          {errors.genre?.message ? (
            <p className="mt-1 text-sm text-red-600">{translateMessage(errors.genre.message)}</p>
          ) : null}
          <p className="mt-1 text-sm text-gray-500">{t("genre_help_text")}</p>
        </div>

        <div className="flex items-center gap-3">
          <Button type="submit" disabled={isSaving}>
            {isSaving ? t("saving") : isEditMode ? t("update_book") : t("create_book")}
          </Button>
          <Button type="button" variant="outline" onClick={goBack} disabled={isSaving}>
            {t("cancel")}
          </Button>
        </div>
      </form>
    </ComponentCard>
  );
}
