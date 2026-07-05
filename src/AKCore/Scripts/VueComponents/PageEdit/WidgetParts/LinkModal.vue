<template>
  <modal
    :show-modal="showModal"
    :header="'Länk'"
    :notransition="true"
    @close="close"
  >
    <template #body>
      <div class="modal-body">
        <div class="form-group" :class="{ 'has-error': visibleError }">
          <label>URL</label>
          <div class="link-modal__url-row">
            <input
              type="text"
              class="form-control"
              v-model="url"
              placeholder="https://"
              @keydown.enter.prevent="apply"
            />
            <button type="button" class="btn btn-default" @click="pickDocument">
              Välj Dokument
            </button>
          </div>
          <p v-if="visibleError" class="help-block">{{ visibleError }}</p>
        </div>
        <div v-if="showTextField" class="form-group">
          <label>Länktext</label>
          <input
            type="text"
            class="form-control"
            v-model="linkText"
            placeholder="Länktext"
            @keydown.enter.prevent="apply"
          />
        </div>
      </div>
    </template>
    <template #footer>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" @click="close">
          Avbryt
        </button>
        <button type="button" class="btn btn-primary" @click="apply">
          Välj
        </button>
      </div>
    </template>
  </modal>
</template>

<script setup lang="ts">
import Modal from "../../Modal.vue";
import { EventBus } from "@utils/eventbus";
import { computed, ref, watch } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
  (e: "apply", url: string, text: string): void;
}>();

const props = defineProps<{
  showModal: boolean;
  initialUrl: string;
  initialText?: string;
  showTextField?: boolean;
}>();

const url = ref("");
const linkText = ref("");
const submitAttempted = ref(false);

const isValidLinkUrl = (value: string): boolean => {
  if (value.startsWith("/")) {
    return /^\/[^\s<>"']+$/.test(value);
  }

  try {
    const parsed = new URL(value);
    return ["http:", "https:", "mailto:"].includes(parsed.protocol);
  } catch {
    return false;
  }
};

const validationError = computed(() => {
  const trimmed = url.value.trim();
  if (!trimmed) {
    return "Ange en URL";
  }

  if (isValidLinkUrl(trimmed)) {
    return "";
  }

  return "Ange en giltig URL, t.ex. https://example.com eller /media/fil.pdf";
});

const visibleError = computed(() => {
  if (!submitAttempted.value) {
    return "";
  }

  return validationError.value;
});

watch(
  () => props.showModal,
  (isOpen) => {
    if (isOpen) {
      url.value = props.initialUrl;
      linkText.value = props.initialText ?? "";
      submitAttempted.value = false;
    }
  },
  { immediate: true },
);

const close = () => {
  emit("close");
};

const apply = () => {
  submitAttempted.value = true;

  if (validationError.value) {
    return;
  }

  emit("apply", url.value.trim(), linkText.value.trim());
};

const pickDocument = () => {
  EventBus.trigger("loadfile", (selectedUrl: string) => {
    url.value = selectedUrl;
  });
};
</script>

<style lang="scss" scoped>
.link-modal__url-row {
  display: flex;
  gap: 8px;

  .form-control {
    flex: 1;
  }
}

.help-block {
  margin: 6px 0 0;
  color: #a94442;
}
</style>
