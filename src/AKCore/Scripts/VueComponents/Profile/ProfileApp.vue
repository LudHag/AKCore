<template>
  <div
    id="profile-app"
    v-if="profileData && profileData.userName !== 'nintendo'"
  >
    <div class="row" ref="formcontainer">
      <EditProfile :profile-data="profileData" @update="updateProfile" />
      <div class="col-md-6">
        <ChangePassword />
        <section>
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
        </section>
        <ProfileStatistics
          :profile-statistics="profileStatistics"
          v-if="profileStatistics"
        />
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import EditProfile from "./EditProfile.vue";
import ChangePassword from "./ChangePassword.vue";
import ProfileStatistics from "./ProfileStatistics.vue";
import { ref, onMounted } from "vue";
import { ProfileData, ProfileStatisticsModel } from "./models";
import { getFromApi } from "@services/apiservice";
import { TranslationDomain, translate } from "@scripts/translations";

const profileData = ref<ProfileData | null>(null);
const profileStatistics = ref<ProfileStatisticsModel | null>(null);
const formcontainer = ref<HTMLElement | null>(null);

const updateProfile = async () => {
  formcontainer.value!.scrollIntoView();
};

const loadData = async () => {
  profileData.value = await getFromApi<ProfileData>("/Profile/ProfileData");
  profileStatistics.value = await getFromApi<ProfileStatisticsModel>(
    "/Profile/Statistics",
  );
};

const t = (key: string, domain: TranslationDomain = "profile") => {
  return translate(domain, key);
};

onMounted(() => {
  loadData();
});
</script>
<style scoped lang="scss">
section {
  padding-bottom: 1rem;
}
.bold {
  font-weight: bold;
  color: #d74b4b;
}
</style>
