import { z } from "zod";
import type { TranslationKey } from "@/i18n";
import { genreValues } from "@/lib/books/genre";

/** Messages are translation keys, resolved by the form. Typed so a typo fails the build. */
const message = (key: TranslationKey): TranslationKey => key;

export const bookSchema = z.object({
  title: z
    .string()
    .min(1, { message: message("title_required") })
    .min(3, { message: message("title_min_length") })
    .max(200, { message: message("title_max_length") }),
  genre: z.enum(genreValues, {
    error: (issue) => (issue.input === "" ? message("genre_required") : message("genre_invalid")),
  }),
});

export type BookFormValues = z.infer<typeof bookSchema>;
