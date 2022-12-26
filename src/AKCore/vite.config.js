import { defineConfig } from 'vite';
import { resolve } from 'path';
import vue from '@vitejs/plugin-vue2';

const assetFileNames = (assetInfo) => {
  if (
    assetInfo.name.endsWith('css') &&
    (assetInfo.name.includes('admin') || assetInfo.name.includes('main'))
  ) {
    return 'assets/[name].[ext]';
  } else {
    return 'assets/vendor.[ext]';
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
    rollupOptions: {
      input: {
        main: resolve(__dirname, 'Scripts/main.html'),
        admin: resolve(__dirname, 'Scripts/admin.html'),
      },
      output: {
        entryFileNames: `assets/[name].js`,
        chunkFileNames: `assets/vendor.js`,
        assetFileNames: assetFileNames,
      },
    },
  },
});
