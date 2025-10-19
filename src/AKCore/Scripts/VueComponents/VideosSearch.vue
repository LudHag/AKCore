<template>
  <form class="form-inline videos-search">
    <div class="form-group">
      <a
        v-if="searchPhrase"
        href="#"
        @click.prevent="searchPhrase = ''"
        class="show-videos"
      >
        {{ t("show-all-videos") }}
      </a>
      <input
        type="text"
        class="form-control"
        v-model="searchPhrase"
        :placeholder="t('search-videos')"
      />
    </div>
  </form>
</template>
<script setup lang="ts">
import { computed } from "vue";
import { sharedVideoState } from "./sharedState";
import { TranslationDomain, translate } from "../translations";

const t = (key: string, domain: TranslationDomain = "videos") => {
  return translate(domain, key);
};

const searchPhrase = computed({
  get: () => sharedVideoState.searchPhrase,
  set: (value: string) => sharedVideoState.updateSearchPhrase(value),
});
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
.videos-search {
  display: flex;
  .form-group {
    margin-left: auto;
  }
}

.show-videos {
  margin-right: 10px;
  color: gray;
}

// Left align on extra small screens (xs)
@media screen and (max-width: $screen-xs-max) {
  .videos-search {
    .form-group {
      margin-left: 0;
    }
  }
}
</style>
