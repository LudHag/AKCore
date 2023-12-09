<template>
  <div id="menu-edit-app">
    <div class="row">
      <div class="col-md-6">
        <button
          type="button"
          class="btn btn-primary"
          @click.prevent="toggleCreateMenu"
        >
          Lägg till en toppmeny
        </button>
      </div>
    </div>
    <div class="row edit-form" v-if="showAddMenu">
      <div class="col-md-6">
        <form
          action="/MenuEdit/AddTopMenu"
          @submit.prevent="createMenu"
          method="post"
        >
          <div class="alert alert-danger" style="display: none"></div>
          <div class="alert alert-success" style="display: none">
            Meny sparad
          </div>
          <div class="form-group">
            <label for="name">Menytext:</label>
            <input
              type="text"
              class="form-control"
              name="name"
              placeholder="Menytext"
            />
          </div>
          <div class="form-group">
            <label for="slug">Menylänk:</label>
            <select name="pageId" class="form-control">
              <option>Välj en sida</option>
              <option :value="page.id" v-for="page in pages" :key="page.id">
                {{ page.name }}
              </option>
            </select>
          </div>
          <div class="checkbox">
            <label>
              <input name="loggedIn" type="checkbox" /> Kräv inloggning
            </label>
          </div>
          <div class="checkbox">
            <label>
              <input name="balett" class="balett" type="checkbox" /> Visa enbart
              för balett
            </label>
          </div>
          <button type="submit" class="btn btn-default">Skapa</button>
        </form>
      </div>
    </div>
    <menu-list :menus="menus" @update="loadMenus" @edit="editMenu"></menu-list>
    <menu-edit-modal
      :show-modal="showEditModal"
      :pages="pages"
      :parent-id="parentId"
      :menu="editedMenu"
      @update="loadMenus"
      @close="closeEditMenu"
    ></menu-edit-modal>
  </div>
</template>
<script setup lang="ts">
import { defaultFormSend, getFromApi } from "../../services/apiservice";
import MenuList from "./MenuList.vue";
import MenuEditModal from "./MenuEditModal.vue";
import { MenuEditModel, PageEditModel } from "./models";
import { onMounted, ref } from "vue";

const menus = ref<MenuEditModel[] | null>(null);
const showAddMenu = ref(false);
const pages = ref<PageEditModel[] | null>(null);
const showEditModal = ref(false);
const editedMenu = ref<MenuEditModel | null>(null);
const parentId = ref<number | null>(null);

const editMenu = (menu: MenuEditModel | null, parent?: MenuEditModel) => {
  showEditModal.value = true;
  editedMenu.value = menu;
  if (parent) {
    parentId.value = parent.id;
  }
};

const loadMenus = async () => {
  const response = await getFromApi<any>("/MenuEdit/MenuListData");
  menus.value = response.menus;
  pages.value = response.pages;
};

const toggleCreateMenu = () => {
  showAddMenu.value = !showAddMenu.value;
};

const createMenu = (event: Event) => {
  const form = event.target as HTMLFormElement;
  defaultFormSend(form, null, null, () => {
    loadMenus();
    showAddMenu.value = false;
    form.reset();
  });
};

const closeEditMenu = () => {
  showEditModal.value = false;
  editedMenu.value = null;
  parentId.value = null;
};

onMounted(() => {
  loadMenus();
});
</script>
<style lang="scss" scoped>
.edit-form {
  padding-top: 10px;
}

.menualerts {
  margin-top: 20px;
}
</style>
