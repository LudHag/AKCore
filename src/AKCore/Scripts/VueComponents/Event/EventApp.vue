<template>
    <div id="event-app">
        <spinner v-if="loading" :size="'medium'"></spinner>
        <div v-if="event">
            <a href="#" @click.prevent="close" class="close-event pull-right glyphicon glyphicon-remove"></a>
            <h1>{{event.name}}</h1>
            <div class="row hidden-print">
                <div class="col-sm-6">
                    <event-form :event="event"></event-form>
                </div>
            </div>
        </div>
    </div>        
</template>
<script>
    import Spinner from "../Spinner";
    import EventForm from "./EventForm";

    export default {
        components: {
            Spinner,
            EventForm
        },
        props: ['eventId'],
        data() {
            return {
                events: {},
                loading: false
            }
        },
        computed: {
            event() {
                if (this.eventId < 0 || !this.events) {
                    return false;
                }
                const e = this.events[this.eventId];
                return e;
            }
        },
        watch: {
            eventId() {
                if (!this.events[this.eventId]) {
                    this.loadEvent(this.eventId);
                }
            }
        },
        methods: {
            loadEvent(id) {
                const self = this;
                this.loading = true;
                $.ajax({
                    url: "/upcoming/Event/EventData/" + id,
                    type: "GET",
                    success: function (res) {
                        self.events = Object.assign({}, self.events, { [id]: res.event})
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
            }
        },
        created() {
            if (this.eventId > -1) {
                this.loadEvent(this.eventId);
            }
        }
    }
</script>
<style lang="scss">
    .close-event {
        font-size: 26px;
    }
</style>
