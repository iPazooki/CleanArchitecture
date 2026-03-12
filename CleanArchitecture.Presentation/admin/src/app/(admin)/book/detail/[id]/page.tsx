"use client";
import React, { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import ComponentCard from "@/components/common/ComponentCard";
import { getGetBookId } from "@/lib/api/book-endpoints/book-endpoints";
import { BookResponse } from "@/lib/api/model/bookResponse";
import Link from "next/link";

export default function BookDetailPage() {
  const { id } = useParams();
  const router = useRouter();
  const [book, setBook] = useState<BookResponse | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (id) {
      const fetchBook = async () => {
        try {
          const response = await getGetBookId(id as string);
          if (response.status === 200 && response.data.isSuccess) {
            setBook(response.data.value!);
          } else {
             console.error("Book not found or error in response");
          }
        } catch (error) {
          console.error("Failed to fetch book details", error);
        } finally {
          setLoading(false);
        }
      };
      fetchBook();
    }
  }, [id]);

  if (loading) return <div>Loading book details...</div>;
  if (!book) return <div>Book not found.</div>;

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
              <p className="text-lg font-semibold text-gray-800 dark:text-white/90">{book.genre}</p>
            </div>
            <div>
                <h4 className="text-sm font-medium text-gray-500 dark:text-gray-400">ID</h4>
                <p className="text-sm text-gray-800 dark:text-white/90">{book.id}</p>
            </div>
            <div className="flex gap-4 pt-4">
              <Link
                href={`/book/edit/${book.id}`}
                className="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-white bg-orange-500 rounded-lg hover:bg-orange-600"
              >
                Edit Book
              </Link>
              <button
                onClick={() => router.back()}
                className="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50"
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
