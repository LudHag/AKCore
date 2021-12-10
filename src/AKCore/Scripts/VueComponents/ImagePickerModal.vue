<template>
  <modal
    :show-modal="showModal"
    :header="'Välj bild'"
    :notransition="notransition"
    @close="close"
  >
    <div slot="body" class="modal-body">
      <form class="form-inline ak-search">
        <div class="form-group">
          <input type="text" class="form-control" v-model="search" />
        </div>
        <div class="form-group">
          <select name="Tag" v-model="type" class="form-control">
            <option value="">Alla bilder</option>
            <option :value="type" v-for="type in imageTypes" :key="type">{{
              type
            }}</option>
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
  </modal>
</template>
<script>
import Modal from "./Modal";
import ApiService from "../services/apiservice";
import Constants from "../constants";

export default {
  components: {
    Modal
  },
  data() {
    return {
      images: [],
      page: 0,
      type: "",
      search: ""
    };
  },
  props: ["showModal", "notransition", "destination"],
  methods: {
    close() {
      this.$emit("close");
    },
    loadImages() {
      ApiService.get("/Media/ImageListData", null, res => {
        this.images = res;
      });
    },
    selectImage(image) {
      if (this.destination) {
        this.destination.val("/media/" + image.name);
        this.$emit("close");
      } else {
        this.$emit("image", image);
      }
    },
    toPage(n) {
      this.page = n;
    }
  },
  computed: {
    filteredImages() {
      return this.images.filter(image => {
        return (
          (!this.type || this.type === image.tag) &&
          (!this.search ||
            image.name.toLowerCase().indexOf(this.search.toLowerCase()) > -1)
        );
      });
    },
    shownImages() {
      const take = this.page * 8;
      return this.filteredImages.slice(take, take + 8);
    },
    pagesLength() {
      const nbrPages = Math.ceil(this.filteredImages.length / 8);
      if (this.page + 1 > nbrPages && nbrPages - 1 > -1) {
        this.page = nbrPages - 1;
      }
      return nbrPages;
    },
    imageTypes() {
      return Constants.IMAGETYPES;
    }
  },
  created() {
    this.loadImages();
  }
};
</script>
<style lang="scss" scoped>
.image-box {
  cursor: pointer;
}
</style>
