import { writeFile, readFileSync } from "fs";
import { resolve } from "path";
import { Plugin, Manifest } from "vite";

export const entrypoints = ["main", "admin", "statistics"];
type Entrypoint = (typeof entrypoints)[number];

const cleanFilePath = (filePath: string) =>
  filePath.replace("wwwroot/dist/", "");

function getAssetsFromManifest(manifest: Manifest) {
  const assets: Record<
    Entrypoint,
    { entrypoint: string; js: string[]; css: string[] }
  > = {};

  entrypoints.forEach((entrypoint) => {
    const entrypointManifest = manifest[`Scripts/${entrypoint}.ts`];

    const imports =
      entrypointManifest.imports?.map((importName) => manifest[importName]) ||
      [];

    const importJs = imports.map((importFile) =>
      cleanFilePath(importFile.file),
    );
    const importCss = imports
      .flatMap((importFile) => importFile.css || [])
      .concat(entrypointManifest.css || [])
      .map(cleanFilePath);

    assets[entrypoint] = {
      entrypoint: cleanFilePath(entrypointManifest.file),
      js: importJs,
      css: importCss,
    };
  });

  return { assets };
}

export function manifestTransform(): Plugin {
  const manifestPath = "manifest.json";

  return {
    name: "manifest-transform",
    closeBundle() {
      const manifestFullPath = resolve(process.cwd(), manifestPath);
      const manifestContent = readFileSync(manifestFullPath, "utf-8");
      const manifest: Manifest = JSON.parse(manifestContent);

      const modifiedManifest = getAssetsFromManifest(manifest);

      writeFile(
        manifestFullPath,
        JSON.stringify(modifiedManifest, null, 2),
        "utf-8",
        (err) => {
          if (err) throw err;
        },
      );
    },
  };
}
