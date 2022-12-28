import { defineConfig } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";

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
  plugins: [vue()],
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
        chunkFileNames: `dist/vendor.js`,
        assetFileNames: assetFileNames,
      },
    },
  },
});
