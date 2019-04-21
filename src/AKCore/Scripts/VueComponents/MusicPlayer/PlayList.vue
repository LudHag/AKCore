
<template>
  <div class="player-module">
    <div class="playingnow" v-if="trackPlaying" v-html="nowPlayingText">Spelar nu: 03 Dansa i neon</div>
    <div class="controls" v-if="trackPlaying">
      <a
        href="#"
        @click.prevent="$emit('playpause')"
        v-if="playing"
        class="pauseplay glyphicon glyphicon-pause"
      ></a>
      <a
        href="#"
        @click.prevent="$emit('playpause')"
        v-else
        class="pauseplay glyphicon glyphicon-play"
      ></a>
      <div class="progress-container" ref="progress" @click.prevent="clickProgress">
        <div class="music-progress">
          <span class="music-progress-bar" :style="{ width: progress + '%' }"></span>
        </div>
      </div>
      <div class="progresstime">{{timeDisplay}}</div>
    </div>
    <audio class="player" v-if="trackPlaying" ref="player" :src="trackPlaying.filepath"></audio>
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
import { fmtMSS, nameCompare } from "../../utils/functions";
export default {
  props: ["playList", "playing", "album"],
  data() {
    return {
      trackLength: 0,
      timePlayed: 0,
      trackPlaying: null
    };
  },
  computed: {
    timeDisplay() {
      return fmtMSS(this.timePlayed) + "/" + fmtMSS(this.trackLength);
    },
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
    },
    nowPlayingText() {
      return "Spelar nu: " + this.trackPlaying.name;
    }
  },
  watch: {
    playing(val) {
      const player = this.$refs.player;
      if (val) {
        player.play();
        player.addEventListener("timeupdate", this.timeUpdate);
      } else {
        player.pause();
        player.removeEventListener("timeupdate", this.timeUpdate);
      }
    }
  },
  methods: {
    timeUpdate(event) {
      const player = this.$refs.player;
      this.trackLength = player.duration;
      this.timePlayed = event.target.currentTime;
    },
    clickProgress(event) {
      const progressContainer = this.$refs.progress;
      const player = this.$refs.player;
      const decimalProgress = event.offsetX / progressContainer.offsetWidth;
      const time = decimalProgress * player.duration;
      this.trackLength = player.duration;
      this.timePlayed = time;
      player.currentTime = time;
    },
    selectTrack(track) {
      this.trackPlaying = track;
      if (!this.playing) {
        this.$nextTick(() => this.$emit("playpause"));
      } else {
        const player = this.$refs.player;
        this.$nextTick(() => {
          player.load();
          player.play();
        });
      }
    }
  }
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.player-module {
  flex-grow: 1;
  padding-left: 30px;

  .controls {
    border: 1px solid $akred;
    border-radius: 7px;
    padding: 7px;
    margin-top: 3px;
    margin-bottom: 12px;
    display: flex;
    align-items: center;
  }
}

.pauseplay {
  margin-right: 7px;
}

.progress-container {
  flex-grow: 1;
  height: 15px;
  display: flex;
  align-items: center;
  cursor: pointer;
}

.progresstime {
  min-width: 50px;
  font-size: 12px;
  text-align: right;
  margin-left: 10px;
}

.music-progress {
  height: 3px;
  background-color: #c1bfbd;
  width: 100%;

  .music-progress-bar {
    background-color: #6f6f6f;
    display: block;
    height: 100%;
    transition: width ease 0.1s;
  }
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
</style>
