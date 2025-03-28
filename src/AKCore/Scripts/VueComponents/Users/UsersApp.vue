﻿<template>
  <div id="user-app">
    <div class="col-md-12">
      <div class="form-inline ak-search">
        <div class="form-group">
          <input
            type="text"
            class="form-control"
            placeholder="Sök"
            v-model="searchPhrase"
          />
        </div>
        <div class="form-group indent">
          <div class="checkbox">
            <label>
              <input type="checkbox" v-model="inactive" /> Visa inaktiva
              medlemmar
            </label>
          </div>
        </div>
        <div class="form-group">
          <div class="col-md-3">
            <a
              class="btn btn-default"
              @click.prevent="createNewUser"
              href="#"
              role="button"
            >
              Lägg till ny användare
            </a>
          </div>
        </div>
        <div class="spinner-container" v-if="loading">
          <spinner :size="'medium'"></spinner>
        </div>
      </div>
      <div class="alert alert-danger" style="display: none"></div>
      <div
        class="alert alert-success alert-edit-user"
        ref="alertEditUser"
        style="display: none"
      ></div>
      <user-list
        :users="filteredUsers"
        @edit="editUser"
        @updateuserprop="updateUserProp"
        @removeuser="removeUser"
      />
    </div>
    <edit-user-modal
      :show-modal="showUserModal"
      :user="updateUser"
      @close="closeModal"
      @updated="userUpdated"
      @created="userCreated"
    ></edit-user-modal>
  </div>
</template>
<script setup lang="ts">
import UserList from "./UserList.vue";
import EditUserModal from "./EditUserModal.vue";
import Spinner from "../Spinner.vue";
import { User } from "./models";
import { ref, computed, watch, onMounted } from "vue";
import { getFromApi } from "../../services/apiservice";
import { slideUpAndDown } from "../../services/slidehandler";

const searchPhrase = ref("");
const inactive = ref(false);
const users = ref<User[]>([]);
const allUsersCollected = ref(false);
const loading = ref(false);
const showUserModal = ref(false);
const updateUser = ref<User | null>(null);
const alertEditUser = ref<HTMLElement | null>(null);

const filteredUsers = computed(() => {
  let filtered = inactive.value
    ? users.value
    : users.value.filter((user) => {
        return user.active != inactive.value;
      });
  if (searchPhrase.value !== "") {
    filtered = filtered.filter((user) => {
      const lowerPhrase = searchPhrase.value.toLowerCase();
      return (
        user.fullName.toLowerCase().indexOf(lowerPhrase) >= 0 ||
        user.userName.toLowerCase().indexOf(lowerPhrase) >= 0
      );
    });
  }
  return filtered;
});

watch(inactive, (newValue) => {
  if (newValue && !allUsersCollected.value) {
    getUsers(true);
  }
});

const getUsers = (inactive: boolean) => {
  loading.value = true;
  getFromApi<any>(`/User/UserListData?inactive=${inactive}`)
    .then((res) => {
      users.value = res.users;
      if (inactive) {
        allUsersCollected.value = true;
      }
    })
    .finally(() => {
      loading.value = false;
    });
};

const updateUserProp = (updateInfo: any) => {
  const user = users.value.find((user) => {
    return user.userName === updateInfo.userName;
  });
  // @ts-ignore
  user[updateInfo.prop] = updateInfo.value;
};

const removeUser = (userName: string) => {
  const userIndex = users.value.findIndex((user) => {
    return user.userName === userName;
  });
  users.value.splice(userIndex, 1);
};

const userUpdated = (user: User) => {
  const index = users.value.map((u) => u.id).indexOf(user.id);
  users.value = Object.assign([], users.value, { [index]: user });
  closeModal();
  slideUpAndDown(alertEditUser.value!, "Användare uppdaterad");
};

const userCreated = (user: User) => {
  user.fullName = user.firstName + " " + user.lastName;
  users.value.push(user);
  closeModal();
  slideUpAndDown(alertEditUser.value!, "Användare skapad");
};

const closeModal = () => {
  showUserModal.value = false;
  updateUser.value = null;
};

const editUser = (user: User) => {
  showUserModal.value = true;
  updateUser.value = user;
};

const createNewUser = () => {
  showUserModal.value = true;
  updateUser.value = null;
};

onMounted(() => {
  getUsers(false);
});
</script>
<style lang="scss" scoped>
.spinner-container {
  float: right;
}
</style>
