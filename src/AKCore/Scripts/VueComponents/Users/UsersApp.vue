<template>
  <div class="row" id="user-app">
    <div class="col-md-9">
      <div class="form-inline ak-search">
        <div class="form-group">
          <input type="text" class="form-control" placeholder="Sök" v-model="searchPhrase">
        </div>
        <div class="form-group">
          <div class="checkbox">
            <label>
              <input type="checkbox" v-model="inactive"> Visa inaktiva medlemmar
            </label>
          </div>
        </div>
        <div class="spinner-container" v-if="loading">
          <spinner :size="'medium'"></spinner>
        </div>
      </div>
      <div class="alert alert-danger" style="display: none;"></div>
      <div class="alert alert-success alert-edit-user" style="display: none;"></div>
      <user-list :users="filteredUsers" @edit="editUser" @updateuserprop="updateUserProp" @removeuser="removeUser"/>
    </div>
    <div class="col-md-3">
      <a class="btn btn-default" @click.prevent="createNewUser" href="#" role="button">Lägg till ny användare</a>
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
<script>
import UserList from "./UserList";
import EditUserModal from "./EditUserModal";
import Spinner from "../Spinner";

export default {
  components: {
    UserList,
    Spinner,
    EditUserModal
  },
  data() {
    return {
      searchPhrase: "",
      inactive: false,
      users: [],
      allUsersCollected: false,
      loading: false,
      showUserModal: false,
      updateUser: null
    };
  },
  computed: {
    filteredUsers() {
      var filtered = this.inactive
        ? this.users
        : this.users.filter(user => {
            return user.active != this.inactive;
          });
      if (this.searchPhrase !== "") {
        filtered = filtered.filter(user => {
          const lowerPhrase = this.searchPhrase.toLowerCase();
          return (
            user.fullName.toLowerCase().indexOf(lowerPhrase) >= 0 ||
            user.userName.toLowerCase().indexOf(lowerPhrase) >= 0
          );
        });
      }
      return filtered;
    }
  },
  watch: {
    inactive() {
      if (this.inactive && !this.allUsersCollected) {
        this.getUsers(true);
      }
    }
  },
  methods: {
    getUsers(inactive) {
      const self = this;
      this.loading = true;
      $.ajax({
        url: "/User/UserListData",
        type: "POST",
        data: { inactive },
        success: function(res) {
          self.users = res.users;
          self.loading = false;
        },
        error: function() {
          console.log("fel");
          self.loading = false;
        }
      });
    },
    updateUserProp(updateInfo) {
      const user = this.users.find(user => {
        return user.userName === updateInfo.userName;
      });
      user[updateInfo.prop] = updateInfo.value;
    },
    removeUser(userName) {
      const userIndex = this.users.findIndex(user => {
        return user.userName === userName;
      });
      this.users.splice(userIndex, 1);
    },
    userUpdated(user) {
        const index = this.users.map(u => u.id).indexOf(user.id);
        this.users = Object.assign([], this.users, {[index]: user});
        this.closeModal();
        $(".alert-edit-user").text("Användare uppdaterad").slideDown().delay(4000).slideUp();
    },
    userCreated(user){
      user.fullName = user.firstName + " " + user.lastName;
      this.users.push(user);
      this.closeModal();
      $(".alert-edit-user").text("Användare skapad").slideDown().delay(4000).slideUp();
    },
    closeModal() {
      this.showUserModal = false;
      this.updateUser = null;
    },
    editUser(user) {
        this.showUserModal = true;
        this.updateUser = user;
    },
    createNewUser() {
        this.showUserModal = true;
        this.updateUser = null;
    }
  },
  created() {
    this.getUsers(false);
  }
};
</script>
<style lang="scss">
.spinner-container {
  float: right;
}
</style>
