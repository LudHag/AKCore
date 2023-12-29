<template>
  <div class="col-md-9">
    <div class="row">
      <div class="col-md-8">
        <div class="form-group">
          <label for="name">Namn</label>
          <input
            type="text"
            class="form-control"
            required
            name="Name"
            placeholder="Namn"
            v-model="modelValue.name"
          />
        </div>
        <div class="form-group">
          <label for="slug">Sidlänk</label>
          <input
            type="text"
            class="form-control"
            required
            name="Slug"
            placeholder="Sidlänk"
            v-model="modelValue.slug"
          />
        </div>
      </div>
      <div class="col-md-4">
        <div class="form-group">
          <label for="slug">Inställningar</label>
          <div class="checkbox">
            <label>
              <input
                type="checkbox"
                name="LoggedIn"
                v-model="modelValue.loggedIn"
              />
              Kräver inloggning
            </label>
          </div>
          <div class="checkbox">
            <label>
              <input
                type="checkbox"
                name="LoggedOut"
                v-model="modelValue.loggedOut"
              />
              Enbart utloggade
            </label>
          </div>
          <div class="checkbox">
            <label>
              <input
                type="checkbox"
                name="BalettOnly"
                v-model="modelValue.balettOnly"
              />
              För baletten
            </label>
          </div>
        </div>
        <button type="submit" class="btn btn-default">Spara</button>
      </div>
    </div>
    <div class="row meta-area" v-if="!modelValue.loggedIn">
      <div class="col-xs-12">
        <a
          href="#"
          class="btn btn-default"
          type="button"
          @click.prevent="showMeta = !showMeta"
        >
          Meta description
          <span class="caret"></span>
        </a>
        <textarea
          class="form-control meta-area"
          v-if="showMeta"
          v-model="modelValue.metaDescription"
        >
        </textarea>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref } from "vue";
import { PageEditModel, PageRevisionEditModel } from "./models";

const showMeta = ref(false);

defineProps<{
  modelValue: PageEditModel | PageRevisionEditModel;
}>();
</script>
<style lang="scss" scoped>
.meta-area {
  margin-top: 10px;
}
.meta-area {
  margin-bottom: 0;
}
</style>
