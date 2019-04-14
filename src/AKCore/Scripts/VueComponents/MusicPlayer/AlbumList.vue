<template>
  <div class="album-list">
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
</template>
<script>
import { groupBy, nameCompare } from "../../utils/functions";

export default {
  props: ["albums"],
   data() {
    return {
      categoriesEnabled: true
    };
  },
  computed: {
    categoryList() {
      const keys = Object.keys(this.albums);
      const albums = keys.map(key => {
        return Object.assign({ id: key, category: "Övrigt" }, this.albums[key]);
      }).sort(nameCompare);
      window.hej = albums;
      if(!this.categoriesEnabled || keys.length < 6) {
        return [albums];
      }
      const categories = groupBy(albums, "category");
      return Object.keys(categories)
        .sort()
        .map(cat => categories[cat]);
    }
  }
};
</script>
<style lang="scss" scoped>
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
</style>
