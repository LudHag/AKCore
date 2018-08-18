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
            <div class="alert alert-success" style="display: none;">Användare skapad</div>
            <user-list :users="filteredUsers"
                       @updateuserprop="updateUserProp" 
                       @removeuser="removeUser"/>
        </div>
        <div class="col-md-3">
            <a class="btn btn-default" id="create-user" href="#" role="button">Lägg till ny användare</a>
        </div>
    </div>
            
</template>
<script>
    import UserList from "./UserList";
    import Spinner from "../Spinner";

    export default {
        components: {
            UserList,
            Spinner
        },
        data() {
            return {
                searchPhrase: "",
                inactive: false,
                users: [],
                allUsersCollected: false,
                loading: false
            }
        },
        computed: {
            filteredUsers() {
                var filtered = this.inactive ? this.users : this.users.filter((user) => {
                    return user.active != this.inactive;
                });
                if (this.searchPhrase !== "") {
                    filtered = filtered.filter((user) => {
                        const lowerPhrase = this.searchPhrase.toLowerCase();
                        return user.fullName.toLowerCase().indexOf(lowerPhrase) >= 0 ||
                            user.userName.toLowerCase().indexOf(lowerPhrase) >= 0;
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
                    success: function (res) {
                        self.users = res.users;
                        self.loading = false;
                    },
                    error: function () {
                        console.log("fel");
                        self.loading = false;
                    }
                });
            },
            updateUserProp(updateInfo) {
                const user = this.users.find((user) => {
                    return user.userName === updateInfo.userName;
                });
                user[updateInfo.prop] = updateInfo.value;
            },
            removeUser(userName) {
                const userIndex = this.users.findIndex((user) => {
                    return user.userName === userName;
                });
                this.users.splice(userIndex, 1);
            }
        },
        created() {
            this.getUsers(false);
        }
    }
</script>
<style lang="scss">
    .spinner-container{
        float: right;
    }
</style>
