<template>
  <div id="admin-event-app">
    <div class="row">
      <div class="col-sm-6">
        <a href="#" class="btn btn-primary" @click.prevent="openNewEvent">
          Lägg till händelse
        </a>
      </div>
      <div class="col-sm-6">
        <select class="form-control" @change="newSort">
          <option value="Kommande">Kommande</option>
          <option value="Gamla">Gamla</option>
        </select>
      </div>
    </div>
    <div class="row">
      <div class="col-xs-12">
        <div class="alert alert-success" style="display: none"></div>
        <div class="alert alert-error" style="display: none"></div>
      </div>
    </div>
    <spinner :size="'medium'" v-if="!adminEventData"></spinner>
    <div v-if="adminEventData">
      <h1>Händelser:</h1>
      <div
        class="row event-row"
        v-for="e in adminEventData.events"
        :key="e.id"
        @click="openEvent(e)"
      >
        <div class="col-sm-2">
          <p>{{ e.day }}</p>
        </div>
        <div class="col-sm-4">
          <p>{{ e.name }}</p>
        </div>
        <div class="col-sm-4">
          <p>{{ e.type }}</p>
        </div>
        <div class="col-sm-2">
          <a
            href="#"
            class="remove-event glyphicon glyphicon-remove"
            @click.prevent.stop="removeEvent(e)"
          ></a>
        </div>
      </div>
      <div class="row" v-if="adminEventData.totalPages > 1">
        <div class="col-xs-12">
          <ul class="pagination">
            <li
              v-for="i in paginationPages"
              :key="i"
              :class="{ active: adminEventData.currentPage === i }"
            >
              <a v-if="i !== 0" href="#" @click.prevent="toPage(i)">{{ i }}</a>
              <span v-if="i === 0" class="dots">...</span>
            </li>
          </ul>
        </div>
      </div>
    </div>
    <admin-event-modal
      v-if="adminEventData"
      :show-modal="eventModalOpened"
      :old="adminEventData.old"
      :selected-event="modalEvent"
      @update="eventUpdated"
      @close="closeModal"
    >
    </admin-event-modal>
  </div>
</template>
<script setup lang="ts">
import Spinner from "../Spinner.vue";
import AdminEventModal from "./AdminEventModal.vue";
import { getFromApi, postByUrl } from "../../services/apiservice";
import { ref, computed, onMounted } from "vue";
import { AdminEventModel } from "./models";
import { UpcomingEvent } from "../Upcoming/models";

const adminEventData = ref<AdminEventModel | null>(null);
const eventModalOpened = ref(false);
const modalEvent = ref<UpcomingEvent | null>(null);

const paginationPages = computed(() => {
  const total = adminEventData.value?.totalPages ?? 0;
  const current = adminEventData.value?.currentPage ?? 0;

  const pages = [];
  let gap = false;
  for (let i = 1; i <= total; i++) {
    if (i <= 2 || (i >= current - 1 && i <= current + 1) || i >= total - 1) {
      pages.push(i);
      gap = false;
    } else if (!gap) {
      pages.push(0);
      gap = true;
    }
  }

  return pages;
});

const newSort = (e: Event) => {
  loadEvents((e.target as HTMLSelectElement).value === "Gamla", 1);
};

const removeEvent = (e: UpcomingEvent) => {
  if (
    confirm("Är du säker på att du vill ta bort event: " + e.day + " " + e.name)
  ) {
    const error = $(".alert-danger");
    const success = $(".alert-success");
    postByUrl("/AdminEvent/Remove/" + e.id, error, success, () => {
      loadEvents(
        adminEventData.value?.old ?? false,
        adminEventData.value?.currentPage ?? 1
      );
    });
  }
};

const loadEvents = (old: boolean, page: number) => {
  getFromApi(
    "/AdminEvent/EventData?old=" + old + "&page=" + page,
    null,
    (res: any) => {
      adminEventData.value = res;
    }
  );
};

const openEvent = (e: UpcomingEvent) => {
  modalEvent.value = e;
  eventModalOpened.value = true;
};

const openNewEvent = () => {
  modalEvent.value = null;
  eventModalOpened.value = true;
};

const closeModal = () => {
  eventModalOpened.value = false;
};

const eventUpdated = () => {
  closeModal();
  loadEvents(
    adminEventData.value?.old ?? false,
    adminEventData.value?.currentPage ?? 1
  );
};

const toPage = (i: number) => {
  loadEvents(adminEventData.value?.old ?? false, i);
};

onMounted(() => {
  loadEvents(false, 1);
});
</script>
<style lang="scss"></style>
