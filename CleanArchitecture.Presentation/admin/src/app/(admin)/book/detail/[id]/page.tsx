import type { Metadata } from "next";
import PageBreadcrumb from "@/components/common/PageBreadCrumb";
import PageContent from "./PageContent";

interface BookDetailPageProps {
  params: Promise<{ id: string }>;
}

export const metadata: Metadata = {
  title: "Book Details | Clean Architecture Admin",
  description: "View book details",
};

export default async function BookDetailPage({ params }: BookDetailPageProps) {
  const { id } = await params;

  return (
    <div>
      <PageBreadcrumb pageTitle="Book Details" />
      <div className="space-y-6">
        <PageContent id={id} />
      </div>
    </div>
  );
}