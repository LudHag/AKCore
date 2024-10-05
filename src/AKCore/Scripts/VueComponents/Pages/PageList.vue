<template>
  <div class="edit-form">
    <div class="form-inline row">
      <div class="form-group">
        <input
          type="text"
          class="form-control"
          placeholder="Sök"
          v-model="searchPhrase"
        />
      </div>
    </div>
    <div class="row top-row">
      <div class="col-xs-3 sort-column" @click="updateSort('name')">Namn</div>
      <div class="col-xs-2 sort-column" @click="updateSort('slug')">
        Sidlänk
      </div>
      <div class="col-xs-2 sort-column" @click="updateSort('loggedin')">
        Kräver inloggning
      </div>
      <div class="col-xs-2 sort-column" @click="updateSort('lastModified')">
        Senast ändrad
      </div>
      <div class="col-xs-3"></div>
    </div>

    <div class="row" v-for="page in sortedFilteredPages" :key="page.id">
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
import { computed, ref } from "vue";
import { PageEditModel, SortType } from "./models";
const props = defineProps<{
  pages: Array<PageEditModel>;
}>();

const searchPhrase = ref("");
const sort = ref<SortType>("name");
const asc = ref(true);

const updateSort = (newSort: SortType) => {
  if (sort.value === newSort) {
    asc.value = !asc.value;
  } else {
    asc.value = true;
  }
  sort.value = newSort;
};

const sortedFilteredPages = computed(() => {
  return props.pages
    .filter((page) => {
      return (
        page.name.toLowerCase().includes(searchPhrase.value.toLowerCase()) ||
        page.slug.toLowerCase().includes(searchPhrase.value.toLowerCase())
      );
    })
    .toSorted((a, b) => {
      const sortA = asc.value ? a : b;
      const sortB = asc.value ? b : a;

      if (sort.value === "name") {
        return sortA.name.localeCompare(sortB.name);
      } else if (sort.value === "slug") {
        return sortA.slug.localeCompare(sortB.slug);
      } else if (sort.value === "loggedin") {
        return Number(sortB.loggedIn) - Number(sortA.loggedIn);
      } else if (sort.value === "lastModified") {
        return (
          new Date(sortB.lastModified).getTime() -
          new Date(sortA.lastModified).getTime()
        );
      }
      return 0;
    });
});

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

.sort-column {
  cursor: pointer;
}
</style>
