<template>
    <tbody>
        <tr class="clickable hover-grey" @click.prevent="expanded=!expanded">
            <td>{{user.fullName}}</td>
            <td>{{user.userName}}</td>
            <td class="roles">
                <span v-for="role in user.roles" class="role" :title="roleInfo(role)">
                {{role}} <a v-if="expanded" class="remove-role glyphicon glyphicon-remove" @click.prevent.stop="removeRole(role)"></a></span>
            </td>
            <td>
                <a class="btn remove-user glyphicon glyphicon-remove" @click.prevent.stop="removeUser"></a>
            </td>
        </tr>
        <tr class="user-edit-container" v-if="expanded">
            <td colspan="4">
                <div class="user-edit">
                    <div class="user-edit-area row">
                        <div class="col-sm-6">
                            <p>Användarinfo:</p>
                            <div class="edit-group">
                                <p>
                                    <strong>Användarnamn:</strong> {{user.userName}}
                                </p>
                                <p>
                                    <strong>Fullt namn:</strong> {{user.firstName}} {{user.lastName}}
                                </p>
                                <p>
                                    <strong>Instrument:</strong> {{user.instrument}}
                                </p>
                                <p>
                                    <strong>Slavposter:</strong>
                                    <span class="listed-items" v-for="post in user.posts">{{post}}</span>
                                </p>
                                <form class="form-inline save-medal" method="post" action="/User/SaveMedal">
                                    <div class="form-group">
                                        <strong>Senast förtjänade medalj: {{user.medal}}</strong>
                                        <select class="form-control input-sm" :value="user.medal">
                                            <option value="">Ingen</option>
                                            <option v-for="medal in medals" :value="medal">{{medal}}</option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <button type="submit" class="btn btn-primary input-sm">Spara</button>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </td>
        </tr>
    </tbody>
</template>
<script>
    import Constants from '../../constants'

    export default {
        props: ['user'],
        data: function () {
            return {
                expanded: false
            }
        },
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
            },
            removeUser() {
                console.log("ta bort anv");
            },
            removeRole(role) {
                console.log("ta bort roll " + role);
            }
        },
        computed: {
            medals: function () {
                return Constants.MEDALS;
            }
        }
    }
</script>
<style lang="scss" scoped>
    @import "../../../Styles/variables.scss";

    .table > tbody + tbody {
        border-top: 0;
    }

    .remove-role {
        color: $akwhite;
        font-size: 12px;
    }
    .listed-items{
        margin-left: 5px;
    }

</style>
