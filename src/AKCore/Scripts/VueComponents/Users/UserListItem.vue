<template>
  <tbody>
    <tr class="clickable hover-grey" @click.prevent="expanded = !expanded">
      <td>{{ user.fullName }}</td>
      <td>{{ user.userName }}</td>
      <td>
        <div class="roles">
          <span
            v-for="role in user.roles"
            :key="role"
            class="role hover-tooltip"
            :data-tooltip="roleInfo(role)"
          >
            {{ role }}
            <a
              v-if="expanded"
              class="remove-role glyphicon glyphicon-remove"
              @click.prevent.stop="removeRole(role)"
            ></a>
          </span>
        </div>
      </td>
      <td>
        <span :class="{ inactive: userInactive }"
          >{{ user.lastSignedIn }}
        </span>
      </td>
      <td class="item-actions">
        <a
          class="btn remove-user glyphicon glyphicon-remove"
          @click.prevent.stop="removeUser"
        ></a>
      </td>
    </tr>
    <tr class="user-edit-container" v-if="expanded">
      <td colspan="5">
        <div class="user-edit">
          <div class="user-edit-area row">
            <div class="col-sm-6">
              <p>Användarinfo:</p>
              <div class="edit-group">
                <p><strong>Användarnamn:</strong> {{ user.userName }}</p>
                <p>
                  <strong>Fullt namn:</strong> {{ user.firstName }}
                  {{ user.lastName }}
                </p>
                <p><strong>Instrument:</strong> {{ user.instrument }}</p>
                <p>
                  <strong>Slavposter:</strong>
                  <span
                    class="listed-items"
                    :key="post"
                    v-for="post in user.posts"
                  >
                    {{ post }}
                  </span>
                </p>
                <form
                  class="form-inline save-medal"
                  method="post"
                  action="/User/SaveMedal"
                  @submit.prevent="saveLastEarned"
                >
                  <div class="form-group">
                    <strong class="spacing-right"
                      >Senast förtjänade medalj: {{ user.medal }}</strong
                    >
                    <input
                      type="hidden"
                      name="userName"
                      :value="user.userName"
                    />
                    <select
                      class="form-control input-sm spacing-right"
                      name="medal"
                      :value="user.medal"
                    >
                      <option value="">Ingen</option>
                      <option
                        v-for="medal in MEDALS"
                        :key="medal"
                        :value="medal"
                      >
                        {{ medal }}
                      </option>
                    </select>
                  </div>
                  <div class="form-group">
                    <button type="submit" class="btn btn-primary input-sm">
                      Spara
                    </button>
                  </div>
                </form>
                <form
                  class="form-inline save-medal"
                  method="post"
                  action="/User/SaveGivenMedal"
                  @submit.prevent="saveLastGiven"
                >
                  <div class="form-group">
                    <strong class="spacing-right">
                      Senast utdelad medalj: {{ user.givenMedal }}
                    </strong>
                    <input
                      type="hidden"
                      name="userName"
                      :value="user.userName"
                    />
                    <select
                      class="form-control input-sm spacing-right"
                      name="medal"
                      :value="user.givenMedal"
                    >
                      <option value="">Ingen</option>
                      <option
                        v-for="medal in MEDALS"
                        :key="medal"
                        :value="medal"
                      >
                        {{ medal }}
                      </option>
                    </select>
                  </div>
                  <div class="form-group">
                    <button type="submit" class="btn btn-primary input-sm">
                      Spara
                    </button>
                  </div>
                </form>
                <a
                  href="#"
                  class="btn btn-default edit-user-info"
                  @click.prevent="$emit('edit')"
                >
                  Redigera användarinfo
                </a>
              </div>
            </div>
            <div class="col-sm-6">
              <p>
                Ändra användarinställningar:
                <span
                  class="hover-tooltip"
                  data-tooltip="Supernintendo: Full access Editor: Kan redigera sidor, filer, album samt se intresseanmlningar Medlem: Kan anmäla sig till spelningar samt redigera sin info Balett: Kan se balettsidor"
                >
                  <i
                    class="fa fa-question-circle roles-info"
                    data-html="true"
                    aria-hidden="true"
                  ></i>
                </span>
              </p>
              <div class="edit-group">
                <form
                  class="form-inline add-role"
                  action="/User/AddRole"
                  method="POST"
                  @submit.prevent="addRole"
                >
                  <div class="form-group">
                    <input
                      type="hidden"
                      name="UserName"
                      :value="user.userName"
                    />
                    <label class="spacing-right">Lägg till roll: </label>
                    <select
                      class="form-control input-sm spacing-right"
                      name="Role"
                    >
                      <option value="">Välj roll</option>
                      <option v-for="role in roles" :key="role" :value="role">
                        {{ role }}
                      </option>
                    </select>
                  </div>
                  <div class="form-group">
                    <button type="submit" class="btn btn-primary input-sm">
                      Lägg till
                    </button>
                  </div>
                </form>
              </div>
              <div class="edit-group">
                <a
                  href="#"
                  class="btn btn-primary reset-pass-btn"
                  @click.prevent="resetPassword"
                >
                  Nytt lösenord
                </a>
              </div>
              <div class="edit-group">
                <label>Lägg till post(er): </label>
                <form
                  class="form-inline add-post"
                  action="/User/AddPost"
                  method="POST"
                  @submit.prevent="addPost"
                >
                  <VueSelect
                    is-multi
                    :searchable="false"
                    v-model="selectedPosts"
                    :options="postOptions"
                  />
                  <div class="form-group">
                    <button
                      type="reset"
                      @click.prevent="clearPosts"
                      class="btn btn-primary input-sm spacing-right"
                    >
                      Rensa
                    </button>
                    <button type="submit" class="btn btn-primary input-sm">
                      Spara
                    </button>
                  </div>
                </form>
              </div>
            </div>
          </div>
        </div>
      </td>
    </tr>
  </tbody>
