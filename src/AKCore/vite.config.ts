import { defineConfig } from "vite";
import { resolve } from "path";
import vue from "@vitejs/plugin-vue";
import { visualizer } from "rollup-plugin-visualizer";
import { entrypoints, manifestTransform } from "./vite.utils";

// https://vitejs.dev/config/
export default defineConfig({
  server: {
    port: 5173,
    hmr: { clientPort: 5173 },
  },
  resolve: {
    alias: {
      "@styles": resolve(__dirname, "Styles"),
      "@scripts": resolve(__dirname, "Scripts"),
      "@components": resolve(__dirname, "Scripts/VueComponents"),
      "@utils": resolve(__dirname, "Scripts/utils"),
      "@translations": resolve(__dirname, "Scripts/translations"),
      "@services": resolve(__dirname, "Scripts/services"),
    },
  },
  plugins: [
    vue(),
    manifestTransform(),
    ...(process.env.ANALYZE
      ? [
          visualizer({
            open: true,
          }),
        ]
      : []),
  ],
  experimental: {
    renderBuiltUrl: (filename) => {
      return filename.replace("wwwroot", "");
    },
  },
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
