<template>
  <modal :show-modal="showModal" header="Lägg till anmälan" @close="close">
    <template #body>
      <form
        action="/upcoming/EditSignup"
        method="POST"
        @submit.prevent="submitForm"
      >
        <div class="modal-body">
          <div
            class="alert alert-danger"
            ref="error"
            style="display: none"
          ></div>
          <div class="row">
            <div class="col-sm-12">
              <input type="hidden" name="eventId" :value="eventId" />
              <div class="form-group">
                <label>Medlem</label>
                <select name="memberId" class="form-control" required>
                  <option value="">Välj en medlem</option>
                  <option
                    v-for="member in members"
                    :key="member.id"
                    :value="member.id"
                  >
                    {{ member.fullName }}
                  </option>
                </select>
              </div>
              <div class="form-group">
                <label>Status</label>
                <select class="form-control" name="type" required>
                  <option value="">Välj anmälningstyp</option>
                  <option
                    v-for="signupType in SIGNUPTYPES"
                    :key="signupType"
                    :value="signupType"
                  >
                    {{ signupType }}
                  </option>
                </select>
                <div class="checkbox">
                  <label>
                    <input type="checkbox" v-model="car" />
                    <input type="hidden" name="Car" v-model="car" />
                    Har bil
                  </label>
                </div>
                <div class="checkbox">
                  <label>
                    <input type="checkbox" v-model="inverseInstrument" />
                    <input
                      type="hidden"
                      name="Instrument"
                      v-model="instrument"
                    />
                    {{ t("brings-instrument", "signup") }}
                  </label>
                </div>
              </div>
            </div>
          </div>
        </div>
        <div class="modal-footer">
          <button class="btn btn-primary" type="reset">Återställ</button>
          <button type="submit" class="btn btn-primary">Spara</button>
        </div>
      </form>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { ref, computed } from "vue";
import { SIGNUPTYPES } from "../../constants";
import { defaultFormSend } from "@services/apiservice";
import Modal from "../Modal.vue";
import { AvailableMember } from "../Upcoming/models";
import { TranslationDomain, translate } from "../../translations";

const emit = defineEmits<{
  (e: "update"): void;
  (e: "close"): void;
}>();

defineProps<{
  members: AvailableMember[];
  eventId: number;
  showModal: boolean;
}>();

const car = ref(false);
const instrument = ref(true);
const inverseInstrument = computed({
  get() {
    return !instrument.value;
  },
  set(value: boolean) {
    instrument.value = !value;
  },
});

const error = ref<HTMLElement | null>(null);

const close = () => emit("close");

const submitForm = (event: Event) => {
  defaultFormSend(event.target as HTMLFormElement, error.value, null, () => {
    emit("update");
    emit("close");
  });
};
const t = (key: string, domain: TranslationDomain = "profile") =>
  translate(domain, key);
</script>
<style lang="scss"></style>
