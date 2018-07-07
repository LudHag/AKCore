<template>
    <div id="user-list">
        {{users.length}}
        <table class="table">
            <thead>
                <tr>
                    <th>Namn</th>
                    <th>Email</th>
                    <th>Roller</th>
                    <th></th>
                </tr>
            </thead>
            <user-list-item v-for="user in users" :user="user" :key="user.userName">
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

    table .tooltip {
        opacity: 1;
    }
    table .tooltip-inner {
        background-color: $akwhite;
        color: #555555;
    }

    table .tooltip-arrow {
        border-top-color: $akwhite;
    }
</style>
