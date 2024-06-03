import { defineConfig } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import { writeFile } from "fs";
import { Plugin } from "vite";
import { OutputBundle } from "rollup";

function generateAssetList(): Plugin {
  return {
    name: "generate-asset-list",
    generateBundle(_options, bundle: OutputBundle) {
      const assets: string[] = [];

      for (const fileName in bundle) {
        const assetInfo = bundle[fileName];
        if (assetInfo.type === "asset" || assetInfo.type === "chunk") {
          assets.push(fileName.split("/")[1]);
        }
      }

      const assetContainer = {
        mainjs: assets.find(
          (asset) => asset.includes("main") && asset.includes("js"),
        ),
        adminjs: assets.find(
          (asset) => asset.includes("admin") && asset.includes("js"),
        ),
        vendorjs: assets.find(
          (asset) => asset.includes("vendor") && asset.includes("js"),
        ),
        maincss: assets.find(
          (asset) => asset.includes("main") && asset.includes("css"),
        ),
        admincss: assets.find(
          (asset) => asset.includes("admin") && asset.includes("css"),
        ),
        vendorcss: assets.find(
          (asset) => asset.includes("vendor") && asset.includes("css"),
        ),
      };

      writeFile(
        "./assets.json",
        JSON.stringify(assetContainer, null, 2),
        (err) => {
          if (err) throw err;
          console.log("Assets manifest has been created");
        },
      );
    },
  };
}

const assetFileNames = (assetInfo) => {
  if (
    assetInfo.name.endsWith("css") &&
    (assetInfo.name.includes("admin") || assetInfo.name.includes("main"))
  ) {
    return "dist/[name].[hash].[ext]";
  } else {
    return "dist/vendor.[hash].[ext]";
  }
};

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  plugins: [vue(), generateAssetList()],
  build: {
    emptyOutDir: false,
    outDir: "wwwroot",
    rollupOptions: {
      input: {
        main: resolve(__dirname, "Scripts/main.html"),
        admin: resolve(__dirname, "Scripts/admin.html"),
      },
      output: {
        entryFileNames: `dist/[name].[hash].js`,
        chunkFileNames: `dist/vendor.[hash].js`,
        assetFileNames: assetFileNames,
      },
    },
  },
});
