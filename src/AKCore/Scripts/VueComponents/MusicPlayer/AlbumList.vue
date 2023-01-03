<template>
  <div class="album-list">
    <div class="form-inline track-search">
      <div class="form-group">
        <a
          v-if="searchQuery"
          href="#"
          @click.prevent="searchQuery = ''"
          class="show-albums"
          >Visa album</a
        >
        Sök låtar:
        <input
          type="text"
          v-model="searchQuery"
          class="form-control"
          placeholder="Sök här"
        />
      </div>
    </div>
    <div class="categories" v-if="!filteredAlbums">
      <div
        class="category"
        v-for="category in categoryList"
        :key="category[0].category"
      >
        <h2 v-if="categoryList.length > 1">{{ category[0].category }}</h2>
        <div
          class="category-list"
          :class="{ 'no-cats': categoryList.length === 1 }"
        >
          <div class="album-element" v-for="album in category" :key="album.id">
            <a
              class="album-link"
              href="#"
              @click.prevent="$emit('select', album)"
            >
              <img class="album-img" :src="album.image" />
              <p class="album-name">{{ album.name }}</p>
            </a>
          </div>
        </div>
      </div>
    </div>
    <div class="tracks" v-if="filteredAlbums">
      <div v-for="album in filteredAlbums" :key="album.id">
        <h2>{{ album.name }}</h2>
        <a
          href="#"
          class="track"
          @click.prevent="$emit('add-track', track)"
          v-for="track in album.tracks"
          :key="track.id"
          v-html="track.name"
        ></a>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from "vue";
import { groupBy, nameCompare } from "../../utils/functions";
import { Album, Track } from "./models";

const { albums } = defineProps<{ albums: Album[] }>();

defineEmits<{
  (e: "select", album: Album): void;
  (e: "add-track", track: Track): void;
}>();

const categoriesEnabled = ref(true);
const searchQuery = ref("");

const categoryList = computed(() => {
  const enrichedAlbums = albums
    .map((album) => {
      if (album.category) {
        return album;
      }
      return { ...album, category: "Övrigt" };
    })
    .sort(nameCompare);
  if (!categoriesEnabled.value || albums.length < 6) {
    return [albums];
  }
  const categories = groupBy(enrichedAlbums, "category");
  return Object.keys(categories)
    .sort()
    .map((cat) => categories[cat]);
});

const filteredAlbums = computed(() => {
  if (!searchQuery.value) {
    return null;
  }
  const lowerQuery = searchQuery.value.toLowerCase();
  const filteredAlbums = albums
    .map((album) => {
      const filteredTracks = album.tracks.filter(
        (track) => track.name.toLowerCase().indexOf(lowerQuery) > -1
      );
      return { ...album, tracks: filteredTracks };
    })
    .filter((album) => album.tracks.length > 0);
  return filteredAlbums;
});
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
.album-list {
  margin-top: 10px;
  position: relative;
  padding-top: 20px;
}
.show-albums {
  margin-right: 20px;
  color: gray;
}
.categories {
  margin-top: 10px;
}
.track-search {
  position: absolute;
  top: 0;
  right: 0;
}
.tracks {
  column-count: 2;
  padding-top: 40px;
  h2 {
    color: #999;
    font-size: 17px;
  }
}
.track {
  display: block;
}
.category-list {
  list-style-type: none;
  padding-left: 0;
  display: flex;
  flex-wrap: wrap;
  margin-top: 5px;
  justify-content: left;
  &.no-cats {
    justify-content: center;
  }
}

.album-element {
  padding: 10px 10px;
  width: 130px;
  position: relative;
  .album-img {
    height: 80px;
    max-width: 80px;
  }
  a {
    width: 100%;
    display: block;
    text-align: center;
  }
}
h2 {
  font-size: 20px;
  line-height: 30px;
}
.album-name {
  text-align: center;
  font-size: 14px;
  margin-top: 5px;
}

@media screen and (max-width: $screen-xs-max) {
  .album-element {
    width: 125px;
  }
  .categories {
    padding-top: 20px;
  }
  .tracks {
    padding-top: 65px;
  }
}
</style>
