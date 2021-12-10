<template>
  <div class="album-list">
    <div class="form-inline track-search">
      <div class="form-group">
        <a v-if="searchQuery" href="#" @click.prevent="searchQuery = ''" class="show-albums">Visa album</a>
        Sök låtar:
        <input
          type="text"
          v-model="searchQuery"
          class="form-control"
          placeholder="Sök här"
        >
      </div>
    </div>
    <div class="categories" v-if="!filteredAlbums">
      <div class="category" v-for="category in categoryList" :key="category[0].category">
        <h2 v-if="categoryList.length > 1">{{category[0].category}}</h2>
        <div class="category-list" :class="{'no-cats': categoryList.length === 1}">
          <div class="album-element" v-for="album in category" :key="album.id">
            <a class="album-link" href="#" @click.prevent="$emit('select', album)">
              <img class="album-img" :src="album.image">
              <p class="album-name">{{album.name}}</p>
            </a>
          </div>
        </div>
      </div>
    </div>
    <div class="tracks" v-if="filteredAlbums">
      <div v-for="album in filteredAlbums" :key="album.id">
        <h2>{{album.name}}</h2>
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
<script>
import { groupBy, nameCompare } from "../../utils/functions";

export default {
  props: ["albums"],
  data() {
    return {
      categoriesEnabled: true,
      searchQuery: ""
    };
  },
  computed: {
    categoryList() {
      const keys = Object.keys(this.albums);
      const albums = keys
        .map(key => {
          return Object.assign(
            { id: key, category: "Övrigt" },
            this.albums[key]
          );
        })
        .sort(nameCompare);
      if (!this.categoriesEnabled || keys.length < 6) {
        return [albums];
      }
      const categories = groupBy(albums, "category");
      return Object.keys(categories)
        .sort()
        .map(cat => categories[cat]);
    },
    filteredAlbums() {
      if (!this.searchQuery) {
        return null;
      }
      const lowerQuery = this.searchQuery.toLowerCase();
      let tracks = [];
      const keys = Object.keys(this.albums);
      const albums = keys
        .map(key => {
          const trackKeys = Object.keys(this.albums[key].tracks);
          const filteredTracks = trackKeys
            .map(tKey => this.albums[key].tracks[tKey])
            .filter(track => track.name.toLowerCase().indexOf(lowerQuery) > -1);
          return Object.assign({}, this.albums[key], {
            tracks: filteredTracks
          });
        })
        .filter(album => album.tracks.length > 0);
      return albums;
    }
  }
};
</script>
<style lang="scss" scoped>
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
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
