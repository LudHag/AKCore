<template>
    <div id="upcoming-app">
        <div v-if="!showEvent">
            <div class="calendar-actions" v-if="loggedIn">
                <a href="/upcoming/akevents.ics" @click.prevent="showIcal = !showIcal" class="fa fa-calendar"> Ical-länk</a>
                <div class="input-group ical-copy" v-if="showIcal">
                    <input class="form-control" id="ical-link" type="text" readonly :value="icalLink" />
                    <span class="input-group-btn">
                        <button class="btn btn-default fa fa-files-o copy-btn" @click.prevent="copyIcal" type="button"></button>
                    </span>
                </div>
                <div class="calendar-control hidden-xs">
                    <a href="#" class="event calendar-toggle" @click.prevent="calendarView = false" v-bind:class="{ active: !calendarView }">Lista</a><a href="#" class="month calendar-toggle" @click.prevent="calendarView = true" v-bind:class="{ active: calendarView }">Månad</a>
                </div>
            </div>
            <spinner :size="'medium'" v-if="!years"></spinner>
            <upcoming-list v-if="!calendarView && years"
                           :years="years"
                           :logged-in="loggedIn"
                           :member="member"
                           @signup="signup"></upcoming-list>
            <upcoming-calendar v-if="calendarView && years"
                           :years="years"
                           :logged-in="loggedIn"
                           :member="member"
                           @signup="signup"></upcoming-calendar>
        </div>
        <keep-alive>
            <event-app v-if="showEvent"
                       :event-id="selectedEventId"
                       @close="closeEvent"></event-app>
        </keep-alive>
    </div>        
</template>
<script>
    import Spinner from "../Spinner";
    import UpcomingList from "./UpcomingList";
    import UpcomingCalendar from "./UpcomingCalendar";
    import EventApp from "../Event/EventApp";

    export default {
        components: {
            Spinner,
            UpcomingList,
            UpcomingCalendar,
            EventApp
        },
        props: ['eventId'],
        data() {
            return {
                years: null,
                loading: false,
                loggedIn: false,
                member: false,
                calendarView: false,
                icalLink: "",
                showIcal: false,
                showEvent: false,
                selectedEventId: -1
            }
        },
        methods: {
            copyIcal() {
                const copyText = document.querySelector("#ical-link");
                copyText.select();
                document.execCommand("copy");
            },
            signup(id) {
                this.selectedEventId = id;
                this.showEvent = true;
                history.pushState({ showEvent: true, selectedEventId: id },
                    "", "/upcoming/Event/" + id);
            },
            closeEvent() {
                this.showEvent = false;
                history.pushState({ showEvent: false, selectedEventId: -1 },
                    "", "/upcoming");
            }
        },
        created() {
            const self = this;
            if (this.eventId > -1) {
                this.selectedEventId = this.eventId;
                this.showEvent = true;
                history.replaceState({ showEvent: true, selectedEventId: this.eventId },
                    "", "/upcoming/Event/" + this.eventId);
            } else {
                history.replaceState({ showEvent: false, selectedEventId: -1 },
                    "", "/upcoming");
            }

            this.loading = true;
        
            $.ajax({
                url: "/Upcoming/UpcomingListData",
                type: "GET",
                success: function (res) {
                    self.years = res.years;
                    self.loggedIn = res.loggedIn;
                    self.member = res.member;
                    self.icalLink = res.icalLink;
                    self.loading = false;
                },
                error: function () {
                    console.log("fel");
                    self.loading = false;
                }
            });
            window.onpopstate = function (event) {
                if (event.state) {
                    self.showEvent = event.state.showEvent;
                    self.selectedEventId = event.state.selectedEventId;
                }
            };
        }
    }
</script>
<style lang="scss">
  
</style>
