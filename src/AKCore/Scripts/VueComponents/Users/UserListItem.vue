<template>
    <tbody>
        <tr class="clickable hover-grey" @click.prevent="expanded=!expanded">
            <td>{{user.fullName}}</td>
            <td>{{user.userName}}</td>
            <td class="roles">
                <span v-for="role in user.roles" :key="role" class="role" v-tooltip:top="roleInfo(role)">
                    {{role}} <a v-if="expanded" class="remove-role glyphicon glyphicon-remove" @click.prevent.stop="removeRole(role)"></a>
                </span>
            </td>
            <td class="item-actions">
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
                                    <span class="listed-items" :key="post" v-for="post in user.posts">{{post}}</span>
                                </p>
                                <form class="form-inline save-medal" method="post" action="/User/SaveMedal" @submit.prevent="saveLastEarned">
                                    <div class="form-group">
                                        <strong>Senast förtjänade medalj: {{user.medal}}</strong>
                                        <input type="hidden" name="userName" :value="user.userName"/>
                                        <select class="form-control input-sm" name="medal" :value="user.medal">
                                            <option value="">Ingen</option>
                                            <option v-for="medal in medals" :key="medal" :value="medal">{{medal}}</option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <button type="submit" class="btn btn-primary input-sm">Spara</button>
                                    </div>
                                </form>
                                <form class="form-inline save-medal" method="post" action="/User/SaveGivenMedal" @submit.prevent="saveLastGiven">
                                    <div class="form-group">
                                        <strong>Senast utdelad medalj: {{user.givenMedal}}</strong>
                                        <input type="hidden" name="userName" :value="user.userName" />
                                        <select class="form-control input-sm" name="medal" :value="user.givenMedal">
                                            <option value="">Ingen</option>
                                            <option v-for="medal in medals" :key="medal" :value="medal">{{medal}}</option>
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
                            <p>Ändra användarinställningar: <i class="fa fa-question-circle roles-info" data-html="true" v-tooltip:top="'Supernintendo: Full access <br>Editor: Kan redigera sidor, filer, album samt se intresseanmlningar<br>Medlem: Kan anmäla sig till spelningar samt redigera sin info<br>Balett: Kan se balettsidor'" aria-hidden="true"></i></p>
                            <div class="edit-group">
                                <form class="form-inline add-role" action="/User/AddRole" method="POST" @submit.prevent="addRole">
                                    <div class="form-group">
                                        <input type="hidden" name="UserName" :value="user.userName" />
                                        <label>Lägg till roll: </label>
                                        <select class="form-control input-sm" name="Role">
                                            <option value="">Välj roll</option>
                                            <option v-for="role in roles" :key="role" :value="role">{{role}}</option>
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
                                    <v-select multiple :searchable="false" v-model="selectedPosts" :options="posts"></v-select>
                                    <div class="form-group">
                                        <button type="reset" @click.prevent="clearPosts" class="btn btn-primary input-sm">Rensa</button>
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
    import Constants from '../../constants';
    import vSelect from 'vue-select';
    import ApiService from '../../services/apiservice';

    export default {
        props: ['user'],
        components: {
            vSelect
        },
        data() {
            return {
                expanded: false,
                selectedPosts: []
            }
        },
        watch: {
            expanded(val) {
                if (val && this.user && this.user.posts) {
                    this.selectedPosts = this.user.posts.slice();
                }
            }
        },
        methods: {
            roleInfo(role) {
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
                if (confirm("Vill du verkligen ta bort " + this.user.fullName + "?")) {
                    const error = $(".alert-danger");
                    const success = $(".alert-success");
                    ApiService.postByUrl(
                        "/User/RemoveUser?userName=" + this.user.userName,
                        error, success, () => {
                            this.$emit('removeuser', this.user.userName);
                        });
                }
            },
            saveLastEarned(event) {
                const error = $(".alert-danger");
                const success = $(".alert-success");
                const form = $(event.target);
                ApiService.defaultFormSend(form, error, success, () => {
                    this.$emit('updateuserprop', {
                        userName: this.user.userName,
                        prop: "medal",
                        value: event.target.elements.medal.value
                    })
                });
            },
            saveLastGiven(event) {
                const error = $(".alert-danger");
                const success = $(".alert-success");
                const form = $(event.target);
                ApiService.defaultFormSend(form, error, success, () => {
                    this.$emit('updateuserprop', {
                        userName: this.user.userName,
                        prop: "givenMedal",
                        value: event.target.elements.medal.value
                    })
                });
            },
            removeRole(role) {
                const error = $(".alert-danger");
                const success = $(".alert-success");
                const roleIndex = this.user.roles.indexOf(role);
                if (roleIndex === -1) {
                    return;
                }
                const newRoles = this.user.roles.slice();
                newRoles.splice(roleIndex, 1);

                ApiService.postByUrl(
                    "/User/RemoveRole?UserName=" + this.user.userName + "&Role=" + role,
                    error, success, () => {
                        this.$emit('updateuserprop', {
                            userName: this.user.userName,
                            prop: "roles",
                            value: newRoles
                        })
                    });
            },
            addRole(event) {
                const error = $(".alert-danger");
                const success = $(".alert-success");
                const form = $(event.target);
                const role = event.target.elements.Role.value;
                const roleIndex = this.user.roles.indexOf(role);
                if (roleIndex !== -1) {
                    return;
                }
                const newRoles = this.user.roles.slice();
                newRoles.push(role);
                ApiService.defaultFormSend(form, error, success, () => {
                    this.$emit('updateuserprop', {
                        userName: this.user.userName,
                        prop: "roles",
                        value: newRoles
                    })
                });
            },
            resetPassword() {
                // $('#change-user-name').val(this.user.userName);
                // $('#changePasswordModal').modal('show');
                this.$emit("newpassword", this.user);
            },
            addPost() {
                const error = $(".alert-danger");
                const success = $(".alert-success");
                const postObj = { post: this.selectedPosts, userName: this.user.userName };
                ApiService.postByObject("/User/AddPost", postObj, error, success, () => {
                    this.$emit('updateuserprop', {
                        userName: this.user.userName,
                        prop: "posts",
                        value: this.selectedPosts.slice()
                    })
                });
            },
            clearPosts() {
                this.selectedPosts = [];
            }
        },
        computed: {
            medals() {
                return Constants.MEDALS;
            },
            roles() {
                return Constants.ROLES.filter((role) => {
                    return this.user.roles.indexOf(role) == -1;
                });
            },
            posts() {
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
       .item-actions{
        text-align: right;
    }
</style>
