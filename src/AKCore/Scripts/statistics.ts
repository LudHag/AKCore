import { createApp } from "vue";

import StatisticsApp from "@components/Statistics/StatisticsApp.vue";

if (document.getElementById("statistics-app")) {
  createApp(StatisticsApp).mount("#statistics-app");
}
