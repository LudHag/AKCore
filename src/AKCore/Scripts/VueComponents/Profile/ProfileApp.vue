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
        <section v-if="profileStatistics">
          <h3>{{ t("statisticsheader") }}</h3>
          <p>{{ t("statisticspreamble") }}</p>
          <ul>
            <li>
              {{ t("total-gigs") }}:
              <span class="bold">{{ profileStatistics.totalGigs }}</span>
            </li>
            <li>
              {{ t("halan") }}:
              <span class="bold">{{ profileStatistics.halan }}</span> -
              {{
                getPercentage(
                  profileStatistics.halan,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
            <li>
              {{ t("direct") }}:
              <span class="bold">{{ profileStatistics.direct }}</span> -
              {{
                getPercentage(
                  profileStatistics.direct,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
            <li>
              {{ t("cantCome") }}:
              <span class="bold">{{ profileStatistics.cantCome }}</span> -
              {{
                getPercentage(
                  profileStatistics.cantCome,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
            <li>
              {{ t("car") }}:
              <span class="bold">{{ profileStatistics.car }}</span> -
              {{
                getPercentage(
                  profileStatistics.car,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
            <li>
              {{ t("instrument-own") }}:
              <span class="bold">{{ profileStatistics.instrument }}</span> -
              {{
                getPercentage(
                  profileStatistics.instrument,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
            <li>
              {{ t("comment") }}:
              <span class="bold">{{ profileStatistics.comment }}</span> -
              {{
                getPercentage(
                  profileStatistics.comment,
                  profileStatistics.totalGigs,
                )
              }}%
            </li>
          </ul>
        </section>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import EditProfile from "./EditProfile.vue";
import ChangePassword from "./ChangePassword.vue";
import { ref, onMounted } from "vue";
import { ProfileData, ProfileStatistics } from "./models";
import { getFromApi } from "../../services/apiservice";
import { TranslationDomain, translate } from "../../translations";

const profileData = ref<ProfileData | null>(null);
const profileStatistics = ref<ProfileStatistics | null>(null);
const formcontainer = ref<HTMLElement | null>(null);

const updateProfile = async () => {
  formcontainer.value!.scrollIntoView();
};

const getPercentage = (value: number, total: number) => {
  return ((value / total) * 100).toFixed(2);
};

const loadData = async () => {
  profileData.value = await getFromApi<ProfileData>("/Profile/ProfileData");
  profileStatistics.value = await getFromApi<ProfileStatistics>(
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
