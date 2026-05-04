<template>
  <form
    v-if="eventInfo && !eventInfo.isPassed"
    @submit.prevent="submitForm"
    :action="'/upcoming/Signup/' + eventInfo.event.id"
    method="POST"
  >
    <div class="alert alert-danger" ref="error" style="display: none"></div>
    <div class="alert alert-success" ref="success" style="display: none"></div>
    <div class="form-group">
      <label>{{ t("coming-to") }}:</label>
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
            {{ t("direct") }}
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
            {{ t("cant-come") }}
          </label>
        </div>
      </div>
    </div>
    <div
      class="form-group"
      v-if="eventInfo.availableInstruments && eventInfo.availableInstruments.length > 1"
    >
      <label>{{ t("instrument") }}:</label>
      <select class="form-control" name="SelectedInstrument" v-model="selectedInstrument">
        <option
          v-for="instr in eventInfo.availableInstruments"
          :key="instr"
          :value="instr"
        >{{ t(instr, "instruments") }}</option>
      </select>
    </div>
    <input
      v-else-if="selectedInstrument"
      type="hidden"
      name="SelectedInstrument"
      :value="selectedInstrument"
    />
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="car" />
        <input type="hidden" name="Car" v-model="car" />
        {{ t("has-car") }}
      </label>
    </div>
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="inverseInstrument" />
        <input type="hidden" name="Instrument" v-model="instrument" />
        {{ t("brings-instrument") }}
      </label>
    </div>
    <div class="form-group">
      <label>{{ t("comment") }}</label>
      <input
        class="form-control"
        name="Comment"
        type="text"
        v-model="comment"
      />
    </div>
    <div class="form-group">
      <button type="submit" class="btn btn-default">{{ t("sign-up") }}</button>
    </div>
  </form>
</template>
<script setup lang="ts">
import { onMounted, ref, onActivated, computed } from "vue";
import { defaultFormSend } from "@services/apiservice";
import { UpcomingEventInfo, UpcomingWhere } from "../Upcoming/models";
import { TranslationDomain, translate } from "@scripts/translations";

const emit = defineEmits<{
  (e: "update"): void;
}>();

const props = defineProps<{
  eventInfo: UpcomingEventInfo | null;
}>();

const where = ref<UpcomingWhere>(null);
const car = ref(false);
const instrument = ref(true);
const comment = ref<string | null>(null);
const selectedInstrument = ref<string | null>(null);

const inverseInstrument = computed({
  get() {
    return !instrument.value;
  },
  set(value: boolean) {
    instrument.value = !value;
  },
});

const error = ref<HTMLElement | null>(null);
const success = ref<HTMLElement | null>(null);

const submitForm = (event: Event) => {
  defaultFormSend(
    event.target as HTMLFormElement,
    error.value,
    success.value,
    () => emit("update"),
  );
};

const loadForm = () => {
  const info = props.eventInfo;
  if (!info) return;

  if (info.where) {
    where.value = info.where;
    comment.value = info.comment;
    instrument.value = info.instrument;
    car.value = info.car;
  }

  selectedInstrument.value =
    info.selectedInstrument ??
    (info.availableInstruments?.length > 0 ? info.availableInstruments[0] : null);
};

onMounted(() => loadForm());
onActivated(() => loadForm());

const t = (key: string, domain: TranslationDomain = "signup") =>
  translate(domain, key);
</script>
<style lang="scss"></style>
