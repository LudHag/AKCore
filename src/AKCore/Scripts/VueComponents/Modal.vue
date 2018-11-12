<template>
  <transition name="modal">
      <div v-if="showModal" class="modal-wrapper">
        <div class="modal show" @click="modalClose">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" @click.prevent="close">&times;</button>
                            <h4 class="modal-title">Redigera händelse</h4>
                        </div>
                        <slot name="body"></slot>
                    </div>
                </div>
            </div>
            <div class="modal-backdrop fade in"></div>
        </div>
    </transition>
</template>
<script>

  export default {
        props: ['showModal', 'header'],
        methods: {
            modalClose() {
                if (event.target.classList.contains("modal")) {
                    this.$emit("close");
                }
            },
            close(){
                this.$emit("close");
            }
        }
  }
</script>

<style lang="scss" scoped>

.modal-enter, .modal-leave-active {
    opacity: 0;
}

.modal-enter .modal-dialog,
.modal-leave-active .modal-dialog {
    transform: translateY(-30%);
}
.modal-wrapper {
    transition: all .3s ease;
}
</style>