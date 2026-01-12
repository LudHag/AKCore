<template>
  <modal :show-modal="showModal" :header="'Byt lösenord'" @close="close">
    <template #body>
      <div class="modal-body">
        <form
          action="/User/ChangePassword"
          ref="changepassform"
          autocomplete="off"
          method="post"
          @submit.prevent="submitForm"
          v-if="user"
        >
          <div
            class="alert alert-danger change-password-error"
            ref="changepasserror"
            style="display: none"
          ></div>
          <input
            type="hidden"
            class="form-control"
            name="userName"
            :value="user.userName"
            required
          />
          <div class="form-group">
            <label>Lösenord</label>
            <input
              type="password"
              autocomplete="off"
              class="form-control"
              name="password"
              placeholder="Lösenord"
              required
            />
          </div>
          <div class="modal-footer">
            <button type="submit" class="btn btn-primary">
              Skapa nytt lösenord
            </button>
          </div>
        </form>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { defaultFormSend } from "@services/apiservice";
import Modal from "../Modal.vue";
import { User } from "./models";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "success"): void;
}>();

defineProps<{
  user: User | null;
  showModal: boolean;
}>();

const changepassform = ref<HTMLFormElement | null>(null);
const changepasserror = ref<HTMLElement | null>(null);

const close = () => {
  emit("close");
};

const submitForm = () => {
  if (changepassform.value === null) return;
  defaultFormSend(changepassform.value, changepasserror.value, null, () => {
    emit("success");
    close();
  });
};
</script>
<style lang="scss" scoped></style>
