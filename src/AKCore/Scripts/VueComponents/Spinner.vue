<template>
    <div>
        <div class="spinner" :class="sizeClass"></div>
    </div>
</template>
<script>
    export default {
        props: ['size'],
        computed: {
            sizeClass() {
                return `spinner-${this.size}`;
            }
        }
    }
</script>
<style lang="scss" scoped>
    @import "../../Styles/variables.scss";

    .spinner {
        margin: 0 auto;
        border-radius: 100%;
        transition: all 0.3s linear;
        animation: spinner-spin 0.8s linear infinite;
    }
    .spinner-small {
        border: 2px solid #EEE;
        border-top: 2px solid $akred;
        width: 16px;
        height: 16px;
    }

    .spinner-medium {
        border: 3px solid #EEE;
        border-top: 3px solid $akred;
        width: 32px;
        height: 32px;
    }
    .spinner-large {
        border: 4px solid #EEE;
        border-top: 4px solid $akred;
        width: 48px;
        height: 48px;
    }

    @keyframes spinner-spin {
        0% {
            transform: rotate(0deg);
        }

        100% {
            transform: rotate(360deg);
        }
    }

</style>
