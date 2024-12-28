<template>
  <div>
    <h3>{{ t("change-password") }}:</h3>
    <form
      method="POST"
      action="/Profile/ChangePassword"
      @submit.prevent="changePassword"
    >
      <div
        class="alert alert-danger"
        ref="passworderror"
        style="display: none"
      ></div>
      <div
        class="alert alert-success"
        ref="passwordsuccess"
        style="display: none"
      >
        {{ t("password-updated") }}
      </div>
      <div class="form-group">
        <label for="newpass">{{ t("new-password") }}:</label>
        <input
          v-model="password"
          type="password"
          class="form-control"
          name="password"
          :placeholder="t('new-password')"
          required
        />
      </div>
      <div class="form-group">
        <label for="confirmpass">{{ t("confirm-password") }}:</label>
        <input
          v-model="confirmPass"
          type="password"
          class="form-control"
          :placeholder="t('confirm-password')"
          required
        />
      </div>
      <div class="form-group">
        <button type="submit" class="btn btn-default">
          {{ t("update-password") }}
        </button>
      </div>
    </form>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { defaultFormSend } from "../../services/apiservice";
import { slideUpAndDown } from "../../services/slidehandler";
import { TranslationDomain, translate } from "../../translations";

const password = ref("");
const confirmPass = ref("");
const passworderror = ref<HTMLElement | null>(null);
const passwordsuccess = ref<HTMLElement | null>(null);

const changePassword = async (event: Event) => {
  if (password.value !== confirmPass.value) {
    slideUpAndDown(passworderror.value!, "Lösenord matchar ej");
    return;
  }

  defaultFormSend(
    event.target as HTMLFormElement,
    passworderror.value,
    passwordsuccess.value,
    () => {
      password.value = "";
      confirmPass.value = "";
    },
  );
};

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};
</script>
<style lang="scss"></style>
