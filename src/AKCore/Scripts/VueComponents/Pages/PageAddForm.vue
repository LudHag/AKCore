<template>
  <div class="row">
    <div class="col-md-6">
      <button
        type="button"
        class="btn btn-primary"
        @click="showForm = !showForm"
      >
        Lägg till sida
      </button>
    </div>
  </div>
  <div class="row edit-form" v-if="showForm">
    <form
      class="form-inline"
      action="/Edit/CreatePage"
      method="post"
      @submit.prevent="submitForm"
    >
      <div class="alert alert-danger" style="display: none" ref="error"></div>
      <div class="form-group">
        <label for="name">Namn: </label>
        <input
          type="text"
          class="form-control"
          name="name"
          id="name"
          placeholder="Sidnamn"
        />
      </div>
      <div class="form-group">
        <label for="slug">Sidlänk: </label>
        <input
          type="text"
          class="form-control"
          name="slug"
          id="slug"
          placeholder="sidlänk"
        />
      </div>
      <div class="form-group">
        <div class="checkbox">
          <label>
            <input type="checkbox" name="loggedIn" /> Enbart för inloggade
          </label>
        </div>
      </div>
      <button type="submit" class="btn btn-default">Skapa</button>
    </form>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { defaultFormSend } from "@services/apiservice";

const showForm = ref(false);
const error = ref<HTMLElement | null>(null);

const submitForm = (e: Event) => {
  defaultFormSend(e.target as HTMLFormElement, error.value, null, () => {
    window.location.reload();
  });
};
</script>
<style lang="scss" scoped>
.edit-form {
  padding-top: 35px;
  .row {
    margin-bottom: 10px;
  }
}
.form-group {
  margin-right: 10px;
}
</style>
