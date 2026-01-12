<template>
  <div id="user-list" class="user-list-scroll">
    <div
      class="alert alert-success update-password-success"
      ref="updatePasswordSuccess"
      style="display: none"
    >
      Lösenord uppdaterat
    </div>
    <table class="table">
      <thead>
        <tr>
          <th class="clickable-header" @click="updateSort('name')">Namn</th>
          <th class="clickable-header" @click="updateSort('userName')">
            Användarnamn
          </th>
          <th class="clickable-header" @click="updateSort('roles')">Roller</th>
          <th class="clickable-header" @click="updateSort('lastSignedIn')">
            Senast inloggad
          </th>
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
import { SortUser, UpdateInfo, User } from "./models";
import { ref, computed } from "vue";
import { slideUpAndDown } from "@services/slidehandler";

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
const updatePasswordSuccess = ref<HTMLElement | null>(null);
const sort = ref<SortUser>("name");
const asc = ref(true);

const updateSort = (newSort: SortUser) => {
  if (sort.value === newSort) {
    asc.value = !asc.value;
  } else {
    asc.value = true;
  }
  sort.value = newSort;
};

const getDateValue = (date: string) => {
  return date ? Date.parse(date) : 0;
};

const sortedUsers = computed(() => {
  return [...props.users].sort((a, b) => {
    const sortA = asc.value ? a : b;
    const sortB = asc.value ? b : a;
    if (sort.value === "name") {
      return sortA.firstName?.localeCompare(sortB.firstName);
    } else if (sort.value === "userName") {
      return sortA.userName.localeCompare(sortB.userName);
    } else if (sort.value === "roles") {
      return sortA.roles.join().localeCompare(sortB.roles.join());
    } else if (sort.value === "lastSignedIn") {
      const dateAValue = getDateValue(sortA.lastSignedIn);
      const dateBValue = getDateValue(sortB.lastSignedIn);
      return dateBValue - dateAValue;
    }
    return 0;
  });
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
  slideUpAndDown(updatePasswordSuccess.value!);
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
.clickable-header {
  cursor: pointer;
}
</style>
