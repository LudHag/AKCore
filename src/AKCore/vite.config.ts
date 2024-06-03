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
        if (
          (assetInfo.type === "asset" || assetInfo.type === "chunk") &&
          fileName.endsWith(".js")
        ) {
          assets.push(fileName.split("/")[1]);
        }
      }

      writeFile("./assets.json", JSON.stringify(assets, null, 2), (err) => {
        if (err) throw err;
        console.log("Assets manifest has been created");
      });
    },
  };
}

const assetFileNames = (assetInfo) => {
  if (
    assetInfo.name.endsWith("css") &&
    (assetInfo.name.includes("admin") || assetInfo.name.includes("main"))
  ) {
    return "dist/[name].[ext]";
  } else {
    return "dist/vendor.[ext]";
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
        entryFileNames: `dist/[name].js`,
        chunkFileNames: `dist/vendor.[hash].js`,
        assetFileNames: assetFileNames,
      },
    },
  },
});
