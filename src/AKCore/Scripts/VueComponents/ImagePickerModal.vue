<template>
    <modal :show-modal="showModal" :header="'Välj bild'" @close="close">
        <div slot="body" class="modal-body">
            <div class="row gallery">
              <div class="col-sm-2 image-box" v-for="image in images" :key="image.id">
                  <img :src="'/media/' + image.name" />
                  <p class="name">{{image.name}}</p>
              </div>
            </div>
        </div>
    </modal>
</template>
<script>
import Modal from "./Modal";
import ApiService from "../services/apiservice";

export default {
  components: {
    Modal
  },
  data() {
    return {
      images: []
    };
  },
  props: ["showModal"],
  methods: {
    close() {
      this.$emit("close");
    },
    loadImages() {
      ApiService.get("/Media/ImageListData", null, res => {
        this.images = res;
      });
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
    .gallery .image-box {
      height: 180px;
    }
</style>