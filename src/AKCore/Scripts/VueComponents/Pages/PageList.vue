<template>
  <div class="edit-form">
    <div class="row top-row">
      <div class="col-xs-3">Namn</div>
      <div class="col-xs-2">Sidlänk</div>
      <div class="col-xs-2">Kräver inloggning</div>
      <div class="col-xs-2">Senast ändrad</div>
      <div class="col-xs-3"></div>
    </div>

    <div class="row" v-for="page in pages" :key="page.id">
      <div class="col-xs-3">{{ page.name }}</div>
      <div class="col-xs-2">
        <a :href="page.slug">{{ page.slug }}</a>
      </div>
      <div class="col-xs-2">{{ page.loggedIn || page.balettOnly }}</div>
      <div class="col-xs-2">{{ formatDate(page.lastModified) }}</div>
      <div class="col-xs-3">
        <a class="btn btn-default" :href="`/Edit/Page/${page.id}`">Ändra</a>
        <a
          class="btn btn-default"
          :href="`/Edit/RemovePage/${page.id}`"
          @click.prevent="removePage(page.id)"
        >
          Ta bort</a
        >
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { PageEditModel } from "./models";

defineProps<{
  pages: Array<PageEditModel>;
}>();

const formatDate = (dateString: string) => {
  const date = new Date(dateString);
  return date.toISOString().split("T")[0];
};

const removePage = (id: number) => {
  if (confirm("Är du säker på att du vill ta bort sidan?")) {
    window.location.href = `/Edit/RemovePage/${id}`;
  }
};
</script>
<style lang="scss" scoped>
.top-row {
  font-weight: 500;
  border-bottom: 2px solid #1a1a1a;
}

.edit-form {
  padding-top: 35px;
}

.btn-default {
  margin-right: 3px;
}
</style>
