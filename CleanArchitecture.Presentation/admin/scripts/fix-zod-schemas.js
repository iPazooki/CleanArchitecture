/**
 * Post-generation fix for Orval Zod schemas.
 * Orval v8.5.1 generates `stringFormat()` calls that don't exist in Zod v4.
 * This script:
 *  1. Replaces standalone `zod.stringFormat('int32'|'double', ...)` with `zod.number()`
 *  2. Replaces other standalone `zod.stringFormat(...)` with `zod.string()`
 *  3. Removes `.stringFormat(...)` chained on zod.number() etc.
 */
const fs = require("fs");
const path = require("path");

function walkDir(dir, callback) {
  fs.readdirSync(dir).forEach((f) => {
    let dirPath = path.join(dir, f);
    let isDirectory = fs.statSync(dirPath).isDirectory();
    isDirectory ? walkDir(dirPath, callback) : callback(path.join(dir, f));
  });
}

walkDir("./src/lib/api/zod", (f) => {
  if (!f.endsWith(".ts")) return;
  let c = fs.readFileSync(f, "utf8");

  // 1. Replace standalone zod.stringFormat('int32'|'double', ...) with zod.number()
  c = c.replace(
    /zod\.stringFormat\('(?:int32|double)'(?:,\s*[^)]+)?\)/g,
    "zod.number()"
  );

  // 2. Replace any remaining standalone zod.stringFormat(...) with zod.string()
  c = c.replace(/zod\.stringFormat\([^)]*\)/g, "zod.string()");

  // 3. Remove .stringFormat(...) chained on something (e.g. zod.number().stringFormat('int32', var))
  c = c.replace(/\.stringFormat\('[^']*'(?:,\s*[^)]+)?\)/g, "");

  fs.writeFileSync(f, c);
});
