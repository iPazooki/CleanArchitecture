import type { Metadata } from "next";
import PageContent from "../PageContent";

interface EditBookPageProps {
  params: Promise<{ id: string }>;
}

export const metadata: Metadata = {
  title: "Edit Book | Clean Architecture Admin",
  description: "Edit an existing book",
};

export default async function EditBookPage({ params }: EditBookPageProps) {
  const { id } = await params;

  return <PageContent id={id} />;
}
