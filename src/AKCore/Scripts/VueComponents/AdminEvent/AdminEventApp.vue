<template>
    <div id="admin-event-app">
        <div class="row">
            <div class="col-sm-2">
                <a href="#" class="btn btn-primary" id="create-new-event">Lägg till händelse</a>
            </div>
            <div class="col-sm-3">
                <form>
                    <div class="form-group">
                        <select id="sort-future-select" name="Future" class="form-control">
                            <option>Kommande</option>
                            <option>Gamla</option>
                        </select>
                    </div>
                </form>
            </div>
        </div>
    </div>
</template>
<script>

    export default {
        data() {
            return {
                adminEventData: null
            }
        },
        created() {
            const self = this;
        }
    }
</script>
<style lang="scss">

</style>
