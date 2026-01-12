import { createApp } from "vue";
import "vue3-select-component/styles";

import ProfileApp from "@components/Profile/ProfileApp.vue";

if (document.getElementById("profile-app")) {
  createApp(ProfileApp).mount("#profile-app");
}
