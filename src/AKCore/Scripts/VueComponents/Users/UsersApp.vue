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
            </div>
            <div class="alert alert-danger" style="display: none;"></div>
            <div class="alert alert-success" style="display: none;">Användare skapad</div>
            <user-list :users="filteredUsers" />
        </div>
    </div>
            
</template>
<script>
    import UserList from "./UserList";

    export default {
        components: {
            UserList
        },
        data: function () {
            return {
                searchPhrase: "",
                inactive: false,
                users: [],
                allUsersCollected: false
            }
        },
        computed: {
            filteredUsers: function () {
                var filtered = this.inactive ? this.users : this.users.filter((user) => {
                    return user.active != this.inactive;
                });
                if (this.searchPhrase !== "") {
                    filtered = filtered.filter((user) => {
                        return user.fullName.toLowerCase().indexOf(this.searchPhrase) >= 0 ||
                            user.userName.toLowerCase().indexOf(this.searchPhrase) >= 0;
                    });
                }
                return filtered;
            }
        },
        watch: {
            inactive: function () {
                if (this.inactive && !this.allUsersCollected) {
                    this.getUsers(true);
                }
            }
        },
        methods: {
            getUsers: function (inactive) {
                const self = this;
                $.ajax({
                    url: "/User/UserListData",
                    type: "POST",
                    data: { inactive },
                    success: function (res) {
                        self.users = res.users;
                    },
                    error: function () {
                        console.log("fel");
                    }
                });
            }
        },
        created: function () {
            this.getUsers(false);
        }
    }
</script>
<style lang="scss">

</style>
