import { defineConfig } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue2";

const assetFileNames = (assetInfo) => {
  if (
    assetInfo.name.endsWith("css") &&
    (assetInfo.name.includes("admin") || assetInfo.name.includes("main"))
  ) {
    return "[name].[ext]";
  } else {
    return "vendor.[ext]";
  }
};

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  plugins: [vue()],
  build: {
    outDir: "wwwroot/dist",
    rollupOptions: {
      input: {
        main: resolve(__dirname, "Scripts/main.html"),
        admin: resolve(__dirname, "Scripts/admin.html"),
      },
      output: {
        entryFileNames: `[name].js`,
        chunkFileNames: `vendor.js`,
        assetFileNames: assetFileNames,
      },
    },
  },
});
