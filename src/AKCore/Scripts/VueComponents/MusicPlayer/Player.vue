<template>
  <div>
    <div class="playingnow" v-if="trackPlaying" v-html="nowPlayingText"></div>
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
      <div
        class="progress-container"
        ref="progress"
        @click.prevent="clickProgress"
      >
        <div class="music-progress">
          <span
            class="music-progress-bar"
            :style="{ width: progress + '%' }"
          ></span>
        </div>
      </div>
      <div class="progresstime">{{ timeDisplay }}</div>
      <a
        href="#"
        @click.prevent="$emit('replay')"
        :class="{ active: replay }"
        class="replay glyphicon glyphicon-repeat"
      ></a>
    </div>
    <audio
      class="player"
      v-if="trackPlaying"
      ref="player"
      :src="trackPlaying.filepath"
    ></audio>
  </div>
</template>
<script>
import { fmtMSS } from "../../utils/functions";
export default {
  props: ["playing", "trackPlaying", "reset", "replay"],
  data() {
    return {
      trackLength: 0,
      timePlayed: 0,
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
    nowPlayingText() {
      return "Spelar nu: " + this.trackPlaying.name;
    },
  },
  watch: {
    playing(val) {
      const player = this.$refs.player;
      if (val) {
        player.play();
        player.addEventListener("timeupdate", this.timeUpdate);
        player.addEventListener("ended", this.endedListener);
      } else {
        player.pause();
        player.removeEventListener("timeupdate", this.timeUpdate);
        player.removeEventListener("ended", this.endedListener);
      }
    },
    trackPlaying() {
      if (this.playing) {
        const player = this.$refs.player;
        this.$nextTick(() => {
          player.load();
          player.play();
        });
      }
    },
    reset(val) {
      if (val) {
        const player = this.$refs.player;
        this.trackLength = player ? player.duration : 0;
        this.timePlayed = 0;
        if (player) {
          player.currentTime = 0;
        }
      }
    },
  },
  methods: {
    timeUpdate(event) {
      const player = this.$refs.player;
      if (player) {
        this.trackLength = player.duration;
        this.timePlayed = event.target.currentTime;
      }
    },
    endedListener() {
      this.trackLength = 0;
      this.timePlayed = 0;
      this.next();
    },
    next() {
      this.$emit("next");
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
  },
};
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.controls {
  border: 1px solid $akred;
  border-radius: 7px;
  padding: 7px;
  margin-top: 3px;
  margin-bottom: 12px;
  display: flex;
  align-items: center;
}

.pauseplay {
  margin-right: 7px;
}
.replay {
  margin-left: 7px;
  color: #6f6f6f;
  &.active {
    color: $akred;
  }
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

@media screen and (max-width: $screen-xs-max) {
  .player {
    width: 100%;
  }
}
</style>
