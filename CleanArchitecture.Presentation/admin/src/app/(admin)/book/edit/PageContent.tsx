"use client";

import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookForm from "@/components/book/BookForm";
import { useLanguage } from "@/context/LanguageContext";

interface PageContentProps {
  /** Absent when creating a book. */
  id?: string;
}

export default function PageContent({ id }: PageContentProps) {
  const { t } = useLanguage();

  return (
    <div>
      <PageBreadcrumb pageTitle={id ? t("edit_book") : t("add_new_book")} />
      <div className="space-y-6">
        <BookForm id={id} />
      </div>
    </div>
  );
}
