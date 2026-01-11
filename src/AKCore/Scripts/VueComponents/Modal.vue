<template>
  <transition name="modal">
    <div
      v-if="showModal"
      class="modal-wrapper"
      :class="{ notransition: notransition }"
    >
      <div class="modal show" @click="modalClose">
        <div class="modal-dialog">
          <div class="modal-content">
            <div class="modal-header">
              <button type="button" class="close" @click.prevent="close">
                &times;
              </button>
              <h4 class="modal-title">{{ header }}</h4>
            </div>
            <slot name="body"></slot>
            <slot name="footer"></slot>
          </div>
        </div>
      </div>
      <div class="modal-backdrop fade in"></div>
    </div>
  </transition>
</template>
<script setup lang="ts">
import { watch, onUnmounted } from "vue";

const emit = defineEmits<{
  (e: "close"): void;
}>();

const props = defineProps<{
  showModal: boolean;
  header?: string;
  notransition?: boolean;
}>();

const modalClose = (event: Event) => {
  const eventTarget = event?.target as HTMLElement | undefined;

  if (eventTarget?.classList.contains("modal")) {
    emit("close");
  }
};

const close = () => {
  emit("close");
};

const handleEscapeKey = (event: KeyboardEvent) => {
  if (event.key === "Escape" && props.showModal) {
    close();
  }
};

let savedScrollY = 0;

watch(
  () => props.showModal,
  (isOpen) => {
    if (isOpen) {
      savedScrollY = window.scrollY;
      document.body.classList.add("modal-open");
      document.body.style.top = `-${savedScrollY}px`;
      document.addEventListener("keydown", handleEscapeKey);
    } else {
      document.body.classList.remove("modal-open");
      document.body.style.top = "";
      window.scrollTo(0, savedScrollY);
      document.removeEventListener("keydown", handleEscapeKey);
    }
  },
  { immediate: true },
);

// Cleanup on unmount
onUnmounted(() => {
  document.body.classList.remove("modal-open");
  document.body.style.top = "";
  window.scrollTo(0, savedScrollY);
  document.removeEventListener("keydown", handleEscapeKey);
});
</script>

<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../Styles/variables.scss";

.modal {
  z-index: 70000;

  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  a:focus {
    outline-color: $akred;
  }
}

.modal-enter,
.modal-leave-active {
  opacity: 0;
}

.modal-enter .modal-dialog,
.modal-leave-active .modal-dialog {
  transform: translateY(-30%);
}
.modal-wrapper {
  transition: all 0.3s ease;
  &.notransition {
    transition: none;
  }
}

.modal-content {
  background-color: $akwhite;
  color: black;
  max-height: calc(100vh - 100px);
  overflow: auto;
}
.modal-backdrop {
  position: fixed;
  top: 0;
  right: 0;
  bottom: 0;
  left: 0;
  z-index: 1040;
  background-color: #000;
  transition: opacity 0.3s ease;
  &.fade {
    opacity: 0;
  }
  &.in {
    opacity: 0.5;
  }
}

.modal-header {
  padding: 15px;
  border-bottom: 1px solid $aklightgrey;
}
.modal-title {
  margin: 0;
  line-height: 1.4;
}
.modal-header .close {
  margin-top: -2px;
}

:deep(.modal-body) {
  padding: 15px;
}

:deep(.modal-footer) {
  padding: 15px;
  text-align: right;
  border-top: 1px solid $aklightgrey;

  .btn + .btn {
    margin-bottom: 0;
    margin-left: 5px;
  }
  .btn-group .btn + .btn {
    margin-left: -1px;
  }
  .btn-block + .btn-block {
    margin-left: 0;
  }
}

@media (min-width: $screen-sm-min) {
  .modal-dialog {
    width: 600px;
    margin: 30px auto;
  }
}
</style>
