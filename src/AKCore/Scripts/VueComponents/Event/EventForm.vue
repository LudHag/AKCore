<template>
  <form
    v-if="eventInfo"
    @submit.prevent="submitForm"
    :action="'/upcoming/Signup/' + eventInfo.event.id"
    method="POST"
  >
    <div class="alert alert-danger" ref="error" style="display: none"></div>
    <div class="alert alert-success" ref="success" style="display: none"></div>
    <div class="form-group">
      <label>Kommer till:</label>
      <div class="indent">
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Hålan"
              required
            />
            Hålan
          </label>
        </div>
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Direkt"
              required
            />
            Direkt
          </label>
        </div>
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Kan inte komma"
              required
            />
            Kan inte komma
          </label>
        </div>
      </div>
    </div>
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="car" />
        <input type="hidden" name="Car" v-model="car" />
        Har bil
      </label>
    </div>
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="instrument" />
        <input type="hidden" name="Instrument" v-model="instrument" />
        Tar med instrument själv
      </label>
    </div>
    <div class="form-group">
      <label>Kommentar</label>
      <input
        class="form-control"
        name="Comment"
        type="text"
        v-model="comment"
      />
    </div>
    <div class="form-group">
      <button type="submit" class="btn btn-default">Anmäl</button>
    </div>
  </form>
</template>
<script setup lang="ts">
import { onMounted, ref, onActivated } from "vue";
import { defaultFormSend } from "../../services/apiservice";
import { UpcomingEventInfo, UpcomingWhere } from "../Upcoming/models";

const emit = defineEmits<{
  (e: "update"): void;
}>();

const props = defineProps<{
  eventInfo: UpcomingEventInfo | null;
}>();

const where = ref<UpcomingWhere>(null);
const car = ref(false);
const instrument = ref(false);
const comment = ref<string | null>(null);
const error = ref<HTMLElement | null>(null);
const success = ref<HTMLElement | null>(null);

const submitForm = (event: Event) => {
  defaultFormSend(
    event.target as HTMLFormElement,
    error.value,
    success.value,
    () => {
      emit("update");
    }
  );
};

const loadForm = () => {
  if (props.eventInfo) {
    where.value = props.eventInfo.where;
    comment.value = props.eventInfo.comment;
    instrument.value = props.eventInfo.instrument;
    car.value = props.eventInfo.car;
  }
};

onMounted(() => {
  loadForm();
});

onActivated(() => {
  loadForm();
});
</script>
<style lang="scss"></style>
