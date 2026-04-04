import { defineConfig } from "orval";

export default defineConfig({
  cleanArchitecture: {
    input: {
      target: "http://localhost:5049/openapi/v1.json",
    },
    output: {
      mode: "tags-split",
      target: "./src/lib/api",
      schemas: "./src/lib/api/model",
      client: "react-query",
      httpClient: "fetch",
      indexFiles: true,
      clean: true,
      prettier: true,
      override: {
        mutator: {
          path: "./src/lib/orval-fetch.ts",
          name: "orvalFetch",
        },
      },
    },
  }
});
