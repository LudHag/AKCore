<template>
  <modal :show-modal="showModal" header="'Lägg till anmälan'" @close="close">
    <template v-slot:body>
      <form
        action="/upcoming/EditSignup"
        method="POST"
        @submit.prevent="submitForm"
      >
        <div class="modal-body">
          <div class="alert alert-danger" style="display: none"></div>
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
                  <option v-for="signupType in signupTypes" :key="signupType">
                    {{ signupType }}
                  </option>
                </select>
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
import Constants from "../../constants";
import Modal from "../Modal.vue";
import { AvailableMember } from "../Upcoming/models";

const emit = defineEmits<{
  (e: "update"): void;
  (e: "close"): void;
}>();

const props = defineProps<{
  members: AvailableMember[];
  eventId: number;
  showModal: boolean;
}>();

const signupTypes = Constants.SIGNUPTYPES;

const close = () => {
  emit("close");
};

const submitForm = (event: Event) => {
  const form = $(event.target as HTMLFormElement);
  const error = form.find(".alert-danger");
  const success = form.find(".alert-success");
  $.ajax({
    url: form.attr("action"),
    type: "POST",
    data: form.serialize(),
    success: function (res) {
      if (res.success) {
        success.text("Anmälan uppdaterad");
        success.slideDown().delay(3000).slideUp();
        form.trigger("reset");
        emit("update");
        emit("close");
      } else {
        error.text(res.message);
        error.slideDown().delay(4000).slideUp();
      }
    },
    error: function () {
      error.text("Misslyckades med att anmäla dig");
      error.slideDown().delay(4000).slideUp();
    },
  });
};
</script>
<style lang="scss"></style>
