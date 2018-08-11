<template>
    <div id="profile-app" v-if="profileData && profileData.userName !== 'nintendo'">
        <div class="row">
            <div class="col-md-6">
                <h3>Användarinfo</h3>
                <form method="POST" action="/Profile/EditProfile" @submit.prevent="updateProfile">
                    <div class="alert alert-danger" style="display: none;"></div>
                    <div class="alert alert-success" style="display: none;">Profil uppdaterad</div>
                    <div class="form-group">
                        <label>Användarnamn</label>
                        <input v-model="profileData.userName" type="text" class="form-control" placeholder="Användarnamn" name="UserName">
                    </div>
                    <div class="form-group">
                        <label>Epost</label>
                        <input v-model="profileData.email" class="form-control" placeholder="Epost" type="email" name="Email">
                    </div>
                    <div class="form-group">
                        <label>Förnamn</label>
                        <input v-model="profileData.firstName" class="form-control" placeholder="Förnamn" name="FirstName">
                    </div>
                    <div class="form-group">
                        <label>Efternamn</label>
                        <input v-model="profileData.lastName" class="form-control" placeholder="Efternamn" name="LastName">
                    </div>
                    <div class="form-group">
                        <label>Telefonnummer</label>
                        <input v-model="profileData.phone" class="form-control" placeholder="Telefonnummer" name="Phone">
                    </div>
                    <div class="form-group">
                        <label>Instrument</label>
                        <select v-model="profileData.instrument" class="form-control" name="Instrument" required>
                            <option value="">Välj instrument</option>
                            <option v-for="instr in instruments">{{instr}}</option>
                        </select>
                    </div>
                    <div class="form-group">
                        <label>Andra instrument</label>
                        <v-select multiple 
                                  :searchable="false" 
                                  v-model="profileData.otherInstrument" 
                                  :options="othInstruments"></v-select>
                    </div>
                    <div class="form-group">
                        <button type="submit" class="btn btn-default">Uppdatera profil</button>
                    </div>
                </form>
            </div>
            <div class="col-md-6">
                <h3>Byt lösenord:</h3>
                <form method="POST" action="/Profile/ChangePassword" @submit.prevent="changePassword">
                    <div class="alert alert-danger" style="display: none;"></div>
                    <div class="alert alert-success" style="display: none;">Lösenord uppdaterat</div>
                    <div class="form-group">
                        <label for="newpass">Nytt lösenord:</label>
                        <input v-model="password" type="password" class="form-control" name="password" placeholder="Lösenord" required>
                    </div>
                    <div class="form-group">
                        <label for="confirmpass">Bekräfta lösenord:</label>
                        <input v-model="confirmPass"  type="password" class="form-control" placeholder="Bekräfta lösenord" required>
                    </div>
                    <div class="form-group">
                        <button type="submit" class="btn btn-default">Uppdatera lösenord</button>
                    </div>
                </form>
                <div>
                    <div v-if="profileData.roles">
                        <h3>Roller</h3>
                        <div class="roles">
                            <span class="role" v-for="role in profileData.roles">
                                {{role}}
                            </span>
                        </div>
                    </div>
                    <div v-if="profileData.poster">
                        <h3>Slavposter</h3>
                        <div class="roles">
                            <span class="role" v-for="post in profileData.poster">
                                {{post}}
                            </span>
                        </div>
                    </div>
                    <div v-if="profileData.medal">
                        <h3>Senaste terminsmedalj</h3>
                        <p>{{profileData.medal}}</p>
                    </div>
                    <div v-if="profileData.givenMedal">
                        <h3>Senaste utdelade terminsmedalj</h3>
                        <p>{{profileData.givenMedal}}</p>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import Constants from '../constants';
    import vSelect from 'vue-select';

    export default {
        components: {
            vSelect
        },
        data() {
            return {
                profileData: null,
                password: "",
                confirmPass: ""
            }
        },
        computed: {
            instruments() {
                return Constants.INSTRUMENTS;
            },
            othInstruments() {
                return Constants.INSTRUMENTS.filter((instr) => {
                    return instr !== this.profileData.instrument;
                });
            }
        },
        methods: {
            updateProfile() {
                console.log("update");
            },
            changePassword(event) {
                const self = this;
                const form = $(event.target);
                const error = form.find(".alert-danger");
                const success = form.find(".alert-success");
                if (this.password !== this.confirmPass) {
                    error.text("Lösenord matchar ej");
                    error.slideDown().delay(3500).slideUp();
                    return;
                }

                $.ajax({
                    url: form.attr("action"),
                    type: form.attr("method"),
                    data: form.serialize(),
                    success: function (res) {
                        if (res.success) {
                            success.slideDown().delay(3000).slideUp();
                            self.password = "";
                            self.confirmPass = "";
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(3500).slideUp();
                        }
                    },
                    error: function (err) {
                        error.text("Misslyckades med att ändra lösenord");
                        error.slideDown().delay(3500).slideUp();
                    }
                });
            }
        },
        created() {
            const self = this;
            $.ajax({
                url: "/Profile/ProfileData",
                type: "GET",
                success: function (res) {
                    self.profileData = res;
                }
            });
        }
    }
</script>
<style lang="scss">

</style>
