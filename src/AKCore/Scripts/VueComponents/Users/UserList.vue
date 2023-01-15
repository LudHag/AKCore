<template>
  <div id="user-list">
    <div
      class="alert alert-success update-password-success"
      style="display: none"
    >
      Lösenord uppdaterat
    </div>
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
        v-for="user in sortedUsers"
        :user="user"
        :key="user.userName"
        @edit="$emit('edit', user)"
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
<script setup lang="ts">
import UserListItem from "./UserListItem.vue";
import PasswordModal from "./PasswordModal.vue";
import { UpdateInfo, User } from "./models";
import { ref, computed } from "vue";

const emit = defineEmits<{
  (e: "updateuserprop", updateInfo: UpdateInfo): void;
  (e: "removeuser", userName: string): void;
  (e: "edit", user: User): void;
}>();

const props = defineProps<{
  users: User[];
}>();

const showUpdatePasswordModal = ref(false);
const updatePasswordUser = ref<User | null>(null);

const sortedUsers = computed(() => {
  return [...props.users].sort((a, b) =>
    a.firstName.localeCompare(b.firstName)
  );
});

const updateUserProp = (updateInfo: UpdateInfo) => {
  emit("updateuserprop", updateInfo);
};

const removeUser = (userName: string) => {
  emit("removeuser", userName);
};

const updatePassword = (user: User) => {
  showUpdatePasswordModal.value = true;
  updatePasswordUser.value = user;
};

const closeModal = () => {
  showUpdatePasswordModal.value = false;
  updatePasswordUser.value = null;
};

const newPasswordSuccess = () => {
  $(".update-password-success").slideDown().delay(4000).slideUp();
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
