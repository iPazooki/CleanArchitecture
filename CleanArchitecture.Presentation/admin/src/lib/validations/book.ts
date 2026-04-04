import { z } from "zod";

export const bookSchema = z.object({
  title: z
    .string()
    .min(1, { message: "Title is required." })
    .min(3, { message: "Title must be at least 3 characters long." })
    .max(200, { message: "Title cannot exceed 200 characters." }),
  genre: z
    .string()
    .min(1, { message: "Genre is required." })
    .max(50, { message: "Genre cannot exceed 50 characters." }),
});

export type BookFormValues = z.infer<typeof bookSchema>;
