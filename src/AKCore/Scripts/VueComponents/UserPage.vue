<template>
    <div id="user-app">
        <user-list :users="filteredUsers" />
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
                users: []
            }
        },
        computed: {
            filteredUsers: function () {
                return this.users;
            }
        },
        created: function () {
            const self = this;
            $.ajax({
                url: "/User/UserListData",
                type: "POST",
                data: { SearchPhrase: this.searchPhrase, Inactive: this.inactive },
                success: function (res) {
                    self.users = res.users;
                },
                error: function () {
                    console.log("fel");
                }
            });
        }
    }
</script>
<style lang="scss">

</style>
