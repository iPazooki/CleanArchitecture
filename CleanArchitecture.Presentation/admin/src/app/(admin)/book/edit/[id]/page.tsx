"use client";
import React from "react";
import { useParams } from "next/navigation";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookForm from "@/components/book/BookForm";

export default function EditBookPage() {
  const { id } = useParams();

  return (
    <div>
      <PageBreadcrumb pageTitle="Edit Book" />
      <div className="space-y-6">
        <BookForm id={id as string} />
      </div>
    </div>
  );
}
