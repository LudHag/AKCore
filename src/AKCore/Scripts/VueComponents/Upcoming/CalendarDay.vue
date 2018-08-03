<template>
    <td class="day" v-bind:class="{ outside: outside }">
        <span class="date">{{day.getDate()}}</span>
        <a href="#" v-for="e in events" class="dayEvent">{{e.halanTime}} {{e.name}}</a>
    </td>
</template>
<script>
    import Constants from '../../constants';
    const today = new Date();

    export default {
        props: ['monthevents' ,'day', 'month', 'year'],
        computed: {
            outside() {
                if (this.month == this.day.getMonth() && this.year == this.day.getFullYear()) {
                    return this.year == today.getFullYear() &&
                        this.month == today.getMonth() &&
                        this.day.getDate() < today.getDate();
                } else {
                    return true;
                }
            },
            events() {
                if (!this.monthevents || this.outside) {
                    return [];
                }
                return this.monthevents.filter((e) => {
                    return e.dayInMonth === this.day.getDate();
                });
            }
        }
    }
</script>
<style lang="scss">
  
</style>
