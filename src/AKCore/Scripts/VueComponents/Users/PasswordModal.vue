<template>
  <modal :show-modal="showModal" :header="'Byt lösenord'" @close="close">
    <div slot="body" class="modal-body">
      <form
        action="/User/ChangePassword"
        ref="changepassform"
        autocomplete="off"
        method="post"
        @submit.prevent="submitForm"
        v-if="user"
      >
        <div class="alert alert-danger" style="display: none;"></div>
        <input type="hidden" class="form-control" name="userName" :value="user.userName" required>
        <div class="form-group">
          <label>Lösenord</label>
          <input
            type="password"
            autocomplete="off"
            class="form-control"
            name="password"
            placeholder="Lösenord"
            required
          >
        </div>
        <div class="modal-footer">
          <button
            type="submit"
            class="btn btn-primary"
          >Skapa nytt lösenord</button>
        </div>
      </form>
    </div>
  </modal>
</template>
<script>
import ApiService from "../../services/apiservice";
import Modal from "../Modal";

export default {
  props: ["user", "showModal"],
  components: {
    Modal
  },
  methods: {
    close() {
      this.$emit("close");
    },
    submitButton() {
      const form = $(this.$refs.changepassform);
      form.submit();
    },
    submitForm(event) {
      const error = $(".alert-danger");
      const form = $(this.$refs.changepassform);
      ApiService.defaultFormSend(form, error, null, () => {
        this.$emit("update");
        this.close();
      });
    }
  }
};
</script>
<style lang="scss" scoped>
</style>
