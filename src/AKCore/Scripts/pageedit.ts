import { createApp } from "vue";
import PageEdit from "@components/PageEdit/PageEdit.vue";
import "vue3-select-component/styles";
import "@styles/adminstyles.scss";

if (document.getElementById("pageedit-app")) {
  createApp(PageEdit).mount("#pageedit-app");
}
