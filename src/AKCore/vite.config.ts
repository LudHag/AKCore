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
          // Get just the filename without any path
          assets.push(fileName.split("/").pop() || fileName);
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

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  plugins: [vue(), generateAssetList()],
  build: {
    emptyOutDir: false,
    outDir: "wwwroot/dist",
    rollupOptions: {
      input: {
        main: resolve(__dirname, "Scripts/main.ts"),
        admin: resolve(__dirname, "Scripts/admin.ts"),
      },
      output: {
        assetFileNames: "[name].[ext]",
        chunkFileNames: "[name]-[hash].js",
        entryFileNames: "[name]-[hash].js",
      },
    },
  },
});
