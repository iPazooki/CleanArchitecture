"use client";

import Link from "next/link";
import type { BookResponse } from "@/lib/api/model";
import { getGenreLabel } from "@/lib/books/genre";
import { Table, TableBody, TableCell, TableHeader, TableRow } from "../ui/table";
import { useLanguage } from "@/context/LanguageContext";

interface BookTableProps {
  books: BookResponse[];
  canEdit: boolean;
  canDelete: boolean;
  onDelete: (book: BookResponse) => void;
}

const headerCellClassName =
  "px-5 py-3 text-start text-theme-xs font-medium text-gray-500 dark:text-gray-400";
const bodyCellClassName = "px-5 py-4 text-sm text-gray-800 dark:text-white/90";

export default function BookTable({ books, canEdit, canDelete, onDelete }: BookTableProps) {
  const { t } = useLanguage();

  return (
    <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-gray-800 dark:bg-white/5">
      <div className="max-w-full overflow-x-auto">
        <Table>
          <TableHeader className="border-b border-gray-100 dark:border-white/5">
            <TableRow>
              <TableCell isHeader className={headerCellClassName}>
                {t("title")}
              </TableCell>
              <TableCell isHeader className={headerCellClassName}>
                {t("genre")}
              </TableCell>
              <TableCell isHeader className={headerCellClassName}>
                {t("actions")}
              </TableCell>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-100 dark:divide-white/5">
            {books.map((book) => (
              <TableRow key={book.id}>
                <TableCell className={bodyCellClassName}>{book.title}</TableCell>
                <TableCell className={bodyCellClassName}>{getGenreLabel(book.genre)}</TableCell>
                <TableCell className={bodyCellClassName}>
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
                        onClick={() => onDelete(book)}
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
                <TableCell className={`${bodyCellClassName} text-center`} colSpan={3}>
                  {t("no_books_found")}
                </TableCell>
              </TableRow>
            ) : null}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
