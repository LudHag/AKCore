<template>
    <div id="upcoming-app">
        <div v-for="event in events">
            {{event.name}} -- {{event.day}}
        </div>

    </div>        
</template>
<script>
    import Spinner from "../Spinner";

    export default {
        components: {
            Spinner
        }, 
        data() {
            return {
                events: null,
                loading: false,
                loggedIn: false,
                member: false,
                icalLink: ""
            }
        },
        created() {
            const self = this;
            this.loading = true;
            $.ajax({
                url: "/Upcoming/UserListData",
                type: "GET",
                success: function (res) {
                    self.events = res.events;
                    self.loggedIn = res.loggedIn;
                    self.medlem = res.medlem;
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
