<template>
  <Modal
    :show-modal="showModal ?? false"
    :header="'Välj bild'"
    :notransition="notransition ?? false"
    @close="close"
  >
    <template v-slot:body>
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
                v-for="imageType in imageTypes"
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
            :key="image.id"
            @click.prevent="selectImage(image)"
          >
            <img :src="'/media/' + image.name" />
            <p class="name">{{ image.name }}</p>
          </div>
          <div class="col-xs-12">
            <ul class="pagination">
              <li
                v-bind:class="{ active: page + 1 === n }"
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
import ApiService from "../services/apiservice";
import Constants from "../constants";
import { Image } from "./models";
import { ref, computed, onMounted } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "image", image: any): void;
}>();

const { destination } = defineProps<{
  showModal: boolean | null;
  notransition?: boolean | null;
  destination?: JQuery<HTMLElement> | null;
}>();

const images = ref<Image[]>([]);
const page = ref(0);
const type = ref("");
const search = ref("");

const close = () => {
  emit("close");
};

const loadImages = () => {
  ApiService.get("/Media/ImageListData", null, (res: Image[]) => {
    images.value = res;
  });
};

const selectImage = (image: Image) => {
  if (destination) {
    destination.val("/media/" + image.name);
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
  if (page.value + 1 > nbrPages && nbrPages - 1 > -1) {
    page.value = nbrPages - 1;
  }
  return nbrPages;
});

const imageTypes = computed(() => {
  return Constants.IMAGETYPES;
});

onMounted(() => {
  loadImages();
});
</script>

<style lang="scss" scoped>
.image-box {
  cursor: pointer;
}
</style>
