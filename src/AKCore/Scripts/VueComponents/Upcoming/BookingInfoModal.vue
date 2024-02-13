<template>
  <modal :show-modal="isOpen" :header="header" @close="close">
    <template #body>
      <div class="modal-body">
        <div class="row">
          <div  style="font-weight: 500">



           <form autocomplete="off" @submit.prevent="onBookSubmit">

              <div class="col-sm-6">
                <div
                  class="alert alert-danger"
                  ref="error"
                  style="display: none"
                ></div>
                <div class="form-group">
                  <label>Meddelande</label>
                  <input
                    type="text"
                    autocomplete="off"
                    class="form-control"
                    name="Meddelande"
                    placeholder="Meddelande"
                    v-model="booking.message"
                  />
                </div>
                <div class="form-group">
                  <label>Starttid</label>
                  <input
                  class="form-control"
                  type="time"
                  v-model="booking.startsTime"
                  name="Halan"
                />
                </div>
  
            </div>
            <div class="modal-footer">
              <button type="submit" class="btn btn-primary">Spara</button>
            </div>
          </form>
          </div>
        </div>
      </div>
    </template>
  </modal>
</template>
<script setup lang="ts">
import { computed, ref } from "vue";
import Modal from "../Modal.vue";
import { BookingEvent, UpcomingEvent } from "./models";
import { TranslationDomain, translate } from "../../translations";
import { eventIsRep } from "./functions";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "onBook", bookingEvent: BookingEvent): void;
}>();

const props = defineProps<{
  event: Date | null;
  isOpen: boolean;
}>();

const close = () => emit("close");

const booking = ref<BookingEvent>({date: props.event} as BookingEvent);

const onBookSubmit = () => {
  emit("onBook", booking.value);
}
const header = computed(() => {
  if (!props.event) {
    return "New Booking";
  }
});


const t = (key: string, domain: TranslationDomain = "upcoming") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
</style>
