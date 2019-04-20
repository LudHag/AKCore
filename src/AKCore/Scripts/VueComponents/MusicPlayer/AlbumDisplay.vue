
<template>
      <div class="album-display" v-if="album">
        <img class="album-display-image" :src="album.image">
        <a href="#" class="play-all" @click.prevent="$emit('playall')">
          Spela alla spår
          <span class="glyphicon glyphicon-play"></span>
        </a>
      </div>
</template>
<script>

export default {
  props: ["album"]
};
</script>
<style lang="scss" scoped>
.album-display {
  flex-grow: 1;
  text-align: center;
  max-width: 290px;
  margin: auto;
}

.play-all {
  display: block;
  margin-top: 10px;
  font-size: 16px;
  font-weight: 800;
}

.album-display-image {
  height: 220px;
  max-width: 220px;
}
</style>
