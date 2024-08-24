<template>
  <Modal
    :show-modal="showModal ?? false"
    :header="'Välj bild'"
    :notransition="notransition ?? false"
    @close="close"
  >
    <template #body>
      <div class="modal-body">
        <form class="form-inline ak-search">
          <div class="form-group">
            <input type="text" class="form-control" v-model="search" />
          </div>
          <div class="form-group">
            <select name="Tag" v-model="type" class="form-control">
              <option value="">Alla bilder</option>
              <option
                :value="imageType"
                v-for="imageType in IMAGETYPES"
                :key="imageType"
              >
                {{ imageType }}
              </option>
            </select>
          </div>
        </form>
        <div class="row gallery">
          <div
            class="col-sm-3 image-box"
            v-for="image in shownImages"
            @click.prevent="selectImage(image)"
          >
            <img :src="'/media/' + image.name" />
            <p class="name">{{ image.name }}</p>
          </div>
          <div class="col-xs-12">
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
      </div>
    </template>
  </Modal>
</template>

<script setup lang="ts">
import Modal from "./Modal.vue";
import { getFromApi } from "../services/apiservice";
import { IMAGETYPES } from "../constants";
import { Image } from "./models";
import { ref, computed, onMounted, watch } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "image", image: Image): void;
}>();

const props = defineProps<{
  showModal: boolean | null;
  notransition?: boolean | null;
  destination?: HTMLInputElement | null;
}>();

const images = ref<Image[]>([]);
const page = ref(0);
const type = ref("");
const search = ref("");

const close = () => {
  emit("close");
};

const loadImages = async () => {
  images.value = await getFromApi<Image[]>("/Media/ImageListData");
};

const selectImage = (image: Image) => {
  if (props.destination) {
    props.destination.value = "/media/" + image.name;
    emit("close");
  } else {
    emit("image", image);
  }
};

const toPage = (n: number) => {
  page.value = n;
};

const filteredImages = computed(() => {
  return images.value.filter((image) => {
    return (
      (!type.value || type.value === image.tag) &&
      (!search.value ||
        image.name.toLowerCase().indexOf(search.value.toLowerCase()) > -1)
    );
  });
});

const shownImages = computed(() => {
  const take = page.value * 8;
  return filteredImages.value.slice(take, take + 8);
});

const pagesLength = computed(() => {
  const nbrPages = Math.ceil(filteredImages.value.length / 8);
  return nbrPages;
});

watch(
  () => filteredImages.value,
  () => {
    if (page.value + 1 > pagesLength.value && pagesLength.value - 1 > -1) {
      page.value = pagesLength.value - 1;
    }
  },
);

onMounted(() => {
  loadImages();
});
</script>

<style lang="scss" scoped>
.image-box {
  cursor: pointer;
}
</style>
