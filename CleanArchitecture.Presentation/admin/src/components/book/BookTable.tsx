"use client";
import React from "react";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../ui/table";
import { useGetApiV1Books, useDeleteApiV1BooksId } from "@/lib/api/books/books";
import { BookResponse } from "@/lib/api/model/bookResponse";
import Button from "../ui/button/Button";
import Link from "next/link";

export default function BookTable() {
  const { data: response, isLoading, refetch } = useGetApiV1Books();

  const deleteMutation = useDeleteApiV1BooksId({
    mutation: {
      onSuccess: () => {
        refetch();
      },
    },
  });

  const handleDelete = async (id: string) => {
    if (confirm("Are you sure you want to delete this book?")) {
      deleteMutation.mutate({ id });
    }
  };

  if (isLoading) return <div>Loading books...</div>;

  const value = response?.status === 200 && response.data.isSuccess ? response.data.value : null;
  const books = (Array.isArray(value) ? value : (value ? [value] : [])) as BookResponse[];

  return (
    <div className="overflow-hidden rounded-xl border border-gray-200 bg-white dark:border-gray-800 dark:bg-white/[0.03]">
      <div className="max-w-full overflow-x-auto">
        <Table>
          <TableHeader className="border-b border-gray-100 dark:border-white/[0.05]">
            <TableRow>
              <TableCell isHeader className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400">
                Title
              </TableCell>
              <TableCell isHeader className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400">
                Genre
              </TableCell>
              <TableCell isHeader className="px-5 py-3 font-medium text-gray-500 text-start text-theme-xs dark:text-gray-400">
                Actions
              </TableCell>
            </TableRow>
          </TableHeader>
          <TableBody className="divide-y divide-gray-100 dark:divide-white/[0.05]">
            {books.map((book) => (
              <TableRow key={book.id}>
                <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                  {book.title}
                </TableCell>
                <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                  {book.genre}
                </TableCell>
                <TableCell className="px-5 py-4 text-sm text-gray-800 dark:text-white/90">
                  <div className="flex items-center gap-3">
                    <Link href={`/book/detail/${book.id}`} className="text-blue-500 hover:text-blue-700">
                      View
                    </Link>
                    <Link href={`/book/edit/${book.id}`} className="text-orange-500 hover:text-orange-700">
                      Edit
                    </Link>
                    <button
                      onClick={() => handleDelete(book.id!)}
                      className="text-red-500 hover:text-red-700"
                    >
                      Delete
                    </button>
                  </div>
                </TableCell>
              </TableRow>
            ))}
            {books.length === 0 && (
               <TableRow>
                <TableCell className="px-5 py-4 text-sm text-center text-gray-800 dark:text-white/90">
                  No books found.
                </TableCell>
              </TableRow>
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  );
}
