"use client";

import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookForm from "@/components/book/BookForm";
import React from "react";
import { useLanguage } from "@/context/LanguageContext";

export default function PageContent() {
  const { t } = useLanguage();

  return (
    <div>
      <PageBreadcrumb pageTitle={t("add_new_book")} />
      <div className="space-y-6">
        <BookForm />
      </div>
    </div>
  );
}
