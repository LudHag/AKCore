import { existsSync } from "node:fs";
import { dirname, resolve } from "node:path";
import { fileURLToPath } from "node:url";
import { describe, expect, it } from "vitest";
import { WIDGET_TYPES } from "@components/PageEdit/models";

const widgetsDir = resolve(
  dirname(fileURLToPath(import.meta.url)),
  "../../Views/Page/Widgets",
);

describe("CMS widget templates", () => {
  it.each(WIDGET_TYPES)("has a Razor partial for type %s", (type) => {
    const templatePath = resolve(widgetsDir, `${type}.cshtml`);
    expect(
      existsSync(templatePath),
      `Missing Views/Page/Widgets/${type}.cshtml`,
    ).toBe(true);
  });
});
