<template>
    <div id="event-app">
        <spinner v-if="loading && !eventInfo" :size="'medium'"></spinner>
        <div v-if="eventInfo">
            <a href="#" @click.prevent="close" class="close-event pull-right glyphicon glyphicon-remove"></a>
            <h1>{{eventInfo.event.name}}</h1>
            <div class="row hidden-print">
                <div class="col-sm-6">
                    <event-form :event-info="eventInfo" @update="loadEvent"></event-form>
                </div>
                <div class="col-sm-6">
                    <div class="col-sm-12" style="font-weight: 500;">
                        <p style="text-transform: capitalize;">{{eventInfo.event.day}}</p>
                        <p>{{eventInfo.event.place}}</p>
                        <br />
                    </div>
                    <div class="col-sm-12">
                        <p v-if="eventInfo.event.halanTime">Samling i hålan: {{eventInfo.event.halanTime}}</p>
                        <p v-if="eventInfo.event.thereTime">Samling på plats: {{eventInfo.event.thereTime}}</p>
                        <p v-if="eventInfo.event.startsTime">Spelning startar: {{eventInfo.event.startsTime}}</p>
                    </div>
                    <div class="col-sm-12" v-if="eventInfo.isNintendo">
                        <a href="#" class="btn btn-default" @click.prevent="showAdminEdit">Lägg till anmälningar</a>
                    </div>
                </div>
            </div>
            <edit-signup-modal v-if="showEditForm" :event-id="eventId" :members="eventInfo.members" @update="loadEvent" @close="closeModal"></edit-signup-modal>
            <signup-list :signups="eventInfo.signups" :nintendo="eventInfo.isNintendo"></signup-list>
        </div>
    </div>        
</template>
<script>
    import Spinner from "../Spinner";
    import EventForm from "./EventForm";
    import SignupList from "./SignupList";
    import EditSignupModal from "./EditSignupModal";

    export default {
        components: {
            Spinner,
            EventForm,
            SignupList,
            EditSignupModal
        },
        props: ['eventId'],
        data() {
            return {
                events: {},
                loading: false,
                showEditForm: false
            }
        },
        computed: {
            eventInfo() {
                if (this.eventId < 0 || !this.events) {
                    return false;
                }
                const eInfo = this.events[this.eventId];
                return eInfo;
            }
        },
        watch: {
            eventId() {
                if (!this.events[this.eventId]) {
                    this.loadEvent();
                }
            }
        },
        methods: {
            loadEvent() {
                const self = this;
                this.loading = true;
                $.ajax({
                    url: "/upcoming/Event/EventData/" + self.eventId,
                    type: "GET",
                    success: function (res) {
                        self.events = Object.assign({}, self.events, { [self.eventId]: res})
                        self.loading = false;
                    },
                    error: function () {
                        console.log("fel");
                        self.loading = false;
                    }
                });
            },
            close() {
                this.$emit('close');
            },
            showAdminEdit() {
                this.showEditForm = true;
            },
            closeModal() {
                this.showEditForm = false;
            }
        },
        created() {
            if (this.eventId > -1) {
                this.loadEvent();
            }
        }
    }
</script>
<style lang="scss">
    .close-event {
        font-size: 26px;
    }
</style>
