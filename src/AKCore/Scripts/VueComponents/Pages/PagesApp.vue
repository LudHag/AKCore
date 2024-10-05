<template>
  <page-add-form />
  <page-list :pages="pages" />
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { getFromApi } from "../../services/apiservice";
import { PageEditModel, PagesRequesponse } from "./models";
import PageList from "./PageList.vue";
import PageAddForm from "./PageAddForm.vue";

const pages = ref<Array<PageEditModel>>([]);

onMounted(() => {
  getFromApi<PagesRequesponse>(window.location.href + "/model").then((res) => {
    pages.value = res.pages;
  });
});
</script>
<style lang="scss" scoped></style>
