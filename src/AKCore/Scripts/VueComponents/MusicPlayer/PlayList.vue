
<template>
  <div class="player-module">
    <div class="playingnow">Spelar nu: 03 Dansa i neon</div>
    <div class="controls">
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
    <audio class="player" ref="player" src="/albums/2/03_Dansa_i_neon.mp3"></audio>
    <div class="playlist">
      <a href="/albums/2/01_Intro(holy_grail)_och_svenskt_flyg.mp3" class="playlist-element">
        <span class="name">01 Intro(holy grail) och svenskt flyg</span>
        <span class="glyphicon glyphicon-download"></span>
      </a>
    </div>
  </div>
</template>
<script>
import { fmtMSS } from "../../utils/functions";
export default {
  props: ["playList", "playing"],
  data() {
    return {
      trackLength: 0,
      timePlayed: 0
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
    }
  },
  watch: {
    playing(val) {
      if (val) {
        this.play();
      } else {
        this.pause();
      }
    }
  },
  methods: {
    play() {
      const player = this.$refs.player;
      player.play();
      player.addEventListener("timeupdate", this.timeUpdate);
    },
    pause() {
      const player = this.$refs.player;
      player.pause();
      player.removeEventListener("timeupdate", this.timeUpdate);
    },
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
</style>
