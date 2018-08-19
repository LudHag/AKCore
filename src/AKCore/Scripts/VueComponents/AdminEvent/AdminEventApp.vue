<template>
    <div id="admin-event-app">
        <div class="row">
            <div class="col-sm-6">
                <a href="#" class="btn btn-primary">Lägg till händelse</a>
            </div>
            <div class="col-sm-6">
                <select class="form-control" @change="newSort">
                    <option value="Kommande">Kommande</option>
                    <option value="Gamla">Gamla</option>
                </select>
            </div>
        </div>
        <spinner :size="'medium'" v-if="!adminEventData"></spinner>
        <div v-if="adminEventData">
            <h1>Händelser:</h1>
            <div class="row event-row" v-for="e in adminEventData.events">
                <div class="col-sm-2">
                    <p>{{e.day}}</p>
                </div>
                <div class="col-sm-4">
                    <p>{{e.name}}</p>
                </div>
                <div class="col-sm-4">
                    <p>{{e.type}}</p>
                </div>
                <div class="col-sm-2">
                    <a href="#" class="remove-event glyphicon glyphicon-remove" @click.prevent="removeEvent(e)"></a>
                </div>
            </div>
            <div class="row" v-if="adminEventData.totalPages > 1">
                <div class="col-xs-12">
                    <ul class="pagination">
                        <li v-for="i in paginationPages"
                            v-bind:class="{ active: adminEventData.currentPage === i }">
                            <a v-if="i !== 0" href="#" @click.prevent="toPage(i)">{{i}}</a>
                            <span v-if="i === 0" class="dots">...</span>
                        </li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</template>
<script>
    import Spinner from "../Spinner";
    export default {
        components: {
            Spinner
        },
        data() {
            return {
                adminEventData: null
            }
        },
        computed: {
            paginationPages() {
                const total = this.adminEventData.totalPages;
                const current = this.adminEventData.currentPage;

                const pages = [];
                let gap = false;
                for (let i = 1; i <= total; i++) {
                    if (i <= 2 || (i >= current - 1 && i <= current + 1) || i >= total - 1) {
                        pages.push(i);
                        gap = false;
                    } else if(!gap) {
                        pages.push(0);
                        gap = true;
                    }
                }

                return pages;
            }
        },
        methods: {
            newSort(e) {
                this.loadEvents(e.target.value === "Gamla", this.adminEventData.currentPage);
            },
            removeEvent(e) {
                console.log(e);
            },
            toPage(i) {
                this.loadEvents(this.adminEventData.old, i);
            },
            loadEvents(old, page) {
                const self = this;
                $.ajax({
                    url: "/AdminEvent/EventData?old=" + old + "&page=" + page,
                    type: "GET",
                    success: function (res) {
                        self.adminEventData = res;
                    }
                });
            }
        },
        created() {
            this.loadEvents(false, 1);
        }
    }
</script>
<style lang="scss">

</style>
