<template>
    <div>
        <div class="controls">
            <a href="" 
               class="prev-month glyphicon glyphicon-chevron-left" 
               @click.prevent="prevMonth"
               v-show="showPrevArrow"></a>
            <span class="date">{{thisMonthName}} {{year}}</span>
            <a href="" class="next-month glyphicon glyphicon-chevron-right" @click.prevent="nextMonth"></a>
        </div>
        <table class="month table table-bordered">
            <thead>
                <tr>
                    <th v-for="day in days">{{day}}</th> 
                </tr>
            </thead>
            <tbody>
                <tr class="week" v-for="day of firstWeekDays">
                    <calendar-day :year="year"
                                  :month="month"
                                  :monthevents="monthEvents"
                                  :day="day.addDays(i)" 
                                  v-for="i in [0,1,2,3,4,5,6]"
                                  :key="month + '' + i"
                                  @open="openEvent">                                  >
                    </calendar-day>
                </tr>
            </tbody>
        </table>
        <event-info-modal :event="modalEvent" member="member" @signup="signup" @close="closeModal"></event-info-modal>
    </div>   
</template>
<script>
    import Constants from '../../constants';
    import CalendarDay from './CalendarDay';
    import EventInfoModal from './EventInfoModal';

    const timeDay = 24 * 60 * 60 * 1000;
    const today = new Date();

    export default {
        props: ['years', 'loggedIn', 'member'],
        components: {
            CalendarDay,
            EventInfoModal
        },
        data() {
            return {
                month: 0,
                year: 0,
                modalEvent: null
            }
        },
        methods: {
            getMonthName(month) {
                return Constants.MONTHS[month];
            },
            signup(id) {
                this.closeModal();
                this.$emit('signup', id);
            },
            nextMonth() {
                this.month++;
                if (this.month > 11) {
                    this.month = 0;
                    this.year++;
                }
            },
            prevMonth() {
                this.month--;
                if (this.month < 0) {
                    this.month = 11;
                    this.year--;
                }
            },
            openEvent(e) {
                this.modalEvent = e;
            },
            closeModal() {
                this.modalEvent = null;
            }
        },
        computed: {
            days() {
                return Constants.DAYS;
            },
            monthEvents() {
                const yearEvents = this.years[this.year];
                if (yearEvents) {
                    return yearEvents.months[this.month + 1];
                }
                return [];
            },
            thisMonthName() {
                return this.getMonthName(this.month);
            },
            firstWeekDays() {
                const firstDayOfMonth = new Date(this.year, this.month, 1);
                const lastDayOfMonth = new Date(this.year, this.month + 1, 0);
                let firstDayWeekDay = firstDayOfMonth.getDay() - 1;
                if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
                const firstDayOfCalendar = new Date(firstDayOfMonth.getTime() - (firstDayWeekDay * timeDay));
                let monday = new Date(firstDayOfCalendar.getTime());
                const weeks = [];
                while (monday < lastDayOfMonth) {
                    weeks.push(monday);
                    monday = monday.addDays(7);
                }
                return weeks;
            },
            showPrevArrow() {
                return this.month - 1 >= today.getMonth() || this.year > today.getFullYear();
            }
        },
        created() {
            this.year = today.getFullYear();
            this.month = today.getMonth();
        }
    }
</script>
<style lang="scss">
  
</style>
