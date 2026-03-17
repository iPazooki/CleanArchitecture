"use client";
import React, { useState } from "react";
import { useQueryClient } from "@tanstack/react-query";
import {
  Table,
  TableBody,
  TableCell,
  TableHeader,
  TableRow,
} from "../ui/table";
import {
  getGetApiV1BooksQueryKey,
  useDeleteApiV1BooksId,
  useGetApiV1Books,
} from "@/lib/api/books/books";
import { BookResponse } from "@/lib/api/model/bookResponse";
import Button from "../ui/button/Button";
import Link from "next/link";
import { Modal } from "../ui/modal";
import { useModal } from "@/hooks/useModal";

export default function BookTable() {
  const queryClient = useQueryClient();
  const { data: response, isLoading } = useGetApiV1Books({
    query: {
      staleTime: Infinity,
      gcTime: Infinity,
    },
  });
  const { isOpen, openModal, closeModal } = useModal();
  const [bookToDelete, setBookToDelete] = useState<string | null>(null);

  const deleteMutation = useDeleteApiV1BooksId({
    mutation: {
      onSuccess: () => {
        queryClient.invalidateQueries({ queryKey: getGetApiV1BooksQueryKey() });
        closeModal();
        setBookToDelete(null);
      },
    },
  });

  const handleDeleteClick = (id: string) => {
    setBookToDelete(id);
    openModal();
  };

  const handleConfirmDelete = () => {
    if (bookToDelete) {
      deleteMutation.mutate({ id: bookToDelete });
    }
  };

  const handleCancelDelete = () => {
    closeModal();
    setBookToDelete(null);
  };

  if (isLoading) return <div>Loading books...</div>;

  const value = response?.status === 200 && response.data.isSuccess ? response.data.value : null;
  const books = (Array.isArray(value) ? value : (value ? [value] : [])) as BookResponse[];

  return (
    <>
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
                        onClick={() => handleDeleteClick(book.id!)}
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

      <Modal isOpen={isOpen} onClose={handleCancelDelete} className="max-w-[700px] m-4">
        <div className="p-6">
          <h2 className="text-xl font-semibold text-gray-900 dark:text-white mb-4">
            Confirm Delete
          </h2>
          <p className="text-gray-600 dark:text-gray-300 mb-6">
            Are you sure you want to delete this book? This action cannot be undone.
          </p>
          <div className="flex justify-end gap-3">
            <Button
              onClick={handleCancelDelete}
              variant="outline"
              disabled={deleteMutation.isPending}
            >
              Cancel
            </Button>
            <Button
              onClick={handleConfirmDelete}
              variant="primary"
              disabled={deleteMutation.isPending}
            >
              {deleteMutation.isPending ? "Deleting..." : "Delete"}
            </Button>
          </div>
        </div>
      </Modal>
    </>
  );
}
