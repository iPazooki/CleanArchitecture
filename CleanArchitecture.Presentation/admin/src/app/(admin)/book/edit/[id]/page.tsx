"use client";

import { useParams } from "next/navigation";
import BookForm from "@/components/book/BookForm";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";

export default function EditBookPage() {
  const params = useParams<{ id: string }>();
  const bookId = Array.isArray(params.id) ? params.id[0] : params.id;

  return (
    <div>
      <PageBreadcrumb pageTitle="Edit Book" />
      <div className="space-y-6">
        <BookForm id={bookId} />
      </div>
    </div>
  );
}
