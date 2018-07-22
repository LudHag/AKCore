<template>
    <div class="row event-row" @click.prevent="expanded=!expanded" v-bind:class="{ expandable, expanded }">
        <div class="col-sm-4" style="font-weight: 500;">
            <p style="text-transform: capitalize;">{{event.day}} </p>
        </div>
        <div class="col-sm-4">
            <p>{{event.name}}</p>
            <p>{{event.place}}</p>
        </div>
        <div class="col-sm-4">
            <p>Spelning startar: {{event.startsTime}}</p>
        </div>
        <div class="extra" v-if="expanded">
            <div class="col-sm-12 description">
                <p>{{event.description}}</p>
            </div>
        </div>
    </div>
</template>
<script>
    export default {
        props: ['event', 'loggedIn', 'member'],
        data() {
            return {
                expanded: false
            }
        },
        computed: {
            expandable() {
                return this.event.description || this.event.internalDescription;
            }
        }
    }
</script>
<style lang="scss">
    @import "../../../Styles/variables.scss";

    .event-row.expandable:before {
        content: "+";
        color: $akred;
        position: absolute;
        right: 6px;
        top: 3px;
        font-size: 25px;
        line-height: 25px;
    }
    .event-row.expandable.expanded.extra {
        display: block;
    }
    .event-row.expandable.expanded:before {
        content: "-";
        right: 6px;
        top: 0px;
        font-size: 50px;
    }
    .event-row.expandable a {
        margin: 0 0 10px;
        display: block;
    }

    .event-row {
        padding: 15px;
        border: 3px solid $akred;
        border-radius: 7px;
        cursor: pointer;
        position: relative;
    }
    .event-row:hover {
        background-color: #1a0000;
    }

    .green {
        color: #02C66F;
    }

</style>
