"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useGetApiV1BooksId } from "@/lib/api/books/books";
import {
  extractApiErrors,
  formatErrorMessages,
} from "@/lib/utils/error-handler";
import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import { useUserPermissions } from "@/hooks/useUserPermissions";
import { getGenreLabel } from "@/lib/books/genre";

export default function BookDetailPage() {
  const params = useParams<{ id: string }>();
  const bookId = Array.isArray(params.id) ? params.id[0] : params.id;
  const router = useRouter();
  const { canEdit } = useUserPermissions();

  const { data: response, error, isLoading } = useGetApiV1BooksId(bookId ?? "", {
    query: {
      enabled: Boolean(bookId),
    },
  });

  if (isLoading) {
    return <div>Loading book details...</div>;
  }

  if (error) {
    return (
      <div className="rounded-md border border-red-200 bg-red-50 p-4 text-sm text-red-700">
        {formatErrorMessages(extractApiErrors(error))}
      </div>
    );
  }

  const book = response?.status === 200 && response.data.isSuccess ? response.data.value : null;
  if (!book) {
    return <div>Book not found.</div>;
  }

  return (
    <div>
      <PageBreadcrumb pageTitle="Book Details" />
      <div className="space-y-6">
        <ComponentCard title="Book Information">
          <div className="space-y-4">
            <div>
              <h4 className="text-sm font-medium text-gray-500 dark:text-gray-400">Title</h4>
              <p className="text-lg font-semibold text-gray-800 dark:text-white/90">{book.title}</p>
            </div>
            <div>
              <h4 className="text-sm font-medium text-gray-500 dark:text-gray-400">Genre</h4>
              <p className="text-lg font-semibold text-gray-800 dark:text-white/90">{getGenreLabel(book.genre)}</p>
            </div>
            <div className="flex gap-4 pt-4">
              {canEdit ? (
                <Link
                  href={`/book/edit/${book.id}`}
                  className="inline-flex items-center justify-center rounded-lg bg-orange-500 px-4 py-2 text-sm font-medium text-white hover:bg-orange-600"
                >
                  Edit Book
                </Link>
              ) : null}
              <button
                type="button"
                onClick={() => router.back()}
                className="inline-flex items-center justify-center rounded-lg border border-gray-300 bg-white px-4 py-2 text-sm font-medium text-gray-700 hover:bg-gray-50"
              >
                Back to List
              </button>
            </div>
          </div>
        </ComponentCard>
      </div>
    </div>
  );
}
