<template>
  <div id="user-list">
    <div class="alert alert-success update-password-success" style="display: none;">Lösenord uppdaterat</div>
    <table class="table">
      <thead>
        <tr>
          <th>Namn</th>
          <th>Email</th>
          <th>Roller</th>
          <th></th>
        </tr>
      </thead>
      <user-list-item
        v-for="user in users"
        :user="user"
        :key="user.userName"
        @newpassword="updatePassword"
        @updateuserprop="updateUserProp"
        @removeuser="removeUser"
      ></user-list-item>
    </table>
    <password-modal
      :show-modal="showUpdatePasswordModal"
      :user="updatePasswordUser"
      @success="newPasswordSuccess"
      @close="closeModal"
    ></password-modal>
  </div>
</template>
<script>
import UserListItem from "./UserListItem";
import PasswordModal from "./PasswordModal";

export default {
  props: ["users"],
  components: {
    UserListItem,
    PasswordModal
  },
  data() {
    return {
      showUpdatePasswordModal: false,
      updatePasswordUser: null
    };
  },
  methods: {
    updateUserProp(updateInfo) {
      this.$emit("updateuserprop", updateInfo);
    },
    removeUser(userName) {
      this.$emit("removeuser", userName);
    },
    updatePassword(user) {
      this.showUpdatePasswordModal = true;
      this.updatePasswordUser = user;
    },
    closeModal() {
      this.showUpdatePasswordModal = false;
      this.updatePasswordUser = null;
    },
    newPasswordSuccess() {
      $(".update-password-success").slideDown().delay(4000).slideUp();
    }
  }
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";

table {
  table-layout: auto;
}
table .role {
  cursor: pointer;
}
</style>
