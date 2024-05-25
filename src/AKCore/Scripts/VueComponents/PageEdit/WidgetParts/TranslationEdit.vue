<template>
  <div v-if="translate">
    <div class="col-sm-12">
      <h3>Översätt till engelska</h3>
    </div>
    <text-edit
      v-model="modelValue.textEng"
      fullwidth
      title="Translation"
    ></text-edit>
    <div class="col-sm-12">
      <a
        v-if="!fetchingData"
        class="btn btn-default translate-btn"
        href="#"
        @click.prevent="translateText"
      >
        Hämta översättning med chatgpt
      </a>
      <spinner
        class="translate-spinner"
        :size="'medium'"
        v-if="fetchingData"
      ></spinner>
    </div>
  </div>
</template>
<script setup lang="ts">
import { postToApi } from "../../../services/apiservice";
import { WidgetEditModel } from "../models";
import TextEdit from "./TextEdit.vue";
import Spinner from "../../Spinner.vue";
import { ref } from "vue";
const props = defineProps<{
  modelValue: WidgetEditModel;
  translate: boolean;
}>();

const fetchingData = ref(false);

const translateText = () => {
  fetchingData.value = true;
  postToApi(
    "/ExtraInfo/TranslateHtml",
    {
      text: props.modelValue.text
    },
    null,
    null,
    (response) => {
      props.modelValue.textEng = response.data;
      fetchingData.value = false;
    }
  );
};
</script>
<style lang="scss" scoped>
h3 {
  color: #000;
  margin-top: 10px;
}
.translate-btn {
  margin-top: 40px;
  display: block;
  width: max-content;
  margin-left: auto;
  margin-right: auto;
}
.translate-spinner {
  margin-top: 40px;
  display: block;
  margin-left: auto;
  margin-right: auto;
}
</style>
