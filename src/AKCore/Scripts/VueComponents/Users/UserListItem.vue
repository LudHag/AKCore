<template>
  <tbody>
    <tr class="clickable hover-grey" @click.prevent="expanded = !expanded">
      <td>{{ user.fullName }}</td>
      <td>{{ user.userName }}</td>
      <td class="roles">
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
      </td>
      <td class="item-actions">
        <a
          class="btn remove-user glyphicon glyphicon-remove"
          @click.prevent.stop="removeUser"
        ></a>
      </td>
    </tr>
    <tr class="user-edit-container" v-if="expanded">
      <td colspan="4">
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
                    <strong>Senast förtjänade medalj: {{ user.medal }}</strong>
                    <input
                      type="hidden"
                      name="userName"
                      :value="user.userName"
                    />
                    <select
                      class="form-control input-sm"
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
                    <strong>
                      Senast utdelad medalj: {{ user.givenMedal }}
                    </strong>
                    <input
                      type="hidden"
                      name="userName"
                      :value="user.userName"
                    />
                    <select
                      class="form-control input-sm"
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
                    <label>Lägg till roll: </label>
                    <select class="form-control input-sm" name="Role">
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
                  <v-select
                    multiple
                    :searchable="false"
                    v-model="selectedPosts"
                    :options="POSTS"
                  ></v-select>
                  <div class="form-group">
                    <button
                      type="reset"
                      @click.prevent="clearPosts"
                      class="btn btn-primary input-sm"
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
// @ts-ignore
import vSelect from "vue-select";
import ApiService from "../../services/apiservice";
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

watch(
  () => props.user,
  (user) => {
    if (user && user.posts) {
      selectedPosts.value = user.posts.slice();
    }
  }
);

watch(
  () => expanded.value,
  (val) => {
    if (val && props.user && props.user.posts) {
      selectedPosts.value = props.user.posts.slice();
    }
  }
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
    const error = $($(".alert-danger")[0]);
    const success = $($(".alert-success")[0]);
    ApiService.postByUrl(
      "/User/RemoveUser?userName=" + props.user.userName,
      error,
      success,
      () => {
        emit("removeuser", props.user.userName);
      }
    );
  }
};

const saveLastEarned = (event: Event) => {
  const error = $($(".alert-danger")[0]);
  const success = $($(".alert-success")[0]);
  const form = $(event.target as HTMLFormElement) as JQuery<HTMLFormElement>;
  ApiService.defaultFormSend(form, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "medal",
      // @ts-ignore
      value: (event.target as HTMLFormElement).elements.medal.value,
    });
  });
};

const saveLastGiven = (event: Event) => {
  const error = $($(".alert-danger")[0]);
  const success = $($(".alert-success")[0]);
  const form = $(event.target as HTMLFormElement) as JQuery<HTMLFormElement>;
  ApiService.defaultFormSend(form, error, success, () => {
    emit("updateuserprop", {
      userName: props.user.userName,
      prop: "givenMedal",
      // @ts-ignore
      value: (event.target as HTMLFormElement).elements.medal.value,
    });
  });
};

const removeRole = (role: string) => {
  const error = $($(".alert-danger")[0]);
  const success = $($(".alert-success")[0]);
  const roleIndex = props.user.roles.indexOf(role);
  if (roleIndex === -1) {
    return;
  }
  const newRoles = props.user.roles.slice();
  newRoles.splice(roleIndex, 1);

  ApiService.postByUrl(
    "/User/RemoveRole?UserName=" + props.user.userName + "&Role=" + role,
    error,
    success,
    () => {
      emit("updateuserprop", {
        userName: props.user.userName,
        prop: "roles",
        value: newRoles,
      });
    }
  );
};

const addRole = (event: Event) => {
  const error = $($(".alert-danger")[0]);
  const success = $($(".alert-success")[0]);
  const form = $(event.target as HTMLFormElement) as JQuery<HTMLFormElement>;
  // @ts-ignore
  const role = (event.target as HTMLFormElement).elements.Role.value;
  const roleIndex = props.user.roles.indexOf(role);
  if (roleIndex !== -1) {
    return;
  }
  const newRoles = props.user.roles.slice();
  newRoles.push(role);
  ApiService.defaultFormSend(form, error, success, () => {
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
  const error = $($(".alert-danger")[0]);
  const success = $($(".alert-success")[0]);
  const postObj = {
    post: selectedPosts.value,
    userName: props.user.userName,
  };
  ApiService.postByObjectAsForm(
    "/User/AddPost",
    postObj,
    error,
    success,
    () => {
      emit("updateuserprop", {
        userName: props.user.userName,
        prop: "posts",
        value: selectedPosts.value.slice(),
      });
    }
  );
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
</style>
