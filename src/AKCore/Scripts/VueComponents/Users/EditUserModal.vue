<template>
  <modal
    :show-modal="showModal"
    :header="user ? 'Redigera användare' : 'Skapa användare'"
    @close="close"
  >
    <template #body>
      <div class="modal-body">
        <form autocomplete="off" @submit.prevent="submitForm">
          <div class="row">
            <div class="col-sm-6">
              <div
                class="alert alert-danger"
                ref="error"
                style="display: none"
              ></div>
              <div class="form-group">
                <label>Förnamn</label>
                <input
                  type="text"
                  autocomplete="off"
                  class="form-control"
                  name="FirstName"
                  placeholder="Förnamn"
                  v-model="editedUser.firstName"
                />
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
                />
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
                />
              </div>
              <div class="form-group">
                <label>Matpreferens</label>
                <input
                  type="text"
                  autocomplete="off"
                  class="form-control"
                  name="FoodPreference"
                  placeholder="Matpreferens"
                  v-model="editedUser.foodPreference"
                />
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
                />
              </div>
              <div class="form-group">
                <label>Instrument</label>
                <select
                  class="form-control"
                  name="Instrument"
                  v-model="editedUser.instrument"
                >
                  <option value>Välj instrument</option>
                  <option :key="instr" v-for="instr in INSTRUMENTS">
                    {{ instr }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Andra instrument</label>
                <VueSelect
                  is-multi
                  :searchable="false"
                  name="OtherInstruments"
                  placeholder="Välj andra instrument"
                  :options="othInstruments"
                  v-model="editedUser.otherInstruments"
                />
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
                />
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
                />
              </div>
              <div class="form-group" v-if="!user">
                <label>Roller</label>
                <VueSelect
                  is-multi
                  :searchable="false"
                  name="Roles"
                  :options="roleOptions"
                  v-model="editedUser.roles"
                ></VueSelect>
              </div>
              <div class="form-group">
                <label>Poster</label>
                <VueSelect
                  is-multi
                  name="Poster"
                  placeholder="Välj poster"
                  :options="postOptions"
                  v-model="editedUser.posts"
                ></VueSelect>
              </div>
              <div class="form-group">
                <label>Senaste medalj</label>
                <select
                  class="form-control"
                  name="Medal"
                  v-model="editedUser.medal"
                >
                  <option value>Ingen medalj</option>
                  <option :key="medal" v-for="medal in MEDALS">
                    {{ medal }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Senast utdelade medalj</label>
                <select
                  class="form-control"
                  name="GivenMedal"
                  v-model="editedUser.givenMedal"
                >
                  <option value>Ingen medalj</option>
                  <option :key="medal" v-for="medal in MEDALS">
                    {{ medal }}
                  </option>
                </select>
              </div>
            </div>
          </div>
          <div class="modal-footer">
            <button type="submit" class="btn btn-primary">Spara</button>
          </div>
        </form>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { postToApi } from "../../services/apiservice";
import Modal from "../Modal.vue";
import { INSTRUMENTS, ROLES, POSTS, MEDALS } from "../../constants";
import VueSelect, { Option } from "vue3-select-component";
import { User } from "./models";
import { ref, watch, computed } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "updated", user: User): void;
  (e: "created", user: User): void;
}>();

const props = defineProps<{
  user: User | null;
  showModal: boolean;
}>();

const editedUser = ref<User>({} as User);
const error = ref<HTMLElement | null>(null);

const postOptions = POSTS.map((post) => {
  return { value: post, label: post } as Option<string>;
});

const roleOptions = ROLES.map((role) => {
  return { value: role, label: role } as Option<string>;
});

watch(
  () => props.showModal,
  () => {
    const newUser = props.user || ({} as User);
    if (!props.user) {
      newUser.roles = ["Medlem"];
      newUser.posts = [];
      newUser.otherInstruments = [];
    }
    editedUser.value = Object.assign({}, newUser);
  },
);

const othInstruments = computed(() => {
  return INSTRUMENTS.filter((instr) => {
    return instr !== editedUser.value.instrument;
  }).map((instr) => {
    return { value: instr, label: instr } as Option<string>;
  });
});

const close = () => {
  emit("close");
};

const submitForm = () => {
  if (props.user) {
    postToApi("/User/EditUser", editedUser.value, error.value, null, () => {
      emit("updated", editedUser.value);
    });
  } else {
    postToApi("/User/CreateUser", editedUser.value, error.value, null, () => {
      emit("created", Object.assign({}, editedUser.value));
    });
  }
};
</script>
<style lang="scss" scoped></style>
