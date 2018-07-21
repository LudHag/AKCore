<template>
    <div id="user-list">
        <table class="table">
            <thead>
                <tr>
                    <th>Namn</th>
                    <th>Email</th>
                    <th>Roller</th>
                    <th></th>
                </tr>
            </thead>
            <user-list-item v-for="user in users" 
                            :user="user" 
                            :key="user.userName"
                            @updateuserprop="updateUserProp"
                            @removeuser="removeUser">
            </user-list-item>
        </table>
    </div>
</template>
<script>
    import UserListItem from "./UserListItem";
    export default {
        props: ['users'],
        components: {
            UserListItem
        },
        methods: {
            updateUserProp(updateInfo) {
                this.$emit('updateuserprop', updateInfo);
            },
            removeUser(userName) {
                this.$emit('removeuser', userName);
            }
        }
    }
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
