<template>
    <td class="day">
        <span class="date" v-bind:class="{ outside: outside }">{{day.getDate()}}</span>
        <!--<a href="#" class="dayEvent">00:00 Tysklandsresa</a>-->
    </td>
</template>
<script>
    import Constants from '../../constants';

    export default {
        props: ['day', 'outside']
    }
</script>
<style lang="scss">
  
</style>
