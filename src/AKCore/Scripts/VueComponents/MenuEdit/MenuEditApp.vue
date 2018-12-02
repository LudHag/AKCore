<template>
  <div id="menu-edit-app">
    <div class="row">
      <div class="col-md-6">
        <button
          type="button"
          class="btn btn-primary"
          @click.prevent="toggleCreateMenu"
        >Lägg till en toppmeny</button>
      </div>
    </div>
    <div class="row edit-form" v-if="showAddMenu">
      <div class="col-md-6">
        <form action="/MenuEdit/AddTopMenu" @submit.prevent="createMenu" method="post">
          <div class="alert alert-danger" style="display: none;"></div>
          <div class="alert alert-success" style="display: none;">Meny sparad</div>
          <div class="form-group">
            <label for="name">Menytext:</label>
            <input type="text" class="form-control" name="name" placeholder="Menytext">
          </div>
          <div class="form-group">
            <label for="slug">Menylänk:</label>
            <select name="pageId" class="form-control chosen-select">
              <option>Välj en sida</option>
              <option :value="page.id" v-for="page in pages" :key="page.id">{{page.name}}</option>
            </select>
          </div>
          <div class="checkbox">
            <label>
              <input name="loggedIn" type="checkbox"> Kräv inloggning
            </label>
          </div>
          <div class="checkbox">
            <label>
              <input name="balett" class="balett" type="checkbox"> Visa enbart för balett
            </label>
          </div>
          <button type="submit" class="btn btn-default">Skapa</button>
        </form>
      </div>
    </div>
    <menu-list :menus="menus" @update="loadMenus"></menu-list>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import MenuList from "./MenuList";

export default {
  components: {
    MenuList
  },
  data() {
    return {
      menus: null,
      showAddMenu: false,
      pages: null
    };
  },
  methods: {
    loadMenus() {
      ApiService.get("/MenuEdit/MenuListData", null, res => {
        this.menus = res.menus;
        this.pages = res.pages;
      });
    },
    toggleCreateMenu() {
      this.showAddMenu = !this.showAddMenu;
    },
    createMenu(event) {
      const form = $(event.target);
      ApiService.defaultFormSend(form, null, null, () => {
        this.loadMenus();
        this.showAddMenu = false;
        form.trigger("reset");
      });
    }
  },
  created() {
    this.loadMenus();
  }
};
</script>
<style lang="scss" scoped>
.edit-form {
  padding-top: 10px;
}

.menualerts {
  margin-top: 20px;
}
</style>
