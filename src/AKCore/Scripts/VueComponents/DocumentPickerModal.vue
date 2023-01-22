<template>
  <modal
    :show-modal="showModal ?? false"
    :header="'Välj Dokument'"
    :notransition="notransition ?? false"
    @close="close"
  >
    <template #body>
      <div class="modal-body">
        <form class="form-inline ak-search" @submit.prevent>
          <div class="form-group">
            <input type="text" class="form-control" v-model="search" />
          </div>
        </form>
        <div class="gallery document-list">
          <a
            class="document-item"
            v-for="doc in shownDocuments"
            :key="doc.id"
            @click.prevent="selectDocument(doc)"
          >
            {{ doc.name }}
          </a>
          <ul class="pagination">
            <li
              :class="{ active: page + 1 === n }"
              v-for="n in pagesLength"
              :key="n"
            >
              <a href="#" @click.prevent="toPage(n - 1)">{{ n }}</a>
            </li>
          </ul>
        </div>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import Modal from "./Modal.vue";
import { getFromApi } from "../services/apiservice";
import { Document } from "./models";
import { ref, computed, onMounted, watch } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "document", image: Document): void;
}>();

const props = defineProps<{
  showModal: boolean;
  notransition?: boolean;
  destination?: JQuery<HTMLElement> | null;
}>();

const documents = ref<Document[]>([]);
const page = ref(0);
const type = ref("");
const search = ref("");

const close = () => emit("close");

const loadDocuments = async () => {
  documents.value = await getFromApi<Document[]>("/Media/DocumentListData");
};

const selectDocument = (document: Document) => {
  if (props.destination) {
    props.destination.val("/media/" + document.name);
    emit("close");
  } else {
    emit("document", document);
  }
};

const toPage = (n: number) => {
  page.value = n;
};

const filteredDocuments = computed(() =>
  documents.value.filter((document) => {
    return (
      (!type.value || type.value === document.tag) &&
      (!search.value ||
        document.name.toLowerCase().indexOf(search.value.toLowerCase()) > -1)
    );
  })
);

const shownDocuments = computed(() => {
  const take = page.value * 16;
  return filteredDocuments.value.slice(take, take + 16);
});

const pagesLength = computed(() => {
  const nbrPages = Math.ceil(filteredDocuments.value.length / 16);

  return nbrPages;
});

watch(
  () => filteredDocuments.value,
  () => {
    if (page.value + 1 > pagesLength.value && pagesLength.value - 1 > -1) {
      page.value = pagesLength.value - 1;
    }
  }
);

onMounted(() => {
  loadDocuments();
});
</script>
<style lang="scss" scoped>
.document-item {
  cursor: pointer;
  display: block;
}
</style>
