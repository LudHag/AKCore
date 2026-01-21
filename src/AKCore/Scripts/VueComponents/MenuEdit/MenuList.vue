<template>
  <div>
    <div class="row menualerts">
      <div class="alert alert-danger" ref="error" style="display: none"></div>
      <div
        class="alert alert-success"
        ref="success"
        style="display: none"
      ></div>
    </div>
    <div class="row menu-display">
      <div class="col-sm-12 menu-item" v-for="menu in menus" :key="menu.id">
        <div class="menu-row">
          <a class="btn btn-default menu" @click.prevent="editMenu(menu)">{{
            menu.name
          }}</a>
          <a
            class="btn add-sub-menu glyphicon glyphicon-plus"
            @click.prevent="addSubMenu(menu)"
          ></a>
          <a
            class="btn remove-menu glyphicon glyphicon-remove"
            @click.prevent="deleteMenu(menu)"
          ></a>
          <a
            class="btn move-right glyphicon glyphicon-chevron-down"
            @click.prevent="menuDown(menu)"
          ></a>
          <a
            class="btn move-left glyphicon glyphicon-chevron-up"
            @click.prevent="menuUp(menu)"
          ></a>
        </div>
        <div
          class="menu-row sub-row"
          v-for="child in menu.children"
          :key="child.id"
        >
          <a class="btn submenu" @click.prevent="editSubMenu(child, menu)">{{
            child.name
          }}</a>
          <a
            class="btn remove-sub-menu glyphicon glyphicon-remove"
            @click.prevent="deleteSubMenu(child)"
          ></a>
          <a
            class="btn move-down glyphicon glyphicon-chevron-down"
            @click.prevent="subMenuDown(menu, child)"
          ></a>
          <a
            class="btn move-up glyphicon glyphicon-chevron-up"
            @click.prevent="subMenuUp(menu, child)"
          ></a>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { postToApi } from "@services/apiservice";
import { MenuEditModel } from "./models";

defineProps<{
  menus: MenuEditModel[] | null;
}>();

const error = ref<HTMLElement | null>(null);

const emit = defineEmits<{
  (e: "edit", menu: MenuEditModel | null, parent?: MenuEditModel): void;
  (e: "update"): void;
}>();

const addSubMenu = (menu: MenuEditModel) => {
  emit("edit", null, menu);
};

const editMenu = (menu: MenuEditModel) => {
  emit("edit", menu);
};

const editSubMenu = (child: MenuEditModel, menu: MenuEditModel) => {
  emit("edit", child, menu);
};

const menuUp = (menu: MenuEditModel) => {
  postToApi(
    "/MenuEdit/MoveLeft",
    { id: menu.id },
    error.value as HTMLElement,
    null,
    () => {
      emit("update");
    },
  );
};

const menuDown = (menu: MenuEditModel) => {
  postToApi("/MenuEdit/MoveRight", { id: menu.id }, error.value, null, () => {
    emit("update");
  });
};

const subMenuUp = (menu: MenuEditModel, child: MenuEditModel) => {
  postToApi(
    "/MenuEdit/MoveUp",
    { id: child.id, parent: menu.id },
    error.value,
    null,
    () => {
      emit("update");
    },
  );
};

const subMenuDown = (menu: MenuEditModel, child: MenuEditModel) => {
  postToApi(
    "/MenuEdit/MoveDown",
    { id: child.id, parent: menu.id },
    error.value as HTMLElement,
    null,
    () => {
      emit("update");
    },
  );
};

const deleteMenu = (menu: MenuEditModel) => {
  postToApi(
    "/MenuEdit/RemoveTopMenu",
    { id: menu.id },
    error.value as HTMLElement,
    null,
    () => {
      emit("update");
    },
  );
};

const deleteSubMenu = (menu: MenuEditModel) => {
  postToApi(
    "/MenuEdit/RemoveSubMenu",
    { id: menu.id },
    error.value as HTMLElement,
    null,
    () => {
      emit("update");
    },
  );
};
</script>
<style lang="scss" scoped>
@use "@styles/variables.scss";
.menu-item {
  margin-bottom: 5px;
}

.menu-row {
  display: inline-block;
  width: 100%;
  &.sub-row {
    margin-top: 5px;
    padding-left: 50px;
  }

  .btn {
    padding: 3px 23px;
  }

  &:hover {
    background-color: #1a0000;
  }
}

.submenu {
  background-color: #999999;
  color: white;
  &:focus {
    background-color: variables.$akred;
    border-color: #6e1601;
    color: white;
  }

  &:hover {
    color: #333;
    background-color: variables.$akwhite;
    border-color: #ccc;
  }
}
@media screen and (max-width: variables.$screen-xs-max) {
  .menu-row.sub-row {
    padding-left: 0;
  }
}
</style>
