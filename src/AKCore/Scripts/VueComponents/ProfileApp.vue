<template>
  <div
    id="profile-app"
    v-if="profileData && profileData.userName !== 'nintendo'"
  >
    <div class="row" ref="formcontainer">
      <div class="col-md-6">
        <h3>{{ t("user-info") }}</h3>
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
            {{ t("profile-updated") }}
          </div>
          <div class="form-group">
            <label>{{ t("user-name", "common") }}</label>
            <input
              v-model="profileData.userName"
              type="text"
              class="form-control"
              :placeholder="t('user-name')"
              name="UserName"
            />
          </div>
          <div class="form-group">
            <label>{{ t("email") }}</label>
            <input
              v-model="profileData.email"
              class="form-control"
              :placeholder="t('email')"
              type="email"
              name="Email"
            />
          </div>
          <div class="form-group">
            <label>{{ t("first-name") }}</label>
            <input
              v-model="profileData.firstName"
              class="form-control"
              :placeholder="t('first-name')"
              name="FirstName"
            />
          </div>
          <div class="form-group">
            <label>{{ t("last-name") }}</label>
            <input
              v-model="profileData.lastName"
              class="form-control"
              :placeholder="t('last-name')"
              name="LastName"
            />
          </div>
          <div class="form-group">
            <label>{{ t("phone-number") }}</label>
            <input
              v-model="profileData.phone"
              class="form-control"
              :placeholder="t('phone-number')"
              name="Phone"
            />
          </div>
          <div class="form-group">
            <label>{{ t("instrument") }}</label>
            <select
              v-model="profileData.instrument"
              class="form-control"
              name="Instrument"
              required
            >
              <option value="">{{ t("select-instrument") }}</option>
              <option v-for="instr in INSTRUMENTS" :key="instr" :value="instr">
                {{ t(instr, "instruments") }}
              </option>
            </select>
          </div>
          <div class="form-group">
            <label>{{ t("other-instruments") }}</label>
            <v-select
              multiple
              :searchable="false"
              v-model="profileData.otherInstruments"
              :options="othInstruments"
            ></v-select>
          </div>
          <div class="form-group">
            <button type="submit" class="btn btn-default">
              {{ t("update-profile") }}
            </button>
          </div>
        </form>
      </div>
      <div class="col-md-6">
        <h3>{{ t("change-password") }}:</h3>
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
            {{ t("password-updated") }}
          </div>
          <div class="form-group">
            <label for="newpass">{{ t("new-password") }}:</label>
            <input
              v-model="password"
              type="password"
              class="form-control"
              name="password"
              :placeholder="t('new-password')"
              required
            />
          </div>
          <div class="form-group">
            <label for="confirmpass">{{ t("confirm-password") }}:</label>
            <input
              v-model="confirmPass"
              type="password"
              class="form-control"
              :placeholder="t('confirm-password')"
              required
            />
          </div>
          <div class="form-group">
            <button type="submit" class="btn btn-default">
              {{ t("update-password") }}
            </button>
          </div>
        </form>
        <div>
          <div v-if="profileData.roles && profileData.roles.length > 0">
            <h3>{{ t("roles") }}</h3>
            <div class="roles">
              <span class="role" :key="role" v-for="role in profileData.roles">
                {{ role }}
              </span>
            </div>
          </div>
          <div v-if="profileData.posts && profileData.posts.length > 0">
            <h3>{{ t("posts") }}</h3>
            <div class="roles">
              <span class="role" :key="post" v-for="post in profileData.posts">
                {{ post }}
              </span>
            </div>
          </div>
          <div v-if="profileData.medal">
            <h3>{{ t("latest-medal") }}</h3>
            <p>{{ profileData.medal }}</p>
          </div>
          <div v-if="profileData.givenMedal">
            <h3>{{ t("latest-medal-given") }}</h3>
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
import { TranslationDomain, translate } from "../translations";

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

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};

onMounted(() => {
  loadData();
});
</script>
<style lang="scss"></style>
