<template>
    <div>
        <div v-for="year in years">
            <h2>{{year.year}}</h2>
            <div v-for="month in year.months">
                <h3>{{month[0].month}}</h3>
                <div v-for="event in month">
                    {{event.name}} -- {{event.day}}
                </div>
            </div>
        </div>
    </div>   
</template>
<script>
    import Spinner from "../Spinner";

    export default {
        props: ['years', 'loggedIn', 'member']
    }
</script>
<style lang="scss">
  
</style>
