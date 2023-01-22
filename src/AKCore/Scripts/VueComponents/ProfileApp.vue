<template>
  <div
    id="profile-app"
    v-if="profileData && profileData.userName !== 'nintendo'"
  >
    <div class="row" ref="formcontainer">
      <div class="col-md-6">
        <h3>Användarinfo</h3>
        <form
          method="POST"
          action="/Profile/EditProfile"
          @submit.prevent="updateProfile"
        >
          <div
            class="alert alert-danger"
            ref="formerror"
            style="display: none"
          ></div>
          <div
            class="alert alert-success"
            ref="formsuccess"
            style="display: none"
          >
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
              <option v-for="instr in INSTRUMENTS" :key="instr">
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
          <div
            class="alert alert-danger"
            ref="passworderror"
            style="display: none"
          ></div>
          <div
            class="alert alert-success"
            ref="passwordsuccess"
            style="display: none"
          >
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
import { INSTRUMENTS } from "../constants";
// @ts-ignore
import vSelect from "vue-select";
import { ref, computed, onMounted } from "vue";
import { ProfileData } from "./models";
import {
  defaultFormSend,
  getFromApi,
  postByObject,
} from "../services/apiservice";
import { slideUpAndDown } from "../services/slidehandler";

const profileData = ref<ProfileData | null>(null);
const password = ref("");
const confirmPass = ref("");
const formerror = ref<HTMLElement | null>(null);
const formsuccess = ref<HTMLElement | null>(null);
const formcontainer = ref<HTMLElement | null>(null);
const passworderror = ref<HTMLElement | null>(null);
const passwordsuccess = ref<HTMLElement | null>(null);

const othInstruments = computed(() => {
  return INSTRUMENTS.filter((instr) => {
    return instr !== profileData.value?.instrument;
  });
});

const updateProfile = async () => {
  postByObject(
    "/Profile/EditProfile",
    profileData.value,
    formerror.value,
    formsuccess.value,
    () => {
      formcontainer.value!.scrollIntoView();
    }
  );
};

const changePassword = async (event: Event) => {
  if (password.value !== confirmPass.value) {
    slideUpAndDown(passworderror.value!, 4000, "Lösenord matchar ej");
    return;
  }

  defaultFormSend(
    event.target as HTMLFormElement,
    passworderror.value,
    passwordsuccess.value,
    () => {
      password.value = "";
      confirmPass.value = "";
    }
  );
};

const loadData = async () => {
  profileData.value = await getFromApi<ProfileData>("/Profile/ProfileData");
};

onMounted(() => {
  loadData();
});
</script>
<style lang="scss"></style>
