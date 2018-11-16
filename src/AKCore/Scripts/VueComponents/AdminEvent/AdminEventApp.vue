<template>
    <div id="admin-event-app">
        <div class="row">
            <div class="col-sm-6">
                <a href="#" class="btn btn-primary" @click.prevent="openNewEvent">Lägg till händelse</a>
            </div>
            <div class="col-sm-6">
                <select class="form-control" @change="newSort">
                    <option value="Kommande">Kommande</option>
                    <option value="Gamla">Gamla</option>
                </select>
            </div>

        </div>
        <div class="row">
            <div class="col-xs-12">
                <div class="alert alert-success" style="display: none;">
                </div>
                <div class="alert alert-error" style="display: none;">
                </div>
            </div>
        </div>
        <spinner :size="'medium'" v-if="!adminEventData"></spinner>
        <div v-if="adminEventData">
            <h1>Händelser:</h1>
            <div class="row event-row"
                    v-for="e in adminEventData.events"
                    @click="openEvent(e)">
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
                    <a href="#" class="remove-event glyphicon glyphicon-remove" @click.prevent.stop="removeEvent(e)"></a>
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
        <admin-event-modal 
            v-if="adminEventData"
            :show-modal="eventModalOpened"
            :old="adminEventData.old"
            :selected-event="modalEvent"
            @update="eventUpdated"
            @close="closeModal">
        </admin-event-modal>
    </div>
</template>
<script>
    import Spinner from "../Spinner";
    import AdminEventModal from "./AdminEventModal";
    import ApiService from '../../services/apiservice';

    export default {
        components: {
            Spinner,
            AdminEventModal
        },
        data() {
            return {
                adminEventData: null,
                eventModalOpened: false,
                modalEvent: null
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
                this.loadEvents(e.target.value === "Gamla", 1);
            },
            removeEvent(e) {
                if (confirm("Är du säker på att du vill ta bort event: " + e.day + " " + e.name)) {
                    const error = $(".alert-danger");
                    const success = $(".alert-success");
                    ApiService.postByUrl("/AdminEvent/Remove/" + e.id, error, success, () => {
                        this.loadEvents(this.adminEventData.old, this.adminEventData.currentPage);
                    });
                }
            },
            openEvent(e) {
                this.modalEvent = e;
                this.eventModalOpened = true;
            },
            openNewEvent() {
                this.modalEvent = null;
                this.eventModalOpened = true;
            },
            closeModal() {
                this.eventModalOpened = false;
            },
            eventUpdated() {
                this.closeModal();
                this.loadEvents(this.adminEventData.old, this.adminEventData.currentPage);
            },
            toPage(i) {
                this.loadEvents(this.adminEventData.old, i);
            },
            loadEvents(old, page) {
                const self = this;
                ApiService.get("/AdminEvent/EventData?old=" + old + "&page=" + page, null, (res) => {
                    self.adminEventData = res;
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
