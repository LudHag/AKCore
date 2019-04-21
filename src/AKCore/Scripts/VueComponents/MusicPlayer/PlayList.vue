
<template>
  <div class="player-module">
    <Player
      :playing="playing"
      :track-playing="trackPlaying"
      @playpause="$emit('playpause')"
      @next="next"
    ></Player>
    <div class="playlist">
      <a
        :href="track.filepath"
        class="playlist-element"
        :class="{'active': trackPlaying && track.id === trackPlaying.id}"
        v-for="track in tracks"
        :key="track.id"
        @click.prevent="selectTrack(track)"
      >
        <span class="name" v-html="track.name"></span>
        <span class="glyphicon glyphicon-download"></span>
      </a>
    </div>
  </div>
</template>
<script>
import { nameCompare } from "../../utils/functions";
import Player from "./Player";
export default {
  props: ["playList", "playing", "album"],
  components: {
    Player
  },
  data() {
    return {
      trackPlaying: null
    };
  },
  computed: {
    progress() {
      if (this.timePlayed === 0 || this.trackLength === 0) {
        return 0;
      }
      return (this.timePlayed / this.trackLength) * 100;
    },
    tracks() {
      if (this.playList.length > 0) {
        return this.playList;
      }
      const trackKeys = Object.keys(this.album.tracks);
      return trackKeys.map(key => this.album.tracks[key]).sort(nameCompare);
    }
  },
  methods: {
    next() {
      const currentIndex = this.tracks.findIndex(
        track => track.id === this.trackPlaying.id
      );
      if (currentIndex === -1 || currentIndex + 1 >= this.tracks.length) {
        this.$emit("stop");
        this.trackPlaying = null;
      } else {
        this.trackPlaying = this.tracks[currentIndex + 1];
      }
    },
    selectTrack(track) {
      this.trackPlaying = track;
      if (!this.playing) {
        this.$nextTick(() => this.$emit("playpause"));
      }
    }
  }
};
</script>
<style lang="scss" scoped>
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.player-module {
  flex-grow: 1;
  padding-left: 30px;
}
.playlist {
  height: 210px;
  overflow: auto;
}
.playlist-element {
  display: block;
  color: #a5a2a0;
  padding: 3px 0;
  font-size: 16px;

  &.queued {
    color: #2c7b12;
  }

  &.active {
    color: $akred;
  }

  .glyphicon-download {
    float: right;
    margin-right: 30px;
  }
}
.player-module {
  ::-webkit-scrollbar {
    width: 12px;
  }

  ::-webkit-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, 0.3);
    border-radius: 10px;
  }

  ::-webkit-scrollbar-thumb {
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px #a5a2a0;
  }

  ::-moz-scrollbar {
    width: 12px;
  }

  ::-moz-scrollbar-track {
    -webkit-box-shadow: inset 0 0 6px rgba(0, 0, 0, 0.3);
    border-radius: 10px;
  }

  ::-webkit-scrollbar-thumb {
    border-radius: 10px;
    -webkit-box-shadow: inset 0 0 6px #a5a2a0;
  }
}

@media screen and (max-width: $screen-xs-max) {
  .player-module {
    padding-top: 30px;
    padding-left: 0;
  }

  .playlist-element {
    position: relative;

    .name {
      width: 82%;
      display: inline-block;
    }

    .glyphicon-download {
      position: absolute;
      top: 50%;
      transform: translateY(-50%);
    }
  }
}
</style>
