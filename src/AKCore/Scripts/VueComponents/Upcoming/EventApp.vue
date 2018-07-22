<template>
    <div id="event-app">
        <spinner v-if="loading" :size="'medium'"></spinner>
        <h1 v-if="event">{{event.name}}</h1>
    </div>        
</template>
<script>
    import Spinner from "../Spinner";

    export default {
        components: {
            Spinner
        },
        props: ['eventId'],
        data() {
            return {
                events: {},
                loadin: false
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
  
</style>
