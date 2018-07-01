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
            <tbody>
                <tr v-for="user in users" class="clickable collapsed hover-grey">
                    <td>{{user.fullName}}</td>
                    <td>{{user.userName}}</td>
                    <td class="roles" >
                        <span v-for="role in user.roles" class="role" data-toggle="tooltip" data-placement="top" :title="roleInfo(role)">{{role}} <a class="remove-role glyphicon glyphicon-remove" :data-user="user.userName" :data-role="role"></a></span>
                    </td>
                    <td>
                        <a class="btn remove-user glyphicon glyphicon-remove" :data-user="user.userName"></a>
                    </td>
                </tr>
            </tbody>
        </table>
        <div v-for="user in users">
            {{user.firstName}} {{user.lastName}}
        </div>
    </div>
</template>
<script>
    export default {
        props: ['users'],
        methods: {
            roleInfo: function (role) {
                switch (role) {
                    case "SuperNintendo":
                        return "Har rättigheter att redigera all information på webben";
                    case "Editor":
                        return "Kan redigera sidor, musikalbum, ladda upp filer samt titta på intresseanmälningar";
                    case "Medlem":
                        return "Kan anmäla sig till spelningar, syns i adressregistret samt kan redigera sin profil";
                    case "Balett":
                        return "Kan se balettsidor";
                }
                return "";
            }
        }
    }
</script>
<style lang="scss" scoped>
    @import "../../Styles/variables.scss";

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
