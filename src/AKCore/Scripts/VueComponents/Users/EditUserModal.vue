<template>
  <modal :show-modal="showModal" :header="'Redigera användare'" @close="close">
    <div slot="body" class="modal-body">
      <form autocomplete="off" @submit.prevent="submitForm">
        <div class="row">
          <div class="col-sm-6">
            <div class="form-group">
              <label>Förnamn</label>
              <input
                type="text"
                autocomplete="off"
                class="form-control"
                name="FirstName"
                placeholder="Förnamn"
                v-model="editedUser.firstName"
              >
            </div>
            <div class="form-group">
              <label>Efternamn</label>
              <input
                type="text"
                autocomplete="off"
                class="form-control"
                name="LastName"
                placeholder="Efternamn"
                v-model="editedUser.lastName"
              >
            </div>
            <div class="form-group">
              <label>Telefonnummer</label>
              <input
                type="text"
                autocomplete="off"
                class="form-control"
                name="Phone"
                placeholder="Telefonnummer"
                v-model="editedUser.phone"
              >
            </div>
            <div class="form-group">
              <label>Epost</label>
              <input
                type="email"
                autocomplete="off"
                class="form-control"
                name="Email"
                placeholder="Epost"
                v-model="editedUser.email"
              >
            </div>
            <div class="form-group">
              <label>Instrument</label>
              <select class="form-control" name="Instrument" v-model="editedUser.instrument">
                <option value="">Välj instrument</option>
                <option :key="instr" v-for="instr in instruments">{{instr}}</option>
              </select>
            </div>
            <div class="form-group">
              <label>Andra instrument</label>
              <v-select multiple :searchable="false" name="OtherInstruments" :options="othInstruments" v-model="editedUser.otherInstruments"></v-select>
            </div>
          </div>

          <div class="col-sm-6">
            <div class="form-group">
              <label>Användarnamn</label>
              <input
                type="text"
                autocomplete="off"
                class="form-control"
                name="UserName"
                placeholder="Användarnamn"
                required
                v-model="editedUser.userName"
              >
            </div>
            <div class="form-group" v-if="!user">
              <label>Lösenord</label>
              <input
                type="password"
                autocomplete="off"
                class="form-control"
                name="Password"
                placeholder="Lösenord"
                required
                v-model="editedUser.password"
              >
            </div>
            <div class="form-group">
              <label>Roller</label>
              <v-select multiple :searchable="false" name="Roles" :options="roles" v-model="editedUser.roles"></v-select>
            </div>
            <div class="form-group">
              <label>Poster</label>
              <v-select multiple :searchable="false" name="Poster" :options="posts" v-model="editedUser.posts"></v-select>
            </div>
            <div class="form-group">
              <label>Senaste medalj</label>
              <select class="form-control" name="Medal" v-model="editedUser.medal">
                <option value>Ingen medalj</option>
                <option :key="medal" v-for="medal in medals">{{medal}}</option>
              </select>
            </div>
            <div class="form-group">
              <label>Senast utdelade medalj</label>
              <select class="form-control" name="GivenMedal" v-model="editedUser.givenMedal">
                <option value>Ingen medalj</option>
                <option :key="medal" v-for="medal in medals">{{medal}}</option>
              </select>
            </div>
          </div>
        </div>
      </form>
    </div>
  </modal>
</template>
<script>
import ApiService from "../../services/apiservice";
import Modal from "../Modal";
import Constants from "../../constants";
import vSelect from 'vue-select';

export default {
  props: ["user", "showModal"],
  components: {
    Modal,
    vSelect
  },
  data() {
    return {
      editedUser: {}
    };
  },
  watch: {
    user() {
      this.editedUser = this.user || {};
    }
  },
  computed: {
    instruments() {
      return Constants.INSTRUMENTS;
    },
    roles() {
      return Constants.ROLES;
    },
    posts() {
      return Constants.POSTS;
    },
    medals() {
      return Constants.MEDALS;
    },
    othInstruments() {
        return Constants.INSTRUMENTS.filter((instr) => {
            return instr !== this.editedUser.instrument;
        });
    }
  },
  methods: {
    close() {
      this.$emit("close");
    },
    submitForm() {
      console.log("woop");
    }
  }
};
</script>
<style lang="scss" scoped>
</style>
