<template>
  <div>
    <modal :show-modal="showModal" :header="t('log-in')" @close="close">
      <template #body>
        <form
          action="/Account/Login"
          id="loginForm"
          @submit.prevent="submitForm"
          method="post"
        >
          <div class="modal-body">
            <div
              class="alert alert-danger"
              ref="error"
              style="display: none"
            ></div>
            <div class="form-group">
              <label for="username">{{ t("user-name", "common") }}</label>
              <input
                type="text"
                class="form-control"
                id="username"
                name="Username"
                ref="username"
                :placeholder="t('user-name', 'common')"
              />
            </div>
            <div class="form-group">
              <label for="password">{{ t("password", "common") }}</label>
              <input
                type="password"
                class="form-control"
                id="password"
                name="Password"
                :placeholder="t('password', 'common')"
              />
            </div>
          </div>
          <div class="modal-footer">
            <button type="button" class="btn btn-default" @click="close">
              {{ t("close", "common") }}
            </button>
            <button type="submit" class="btn btn-primary submit-login">
              {{ t("log-in") }}
            </button>
          </div>
        </form>
      </template>
    </modal>
  </div>
</template>
<script setup lang="ts">
import Modal from "../Modal.vue";
import { defaultFormSend } from "@services/apiservice";
import { TranslationDomain, translate } from "../../translations";
import { ref, onMounted, nextTick } from "vue";

const showModal = ref(false);
const error = ref<HTMLElement | null>(null);
const username = ref<HTMLElement | null>(null);

const close = () => {
  showModal.value = false;
};

const submitForm = (e: Event) => {
  e.preventDefault();
  defaultFormSend(e.target as HTMLFormElement, error.value, null, () => {
    window.location.reload();
  });
};

const t = (key: string, domain: TranslationDomain = "login") => {
  return translate(domain, key);
};

onMounted(() => {
  const loginButtons = document.querySelectorAll(".login");
  loginButtons.forEach((button) => {
    button.addEventListener("click", (e) => {
      e.preventDefault();
      showModal.value = true;
      nextTick(() => {
        username.value?.focus();
      });
    });
  });
});
</script>
<style lang="scss" scoped></style>
