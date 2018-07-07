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
                                <form class="form-inline save-medal" method="post" action="/User/SaveMedal" @submit.prevent="saveLastEarned">
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
                                <form class="form-inline save-medal" method="post" action="/User/SaveGivenMedal" @submit.prevent="saveLastGiven">
                                    <div class="form-group">
                                        <strong>Senast utdelad medalj: {{user.givenMedal}}</strong>
                                        <select class="form-control input-sm" :value="user.givenMedal">
                                            <option value="">Ingen</option>
                                            <option v-for="medal in medals" :value="medal">{{medal}}</option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <button type="submit" class="btn btn-primary input-sm">Spara</button>
                                    </div>
                                </form>
                                <a href="#" :data-user="user.userName" class="btn btn-default edit-user-info">Redigera användarinfo</a>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <p>Ändra användarinställningar: <i class="fa fa-question-circle roles-info" data-html="true" data-toggle="tooltip" data-placement="top" title="Supernintendo: Full access <br>Editor: Kan redigera sidor, filer, album samt se intresseanmlningar<br>Medlem: Kan anmäla sig till spelningar samt redigera sin info<br>Balett: Kan se balettsidor" aria-hidden="true"></i></p>
                            <div class="edit-group">
                                <form class="form-inline add-role" action="/User/AddRole" method="POST" @submit.prevent="addRole">
                                    <div class="form-group">
                                        <label>Lägg till roll: </label>
                                        <select class="form-control input-sm">
                                            <option value="">Välj roll</option>
                                            <option v-for="role in roles" :value="role">{{role}}</option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <button type="submit" class="btn btn-primary input-sm">Lägg till</button>
                                    </div>
                                </form>
                            </div>
                            <div class="edit-group">
                                <a href="#" class="btn btn-primary reset-pass-btn" @click.prevent="resetPassword">Nytt lösenord</a>
                            </div>
                            <div class="edit-group">
                                <label>Lägg till post(er): </label>
                                <form class="form-inline add-post" action="/User/AddPost" method="POST" @submit.prevent="addPost">
                                    <select name="post" multiple class="form-control multi-select" >
                                        <option v-for="post in posts">{{post}}</option>
                                    </select>
                                    <div class="form-group">
                                        <button type="reset" class="btn btn-primary input-sm">Rensa</button>
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
            },
            saveLastEarned() {
                console.log("spara medalj");
            },
            saveLastGiven() {
                console.log("spara given medalj");
            },
            addRole() {
                console.log("lägg till roll");
            },
            resetPassword() {
                console.log("Återställ lösenord");
            },
            addPost() {
                console.log("Lägg till post");
            }
        },
        computed: {
            medals: function () {
                return Constants.MEDALS;
            },
            roles: function () {
                return Constants.ROLES.filter((role) => {
                    return this.user.roles.indexOf(role) == -1;
                });
            },
            posts: function () {
                return Constants.POSTS;
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
