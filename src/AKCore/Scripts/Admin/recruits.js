import { createApp } from "vue";
import RecruitsApp from "../VueComponents/Recruits/RecruitsApp.vue";

if ($("#recruits-app").length > 0) {
  createApp(RecruitsApp).mount("#recruits-app");
}
