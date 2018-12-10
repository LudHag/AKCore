<template>
  <modal :show-modal="showModal" :header="'Redigera meny'" @close="close">
    <div slot="body" class="modal-body">
      <form action="/MenuEdit/EditMenu" method="post" @submit.prevent="submitForm">
        <div class="alert alert-success" style="display: none;"></div>
        <div class="alert alert-danger" style="display: none;"></div>
        <input type="hidden" name="parentId" class="parentId">
        <input type="hidden" name="menuId" class="menuId">
        <div class="form-group">
          <label>Namn</label>
          <input
            type="text"
            class="form-control name"
            name="text"
            placeholder="Namn"
            :value="menuName"
            required
          >
        </div>
        <div class="form-group">
          <select name="pageId" class="form-control" :value="linkId" placeholder="Sida" required>
            <option value>Välj en sida</option>
            <option :value="page.id" v-for="page in pages" :key="page.id">{{page.name}}</option>
          </select>
        </div>
        <div class="checkbox">
          <label>
            <input name="loggedIn" class="logged" v-model="menuLoggedIn" type="checkbox"> Kräv inloggning
          </label>
        </div>
        <div class="checkbox">
          <label>
            <input name="balett" class="balett" type="checkbox" v-model="menuBalett"> Visa enbart för balett
          </label>
        </div>
      </form>
    </div>
    <div slot="footer" class="modal-footer">
      <button type="button" class="btn btn-default" @click.prevent="close">Stäng</button>
      <button type="submit" class="btn btn-primary" @click.prevent="submitForm">Spara</button>
    </div>
  </modal>
</template>
<script>
import ApiService from "../../services/apiservice";
import Modal from "../Modal";

export default {
  props: ["menu", "showModal", "pages"],
  components: {
    Modal
  },
  data() {
    return {
      menuLoggedIn: false,
      menuBalett: false
    };
  },
  computed: {
    menuName() {
      if (this.menu) {
        return this.menu.name;
      } else {
        return "";
      }
    },
    linkId() {
      if (this.menu && this.menu.linkId > 0) {
        return this.menu.linkId;
      } else {
        return "";
      }
    }
  },
  methods: {
    close() {
      this.$emit("close");
    },
    submitForm() {
      console.log("submit");
    },
    updateValues() {
      if (this.menu) {
        this.menuLoggedIn = this.menu.menuLoggedIn;
        this.menuBalett = this.menu.menuBalett;
      }
    }
  },
  watch: {
    menu() {
      this.updateValues();
    }
  },
  created() {
    this.updateValues();
  }
};
</script>
<style lang="scss" scoped>
</style>
