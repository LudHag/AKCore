<template>
    <td class="day" v-bind:class="{ outside: outside }">
        <span class="date">{{day.getDate()}}</span>
        <!--<a href="#" class="dayEvent">00:00 Tysklandsresa</a>-->
    </td>
</template>
<script>
    import Constants from '../../constants';
    const today = new Date();

    export default {
        props: ['day', 'month', 'year'],
        computed: {
            outside() {
                if (this.month == this.day.getMonth() && this.year == this.day.getFullYear()) {
                    return this.year == today.getFullYear() &&
                        this.month == today.getMonth() &&
                        this.day.getDate() < today.getDate();
                } else {
                    return true;
                }
            }
        }
    }
</script>
<style lang="scss">
  
</style>
