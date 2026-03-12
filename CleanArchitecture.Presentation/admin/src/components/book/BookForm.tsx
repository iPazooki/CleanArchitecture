"use client";
import React, { useEffect, useState } from "react";
import { useRouter } from "next/navigation";
import { postCreateBook, putUpdateBook, getGetBookId } from "@/lib/api/book-endpoints/book-endpoints";
import Label from "../form/Label";
import Input from "../form/input/InputField";
import Button from "../ui/button/Button";
import ComponentCard from "../common/ComponentCard";

interface BookFormProps {
  id?: string;
}

export default function BookForm({ id }: BookFormProps) {
  const router = useRouter();
  const [title, setTitle] = useState("");
  const [genre, setGenre] = useState("");
  const [loading, setLoading] = useState(false);
  const [fetching, setFetching] = useState(!!id);

  useEffect(() => {
    if (id) {
      const fetchBook = async () => {
        try {
          const response = await getGetBookId(id);
          if (response.status === 200 && response.data.isSuccess) {
            const book = response.data.value!;
            setTitle(book.title || "");
            setGenre(book.genre || "");
          }
        } catch (error) {
          console.error("Failed to fetch book for editing", error);
        } finally {
          setFetching(false);
        }
      };
      fetchBook();
    }
  }, [id]);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      if (id) {
        // Update
        const response = await putUpdateBook({ id, title, genre });
        if (response.status === 204) {
          router.push("/book/list");
        } else {
            // Handle error (ProblemDetails might be in response.data)
            console.error("Update failed", response.data);
        }
      } else {
        // Create
        const response = await postCreateBook({ title, genre });
        if (response.status === 201) {
          router.push("/book/list");
        } else {
            console.error("Create failed", response.data);
        }
      }
    } catch (error) {
      console.error("Form submission failed", error);
    } finally {
      setLoading(false);
    }
  };

  if (fetching) return <div>Loading book data...</div>;

  return (
    <ComponentCard title={id ? "Edit Book" : "Add New Book"}>
      <form onSubmit={handleSubmit} className="space-y-6">
        <div>
          <Label htmlFor="title">Title</Label>
          <Input
            id="title"
            type="text"
            placeholder="Book Title"
            defaultValue={title}
            onChange={(e) => setTitle(e.target.value)}
          />
        </div>
        <div>
          <Label htmlFor="genre">Genre</Label>
          <Input
            id="genre"
            type="text"
            placeholder="Book Genre"
            defaultValue={genre}
            onChange={(e) => setGenre(e.target.value)}
          />
        </div>
        <div className="flex items-center gap-3">
          <Button disabled={loading}>
            {loading ? "Saving..." : id ? "Update Book" : "Create Book"}
          </Button>
          <Button
            variant="outline"
            onClick={() => router.back()}
            disabled={loading}
          >
            Cancel
          </Button>
        </div>
      </form>
    </ComponentCard>
  );
}
