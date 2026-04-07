import type { Metadata } from "next";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import BookForm from "@/components/book/BookForm";

interface EditBookPageProps {
  params: Promise<{ id: string }>;
}

export const metadata: Metadata = {
  title: "Edit Book | Clean Architecture Admin",
  description: "Edit an existing book",
};

export default async function EditBookPage({ params }: EditBookPageProps) {
  const { id } = await params;

  return (
    <div>
      <PageBreadcrumb pageTitle="Edit Book" />
      <div className="space-y-6">
        <BookForm id={id} />
      </div>
    </div>
  );
}