<template>
  <div
    id="profile-app"
    v-if="profileData && profileData.userName !== 'nintendo'"
  >
    <div class="row">
      <div class="col-md-6">
        <h3>Användarinfo</h3>
        <form
          method="POST"
          action="/Profile/EditProfile"
          @submit.prevent="updateProfile"
        >
          <div class="alert alert-danger" style="display: none"></div>
          <div class="alert alert-success" style="display: none">
            Profil uppdaterad
          </div>
          <div class="form-group">
            <label>Användarnamn</label>
            <input
              v-model="profileData.userName"
              type="text"
              class="form-control"
              placeholder="Användarnamn"
              name="UserName"
            />
          </div>
          <div class="form-group">
            <label>Epost</label>
            <input
              v-model="profileData.email"
              class="form-control"
              placeholder="Epost"
              type="email"
              name="Email"
            />
          </div>
          <div class="form-group">
            <label>Förnamn</label>
            <input
              v-model="profileData.firstName"
              class="form-control"
              placeholder="Förnamn"
              name="FirstName"
            />
          </div>
          <div class="form-group">
            <label>Efternamn</label>
            <input
              v-model="profileData.lastName"
              class="form-control"
              placeholder="Efternamn"
              name="LastName"
            />
          </div>
          <div class="form-group">
            <label>Telefonnummer</label>
            <input
              v-model="profileData.phone"
              class="form-control"
              placeholder="Telefonnummer"
              name="Phone"
            />
          </div>
          <div class="form-group">
            <label>Instrument</label>
            <select
              v-model="profileData.instrument"
              class="form-control"
              name="Instrument"
              required
            >
              <option value="">Välj instrument</option>
              <option v-for="instr in instruments" :key="instr">
                {{ instr }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>Andra instrument</label>
            <v-select
              multiple
              :searchable="false"
              v-model="profileData.otherInstruments"
              :options="othInstruments"
            ></v-select>
          </div>
          <div class="form-group">
            <button type="submit" class="btn btn-default">
              Uppdatera profil
            </button>
          </div>
        </form>
      </div>
      <div class="col-md-6">
        <h3>Byt lösenord:</h3>
        <form
          method="POST"
          action="/Profile/ChangePassword"
          @submit.prevent="changePassword"
        >
          <div class="alert alert-danger" style="display: none"></div>
          <div class="alert alert-success" style="display: none">
            Lösenord uppdaterat
          </div>
          <div class="form-group">
            <label for="newpass">Nytt lösenord:</label>
            <input
              v-model="password"
              type="password"
              class="form-control"
              name="password"
              placeholder="Lösenord"
              required
            />
          </div>
          <div class="form-group">
            <label for="confirmpass">Bekräfta lösenord:</label>
            <input
              v-model="confirmPass"
              type="password"
              class="form-control"
              placeholder="Bekräfta lösenord"
              required
            />
          </div>
          <div class="form-group">
            <button type="submit" class="btn btn-default">
              Uppdatera lösenord
            </button>
          </div>
        </form>
        <div>
          <div v-if="profileData.roles && profileData.roles.length > 0">
            <h3>Roller</h3>
            <div class="roles">
              <span class="role" :key="role" v-for="role in profileData.roles">
                {{ role }}
              </span>
            </div>
          </div>
          <div v-if="profileData.posts && profileData.posts.length > 0">
            <h3>Slavposter</h3>
            <div class="roles">
              <span class="role" :key="post" v-for="post in profileData.posts">
                {{ post }}
              </span>
            </div>
          </div>
          <div v-if="profileData.medal">
            <h3>Senaste terminsmedalj</h3>
            <p>{{ profileData.medal }}</p>
          </div>
          <div v-if="profileData.givenMedal">
            <h3>Senaste utdelade terminsmedalj</h3>
            <p>{{ profileData.givenMedal }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import Constants from "../constants";
// @ts-ignore
import vSelect from "vue-select";
import { ref, computed, onMounted } from "vue";
import { ProfileData } from "./models";

const profileData = ref<ProfileData | null>(null);
const password = ref("");
const confirmPass = ref("");

const instruments = Constants.INSTRUMENTS;

const othInstruments = computed(() => {
  return Constants.INSTRUMENTS.filter((instr) => {
    return instr !== profileData.value?.instrument;
  });
});

const updateProfile = async (event: Event) => {
  const form = $(event.target as HTMLFormElement);
  const success = form.find(".alert-success");
  const error = form.find(".alert-danger");
  $.ajax({
    url: form.attr("action"),
    type: form.attr("method"),
    contentType: "application/json",
    data: JSON.stringify(profileData.value),
    success: function (res) {
      if (res.success) {
        //@ts-ignore
        form.parent().get(0).scrollIntoView();
        success.text("Din profil uppdaterades");
        success.slideDown().delay(3000).slideUp();
      } else {
        error.text(res.message);
        error.slideDown().delay(3500).slideUp();
      }
    },
    error: function (err) {
      error.text("Något gick fel");
      error.slideDown().delay(3500).slideUp();
    },
  });
};

const changePassword = async (event: Event) => {
  const form = $(event.target as HTMLFormElement);
  const error = form.find(".alert-danger");
  const success = form.find(".alert-success");
  if (password.value !== confirmPass.value) {
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
        password.value = "";
        confirmPass.value = "";
      } else {
        error.text(res.message);
        error.slideDown().delay(3500).slideUp();
      }
    },
    error: function (err) {
      error.text("Något gick fel");
      error.slideDown().delay(3500).slideUp();
    },
  });
};

onMounted(() => {
  $.ajax({
    url: "/Profile/ProfileData",
    type: "GET",
    success: function (res) {
      profileData.value = res;
    },
  });
});
</script>
<style lang="scss"></style>
