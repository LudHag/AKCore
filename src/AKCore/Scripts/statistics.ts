import { createApp } from "vue";

import StatisticsApp from "./VueComponents/Statistics/StatisticsApp.vue";

if (document.getElementById("statistics-app")) {
  createApp(StatisticsApp).mount("#statistics-app");
}
