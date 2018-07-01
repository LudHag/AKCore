<template>
    <div id="user-list">
        {{users.length}}
        <div v-for="user in users">
            {{user.firstName}} {{user.lastName}}
        </div>
    </div>
</template>
<script>
    export default {
        props: ['users']
    }
</script>
<style lang="scss">

</style>
