"use client";

import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookTable from "@/components/book/BookTable";
import Link from "next/link";
import { useLanguage } from "@/context/LanguageContext";

export default function PageContent({ canCreate }: { canCreate: boolean }) {
  const { t } = useLanguage();

  return (
    <div>
      <PageBreadcrumb pageTitle={t("Books")} />
      <div className="space-y-6">
        <div className="flex justify-end">
          {canCreate ? (
            <Link
              href="/book/edit"
              className="inline-flex items-center justify-center rounded-lg bg-blue-600 px-4 py-2 text-sm font-medium text-white hover:bg-blue-700"
            >
              {t("add_new_book")}
            </Link>
          ) : null}
        </div>
        <ComponentCard title={t("all_books")}>
          <BookTable />
        </ComponentCard>
      </div>
    </div>
  );
}
