<template>
    <div class="row">
        <div class="col-sm-2 image">
            <img class="album-img" :src="album.image"/>
        </div>
        <div class="col-sm-4 name">
            <input ref="inputelement" v-if="editName" type="text" class="name-input" v-model="name" @keyup.enter="onInputBlur" @blur="onInputBlur">
            <span class="album-name" v-if="!editName" @click="showInput">{{name}}</span>
        </div>
        <div class="col-sm-2 actions">
            <a href="#" class="del-album btn glyphicon glyphicon-remove"></a>
        </div>
        <div class="col-sm-4 tracks">
            <span class="tracks-info">
                {{tracks}} spår uppladdade.<br>
                Klicka här för att hantera.
            </span>
        </div>
    </div>
</template>
<script>
import ApiService from "../../services/apiservice";

export default {
  props: ["album"],
  data() {
    return {
      editName: false,
      name: ""
    };
  },
  methods: {
    showInput() {
        this.editName = true;
        this.$nextTick(() => this.$refs.inputelement.focus());
    },
    onInputBlur() {
        if(this.editName){
            this.editName = false;
            if(this.album.name !== this.name) {
                this.$emit("name", this.name, this.album.id);
            }
        }
    }
  },
  computed: {
    tracks() {
      return this.album.tracks.length;
    }
  },
  created() {
      this.name = this.album.name;
  }
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
.row:hover {
  background-color: #1a0000;
}

.image,
.name,
.actions,
.tracks {
  height: 100px;
  vertical-align: middle;
}

.actions,
.image,
.tracks {
  display: flex;
  align-items: center;
}

.tracks {
  cursor: pointer;
}

.name {
  font-size: 20px;
  display: flex;
  align-items: center;

  .name-input {
    background-color: transparent;
    border: 0;
    outline: 0;

    &:focus {
      background: $akwhite;
      color: #000;
    }
  }
}

.del-album {
  font-size: 20px;
  color: red;
}

.album-img {
  height: 100px;
  width: 100px;
}
.album-img{
    height: 100px;
    max-width: 100px;
}
</style>
