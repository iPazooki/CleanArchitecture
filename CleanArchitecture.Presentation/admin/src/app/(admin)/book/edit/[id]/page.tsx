"use client";

import { useParams } from "next/navigation";
import BookForm from "@/components/book/BookForm";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import { useLanguage } from "@/context/LanguageContext";

export default function EditBookPage() {
  const params = useParams<{ id: string }>();
  const bookId = Array.isArray(params.id) ? params.id[0] : params.id;
  const { t } = useLanguage();

  return (
    <div>
      <PageBreadcrumb pageTitle={t("edit_book")} />
      <div className="space-y-6">
        <BookForm id={bookId} />
      </div>
    </div>
  );
}
