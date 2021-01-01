<template>
  <modal
    :show-modal="showModal"
    :header="'Välj Dokument'"
    :notransition="notransition"
    @close="close"
  >
    <div slot="body" class="modal-body">
      <form class="form-inline ak-search">
        <div class="form-group">
          <input type="text" class="form-control" v-model="search" />
        </div>
      </form>
      <div class="row gallery">
        <div
          class="col-sm-3 image-box"
          v-for="doc in shownDocuments"
          :key="doc.id"
          @click.prevent="selectDocument(doc)"
        >
          <img :src="'/media/' + doc.name" />
          <p class="name">{{ doc.name }}</p>
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

export default {
  components: {
    Modal
  },
  data() {
    return {
      documents: [],
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
    loadDocuments() {
      ApiService.get("/Media/DocumentListData", null, res => {
        this.documents = res;
      });
    },
    selectDocument(document) {
      if (this.destination) {
        this.destination.val("/media/" + document.name);
        this.$emit("close");
      } else {
        this.$emit("document", document);
      }
    },
    toPage(n) {
      this.page = n;
    }
  },
  computed: {
    filteredDocuments() {
      return this.documents.filter(document => {
        return (
          (!this.type || this.type === document.tag) &&
          (!this.search ||
            document.name.toLowerCase().indexOf(this.search.toLowerCase()) > -1)
        );
      });
    },
    shownDocuments() {
      const take = this.page * 8;
      return this.filteredDocuments.slice(take, take + 8);
    },
    pagesLength() {
      const nbrPages = Math.ceil(this.filteredDocuments.length / 8);
      if (this.page + 1 > nbrPages && nbrPages - 1 > -1) {
        this.page = nbrPages - 1;
      }
      return nbrPages;
    }
  },
  created() {
    this.loadDocuments();
  }
};
</script>
<style lang="scss" scoped>
.image-box {
  cursor: pointer;
}
</style>
