<template>
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
      <div class="alert alert-success" ref="formsuccess" style="display: none">
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
</template>
<script setup lang="ts">
import { INSTRUMENTS } from "../../constants";
// @ts-ignore
import vSelect from "vue-select";
import { ref, computed } from "vue";
import { ProfileData } from "./models";
import { postByObject } from "../../services/apiservice";
import { TranslationDomain, translate } from "../../translations";

const emit = defineEmits<{
  (e: "update"): void;
}>();

const props = defineProps<{ profileData: ProfileData }>();
const { profileData } = props;

const formerror = ref<HTMLElement | null>(null);
const formsuccess = ref<HTMLElement | null>(null);

const othInstruments = computed(() => {
  return INSTRUMENTS.filter((instr) => {
    return instr !== profileData?.instrument;
  });
});

const updateProfile = async () => {
  postByObject(
    "/Profile/EditProfile",
    profileData,
    formerror.value,
    formsuccess.value,
    () => {
      emit("update");
    },
  );
};

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};
</script>
<style lang="scss"></style>
