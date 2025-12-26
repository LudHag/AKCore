import { defineConfig } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import { entrypoints, manifestTransform } from "./vite.utils";

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  plugins: [vue(), manifestTransform()],
  build: {
    manifest: "manifest.json",
    outDir: ".",
    emptyOutDir: false,
    rollupOptions: {
      input: entrypoints.reduce(
        (acc, entrypoint) => {
          acc[entrypoint] = resolve(__dirname, `Scripts/${entrypoint}.ts`);
          return acc;
        },
        {} as Record<string, string>,
      ),
      output: {
        assetFileNames: "wwwroot/dist/[name]-[hash].[ext]",
        chunkFileNames: "wwwroot/dist/[name]-[hash].js",
        entryFileNames: "wwwroot/dist/[name]-[hash].js",
      },
    },
  },
});
