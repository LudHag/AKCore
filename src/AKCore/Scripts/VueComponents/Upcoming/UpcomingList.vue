<template>
    <div>
        <div v-for="year in years">
            <h2>{{year.year}}</h2>
            <div v-for="month in year.months">
                <h3 class="new-month">{{getMonthName(month)}}</h3>
                <upcoming-list-item v-for="event in month"
                                    :key="event.id"
                                    :event="event"
                                    :logged-in="loggedIn"
                                    :member="member"
                                    @signup="signup">
                </upcoming-list-item>
            </div>
        </div>
    </div>   
</template>
<script>
    import Constants from '../../constants';
    import UpcomingListItem from "./UpcomingListItem";

    export default {
        components: { UpcomingListItem },
        props: ['years', 'loggedIn', 'member'],
        methods: {
            getMonthName(month) {
                return Constants.MONTHS[month[0].month - 1];
            },
            signup(id) {
                this.$emit('signup', id);
            }
        },
        computed: {
            months() {
                return Constants.MONTHS;
            }
        }
    }
</script>
<style lang="scss">
  
</style>
