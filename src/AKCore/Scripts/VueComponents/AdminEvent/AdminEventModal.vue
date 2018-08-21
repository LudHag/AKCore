<template>
    <div class="modal show" @click="close">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" @click.prevent="close">&times;</button>
                    <h4 class="modal-title">Redigera händelse</h4>
                </div>
                <form action="/AdminEvent/Edit" method="post">
                    <div class="modal-body">
                        <div class="alert alert-success" style="display: none;">
                        </div>
                        <div class="alert alert-danger" style="display: none;">
                        </div>
                        <input type="hidden" name="Id" :value="eventId" />
                        <div class="form-group">
                            <label>Typ</label>
                            <select class="form-control" name="Type" 
                                    v-model="eventType" required>
                                <option value="">Typ av händelse</option>
                                <option v-for="e in eventTypes">{{e}}</option>
                            </select>
                        </div>
                        <div class="editeventbody">
                            <div class="form-group spelning-fest">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <label asp-for="Name"></label>
                                        <input class="form-control" asp-for="Name">
                                    </div>
                                    <div class="col-sm-6 only-spelning">
                                        <label> </label>
                                        <div class="checkbox checkbox-center">
                                            <label>
                                                <input type="checkbox" asp-for="Secret"> Hemlig spelning
                                            </label>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group place">
                                <div class="row">
                                    <div class="col-sm-6">
                                        <label asp-for="Place"></label>
                                        <input class="form-control" asp-for="Place">
                                    </div>
                                    <div class="col-sm-6 sta-ga">
                                        <label asp-for="Stand"></label>
                                        <select class="form-control" asp-for="Stand">
                                            <option value="">Välj speltyp</option>
                                            @foreach (var sp in AkSpeltyp.Speltyper)
                                            {
                                            <option>@sp</option>
                                            }
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-sm-3">
                                        <label asp-for="Day"></label>
                                        <input type="datetime" class="form-control datepicker" asp-for="Day" required>
                                    </div>
                                    <div class="col-sm-3">
                                        <label asp-for="Halan"></label>
                                        <input class="form-control" type="time" asp-for="Halan" value="00:00">
                                    </div>
                                    <div class="col-sm-3 spelning-tid-there">
                                        <label asp-for="There"></label>
                                        <input class="form-control" type="time" asp-for="There" value="00:00">
                                    </div>
                                    <div class="col-sm-3 spelning-tid">
                                        <label asp-for="Starts"></label>
                                        <input class="form-control" type="time" asp-for="Starts" value="00:00" required>
                                    </div>
                                    <div class="col-sm-6 rep-fika">
                                        <label asp-for="Fika"></label>
                                        <select class="form-control" asp-for="Fika">
                                            <option value="">Välj en sektion</option>
                                            @foreach (var inst in AkFika.Sektioner)
                                            {
                                            <option>@inst</option>
                                            }
                                        </select>
                                    </div>
                                </div>
                            </div>

                            <div class="form-group description">
                                <label asp-for="Description"></label>
                                <textarea class="form-control" asp-for="Description"></textarea>
                            </div>
                            <div class="form-group">
                                <label asp-for="InternalDescription"></label>
                                <textarea class="form-control" asp-for="InternalDescription"></textarea>
                            </div>
                        </div>

                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-default" data-dismiss="modal">Stäng</button>
                        <button type="submit" class="btn btn-primary">Spara</button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>
<script>
    import Constants from '../../constants';

    export default {
        props: ['selectedEvent'],
        data() {
            return {
                eventType: ""
            }
        },
        methods: {
            close() {
                if (event.target.classList.contains("modal") ||
                    event.target.classList.contains("close")) {
                    this.$emit("close");
                }
            }
        },
        computed: {
            eventId() {
                if (!this.selectedEvent) {
                    return 0;
                }
                return this.selectedEvent.id;
            },
            eventTypes() {
                return Constants.EVENTTYPES;
            },
        },
        created() {
            this.eventType = this.selectedEvent ? this.selectedEvent.type : "";
        }
    }
</script>
<style lang="scss">

</style>
