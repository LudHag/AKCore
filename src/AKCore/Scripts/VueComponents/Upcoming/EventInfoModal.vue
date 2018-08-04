<template>
    <transition name="modal">
        <div v-if="event">
            <div class="modal event-info-modal show" @click="close">
                <div class="modal-dialog modal-lg">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" @click.prevent="close">×</button>
                            <h4 class="modal-title">{{event.name}}</h4>
                        </div>
                        <div class="modal-body">
                            <div class="row">
                                <div class="col-sm-4" style="font-weight: 500;">
                                    <p class="modal-day" style="text-transform: capitalize;">{{event.day}}</p>
                                    <p class="modal-place">{{event.place}}</p>
                                </div>
                                <div class="col-sm-4">
                                    <p class="modal-halan" v-if="event.halanTime">Samling i hålan: {{event.halanTime}}</p>
                                    <p class="modal-there" v-if="event.thereTime">Samling på plats: {{event.thereTime}}</p>
                                    <p class="modal-start" v-if="event.startsTime">Spelning startar: {{event.startsTime}}</p>
                                </div>
                                <div class="col-sm-4">
                                    <a class="green" v-if="signupable && event.signupState" @click.prevent.stop="openSignup" :href="signupUrl">Anmäld ({{event.signupState}})</a>
                                    <a v-if="signupable && !event.signupState" @click.prevent.stop="openSignup" :href="signupUrl">Anmäl</a>
                                    <p class="modal-comming" v-if="signupable">{{event.coming}} Kommer - {{event.notComing}} Kommer inte</p>
                                    <p class="modal-stand" v-if="event.stand">Speltyp: {{event.stand}}</p>
                                    <p class="modal-fika" v-if="event.fika">Fika och städning: {{event.fika}}</p>
                                </div>
                                <div class="extra">
                                    <div class="col-sm-12">
                                        <p class="modal-description">{{event.description}}</p>
                                    </div>
                                    <div class="col-sm-12">
                                        <p class="modal-intdescription">{{event.internalDescription}}</p>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="modal-backdrop fade in"></div>
        </div>
    </transition>
</template>
<script>

    export default {
        props: ['event', 'member'],
        methods: {
            close(event) {
                if (event.target.classList.contains("modal") ||
                    event.target.classList.contains("close")) {
                    this.$emit("close");
                }
            },
            openSignup() {
                this.$emit('signup', this.event.id);
            }
        },
        computed: {
            signupUrl() {
                return "/upcoming/Event/" + this.event.id;
            },
            signupable() {
                return this.member && (event.type === 'Spelning' || event.type === 'Kårhusrep');
            }
        }
    }
</script>
<style lang="scss">
    .event-info-modal .green {
        color: #02C66F;
    }

    .modal-dialog {
        transition: all .3s ease;
    }

    .modal-enter, .modal-leave-active {
        opacity: 0;
    }

    .modal-enter .modal-dialog,
    .modal-leave-active .modal-dialog {
        transform: translateY(-30%);
    }
</style>