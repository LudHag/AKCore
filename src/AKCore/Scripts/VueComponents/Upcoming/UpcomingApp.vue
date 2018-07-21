<template>
    <div id="upcoming-app">
    </div>        
</template>
<script>
    import Spinner from "../Spinner";

    export default {
        components: {
            Spinner
        }, 
        props: ['loggedin'],
        data() {
            return {
                events: null
            }
        },
        created() {
            console.log("started");
        }
    }
</script>
<style lang="scss">
  
</style>
