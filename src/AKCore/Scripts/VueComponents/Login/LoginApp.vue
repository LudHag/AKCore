<template>
  <modal :show-modal="showModal" :header="'Logga in'" @close="close">
    <template #body>
      <form
        action="/Account/Login"
        id="loginForm"
        @submit.prevent="submitForm"
        method="post"
      >
        <div class="modal-body">
          <div class="alert alert-success" style="display: none"></div>
          <div class="alert alert-danger" style="display: none"></div>
          <div class="form-group">
            <label for="username">Användarnamn</label>
            <input
              type="text"
              class="form-control"
              id="username"
              name="Username"
              placeholder="Användarnamn"
            />
          </div>
          <div class="form-group">
            <label for="password">Lösenord</label>
            <input
              type="password"
              class="form-control"
              id="password"
              name="Password"
              placeholder="Lösenord"
            />
          </div>
        </div>
        <div class="modal-footer">
          <button type="button" class="btn btn-default" @click="close">
            Stäng
          </button>
          <button type="submit" class="btn btn-primary submit-login">
            Logga in
          </button>
        </div>
      </form>
    </template>
  </modal>
</template>
<script setup lang="ts">
import Modal from "../Modal.vue";
import { defaultFormSend } from "../../services/apiservice";
import { ref, onMounted } from "vue";

const showModal = ref(false);

const close = () => {
  showModal.value = false;
};

const submitForm = (e: Event) => {
  e.preventDefault();
  const form = $(e.target as HTMLFormElement) as JQuery<HTMLElement>;
  const error = form.find(".alert-danger");
  defaultFormSend(e.target as HTMLFormElement, error, null, () => {
    window.location.reload();
  });
};

onMounted(() => {
  const loginButtons = document.querySelectorAll(".login");
  loginButtons.forEach((button) => {
    button.addEventListener("click", (e) => {
      e.preventDefault();
      showModal.value = true;
    });
  });
});
</script>
<style lang="scss" scoped></style>
