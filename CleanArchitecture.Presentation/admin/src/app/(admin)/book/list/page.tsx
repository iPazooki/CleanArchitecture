import ComponentCard from "@/components/common/ComponentCard";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookTable from "@/components/book/BookTable";
import { Metadata } from "next";
import React from "react";
import Link from "next/link";

export const metadata: Metadata = {
  title: "Books List | Clean Architecture Admin",
  description: "View and manage books",
};

export default function BookListPage() {
  return (
    <div>
      <PageBreadcrumb pageTitle="Books" />
      <div className="space-y-6">
        <div className="flex justify-end">
            <Link
                href="/book/edit"
                className="inline-flex items-center justify-center px-4 py-2 text-sm font-medium text-white bg-blue-600 rounded-lg hover:bg-blue-700"
            >
                Add New Book
            </Link>
        </div>
        <ComponentCard title="All Books">
          <BookTable />
        </ComponentCard>
      </div>
    </div>
  );
}
