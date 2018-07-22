<template>
    <div id="upcoming-app">
        <div class="calendar-actions" v-if="loggedIn">
            <a href="/upcoming/akevents.ics" @click.prevent="showIcal=!showIcal" class="fa fa-calendar"> Ical-länk</a>
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
                       :member="member"></upcoming-list>
        <div v-if="calendarView">
            Månadskalender
        </div>
    </div>        
</template>
<script>
    import Spinner from "../Spinner";
    import UpcomingList from "./UpcomingList";

    export default {
        components: {
            Spinner,
            UpcomingList
        }, 
        data() {
            return {
                years: null,
                loading: false,
                loggedIn: false,
                member: false,
                calendarView: false,
                icalLink: "",
                showIcal: false
            }
        },
        methods: {
            copyIcal() {
                const copyText = document.querySelector("#ical-link");
                copyText.select();
                document.execCommand("copy");
            }
        },
        created() {
            const self = this;
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
        }
    }
</script>
<style lang="scss">
  
</style>
