import { z } from "zod";

export const bookSchema = z.object({
  title: z
    .string()
    .min(1, { message: "title_required" })
    .min(3, { message: "title_min_length" })
    .max(200, { message: "title_max_length" }),
  genre: z
    .string()
    .min(1, { message: "genre_required" })
    .max(50, { message: "genre_max_length" }),
});

export type BookFormValues = z.infer<typeof bookSchema>;