</template>
<script setup lang="ts">
import { MEDALS, POSTS, ROLES } from "../../constants";
import VueSelect, { Option } from "vue3-select-component";
import {
  defaultFormSend,
  postByObject,
  postToApi,
} from "../../services/apiservice";
import { UpdateInfo, User } from "./models";
import { computed, ref, watch } from "vue";

const emit = defineEmits<{
  (e: "removeuser", userName: string): void;
  (e: "updateuserprop", updateInfo: UpdateInfo): void;
  (e: "newpassword", user: User): void;
  (e: "edit"): void;
}>();

const props = defineProps<{
  user: User;
}>();

const expanded = ref(false);
const selectedPosts = ref<string[]>([]);
const error = document.getElementsByClassName("alert-danger")[0] as HTMLElement;
const success = document.getElementsByClassName(
  "alert-success",
)[0] as HTMLElement;

const userInactive = ref(false);
const date = new Date();
date.setFullYear(date.getFullYear() - 1);
userInactive.value = Date.parse(props.user.lastSignedIn) - date.valueOf() < 0;

const postOptions = POSTS.filter(Boolean).map((post) => {
  return { value: post, label: post } as Option<string>;
});

watch(
  () => props.user,
  (user) => {
    if (user && user.posts) {
      selectedPosts.value = user.posts.slice();
    }
  },
);

watch(
  () => expanded.value,
  (val) => {
    if (val && props.user && props.user.posts) {
      selectedPosts.value = props.user.posts.slice();
    }
  },
);

const roleInfo = (role: string) => {
  switch (role) {
    case "SuperNintendo":
      return "Har rättigheter att redigera all information på webben";
    case "Editor":
      return "Kan redigera sidor, musikalbum, ladda upp filer samt titta på intresseanmälningar";
    case "Medlem":
      return "Kan anmäla sig till spelningar, syns i adressregistret samt kan redigera sin profil";
    case "Balett":
      return "Kan se balettsidor";
  }
  return "";
};

const removeUser = () => {
  if (confirm("Vill du verkligen ta bort " + props.user.fullName + "?")) {
    postToApi(
      "/User/RemoveUser?userName=" + props.user.userName,
      null,
      error,
      success,
      () => {
        emit("removeuser", props.user.userName);
      },
    );
  }
};

const saveLastEarned = (event: Event) => {
  defaultFormSend(event.target as HTMLFormElement, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "medal",
      // @ts-ignore
      value: (event.target as HTMLFormElement).elements.medal.value,
    });
  });
};

const saveLastGiven = (event: Event) => {
  defaultFormSend(event.target as HTMLFormElement, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "givenMedal",
      // @ts-ignore
      value: (event.target as HTMLFormElement).elements.medal.value,
    });
  });
};

const removeRole = (role: string) => {
  const roleIndex = props.user.roles.indexOf(role);
  if (roleIndex === -1) {
    return;
  }
  const newRoles = props.user.roles.slice();
  newRoles.splice(roleIndex, 1);

  postToApi(
    "/User/RemoveRole?UserName=" + props.user.userName + "&Role=" + role,
    null,
    error,
    success,
    () => {
      emit("updateuserprop", {
        userName: props.user.userName,
        prop: "roles",
        value: newRoles,
      });
    },
  );
};

const addRole = (event: Event) => {
  // @ts-ignore
  const role = (event.target as HTMLFormElement).elements.Role.value;
  const roleIndex = props.user.roles.indexOf(role);
  if (roleIndex !== -1) {
    return;
  }
  const newRoles = props.user.roles.slice();
  newRoles.push(role);
  defaultFormSend(event.target as HTMLFormElement, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "roles",
      value: newRoles,
    });
  });
};

const resetPassword = () => {
  emit("newpassword", props.user);
};

const addPost = () => {
  const postObj = {
    post: selectedPosts.value,
    userName: props.user.userName,
  };
  postByObject("/User/AddPost", postObj, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "posts",
      value: selectedPosts.value.slice(),
    });
  });
};

const clearPosts = () => {
  selectedPosts.value = [];
};

const roles = computed(() => {
  return ROLES.filter((role) => {
    return props.user.roles.indexOf(role) == -1;
  });
});
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";

.table > tbody + tbody {
  border-top: 0;
}

.remove-role {
  color: $akwhite;
  font-size: 12px;
}
.listed-items {
  margin-left: 5px;
}
.item-actions {
  text-align: right;
}

.inactive {
  color: orangered;
}

.hover-tooltip {
  position: relative;
  display: inline-block;
  cursor: default;
  line-height: 1.2em;
}

.hover-tooltip:hover {
  &:before {
    content: attr(data-tooltip);
    position: absolute;
    bottom: 35px;
    left: 0;
    width: 255px;
    background: $akwhite;
    color: #555555;
    padding: 5px;
    border-radius: 5px;
    z-index: 100;
  }
  &:after {
    content: "";
    position: absolute;
    left: 5px;
    bottom: 15px;
    border: 10px solid $akwhite;
    border-color: $akwhite transparent transparent transparent;
  }
}
.edit-group :deep(.vue-select) {
  color: black;
  margin-bottom: 10px;
}
</style>
