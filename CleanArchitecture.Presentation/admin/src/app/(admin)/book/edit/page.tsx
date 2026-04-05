import { Metadata } from "next";
import React from "react";
import PageContent from "./PageContent";

export const metadata: Metadata = {
  title: "Add Book | Clean Architecture Admin",
  description: "Create a new book",
};

export default function AddBookPage() {
  return (
    <PageContent />
  );
}
