<template>
  <modal :show-modal="showModal" :header="'Redigera meny'" @close="close">
    <template v-slot:body>
      <div class="modal-body">
        <form
          :action="formEndpoint"
          method="post"
          @submit.prevent="submitForm"
          ref="editform"
        >
          <div class="alert alert-danger" style="display: none"></div>
          <input
            type="hidden"
            name="parentId"
            class="parentId"
            :value="parentId"
          />
          <input type="hidden" name="menuId" class="menuId" :value="menuId" />
          <div class="form-group">
            <label>Namn</label>
            <input
              type="text"
              class="form-control name"
              name="text"
              placeholder="Namn"
              :value="menuName"
              required
            />
          </div>
          <div class="form-group">
            <select
              name="pageId"
              class="form-control"
              :value="linkId"
              placeholder="Sida"
            >
              <option value>Välj en sida</option>
              <option :value="page.id" v-for="page in pages" :key="page.id">
                {{ page.name }}
              </option>
            </select>
          </div>
          <div class="checkbox" v-if="!parentId">
            <label>
              <input
                name="loggedIn"
                class="logged"
                :checked="menuLoggedIn"
                type="checkbox"
              />
              Kräv inloggning
            </label>
          </div>
          <div class="checkbox" v-if="!parentId">
            <label>
              <input
                name="balett"
                class="balett"
                type="checkbox"
                :checked="menuBalett"
              />
              Visa enbart för balett
            </label>
          </div>
        </form>
      </div>
    </template>
    <template v-slot:footer>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" @click.prevent="close">
          Stäng
        </button>
        <button
          type="submit"
          class="btn btn-primary"
          @click.prevent="submitForm"
        >
          Spara
        </button>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { ref, computed, watch, onMounted } from "vue";
import ApiService from "../../services/apiservice";
import Modal from "../Modal.vue";
import { MenuEditModel, PageEditModel } from "./models";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "update"): void;
}>();

const props = defineProps<{
  menu: MenuEditModel | null;
  parentId: number | null;
  showModal: boolean;
  pages: PageEditModel[] | null;
}>();

const menuLoggedIn = ref(false);
const menuBalett = ref(false);
const editform = ref<HTMLFormElement | null>(null);

const menuName = computed(() => {
  if (props.menu) {
    return props.menu.name;
  } else {
    return "";
  }
});

const linkId = computed(() => {
  if (props.menu && props.menu.linkId > 0) {
    return props.menu.linkId;
  } else {
    return "";
  }
});

const menuId = computed(() => {
  if (props.menu && props.menu.id > 0) {
    return props.menu.id;
  } else {
    return "";
  }
});

const formEndpoint = computed(() => {
  return props.menu ? "/MenuEdit/EditMenu" : "/MenuEdit/AddSubMenu";
});

const close = () => {
  emit("close");
};

const submitForm = () => {
  if (!editform.value) return;
  const error = $(".alert-danger");

  const form = $(editform.value as HTMLFormElement) as JQuery<HTMLFormElement>;
  ApiService.defaultFormSend(form, error, null, () => {
    emit("update");
    close();
  });
};

const updateValues = () => {
  if (props.menu) {
    menuLoggedIn.value = props.menu.menuLoggedIn;
    menuBalett.value = props.menu.menuBalett;
  }
};

watch(
  () => props.menu,
  () => {
    updateValues();
  }
);

onMounted(() => {
  updateValues();
});
</script>
<style lang="scss" scoped></style>
