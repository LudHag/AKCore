<template>
    <div>
        Månadskalender
    </div>   
</template>
<script>
    import Constants from '../../constants';

    export default {
        props: ['years', 'loggedIn', 'member'],
        methods: {
            getMonthName(month) {
                return Constants.MONTHS[month[0].month - 1];
            },
            signup(id) {
                this.$emit('signup', id);
            }
        }
    }
</script>
<style lang="scss">
  
</style>
