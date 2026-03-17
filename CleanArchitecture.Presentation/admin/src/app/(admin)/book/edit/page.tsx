import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookForm from "@/components/book/BookForm";
import { Metadata } from "next";
import React from "react";

export const metadata: Metadata = {
  title: "Add Book | Clean Architecture Admin",
  description: "Create a new book",
};

export default function AddBookPage() {
  return (
    <div>
      <PageBreadcrumb pageTitle="Add New Book" />
      <div className="space-y-6">
        <BookForm />
      </div>
    </div>
  );
}
