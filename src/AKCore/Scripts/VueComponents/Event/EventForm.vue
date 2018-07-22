<template>
    <form @submit.prevent="submitForm" method="POST">
        <div class="alert alert-danger" style="display: none;">
        </div>
        <div class="alert alert-success" style="display: none;">
        </div>
        <div class="form-group">
            <label>Kommer till:</label>
            <div class="indent">
                <div class="radio">
                    <label>
                        <input type="radio" name="Where" v-model="where" value="Hålan" required>
                        Hålan
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <input type="radio" name="Where" v-model="where" value="Direkt" required>
                        Direkt
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <input type="radio" name="Where" v-model="where" value="Kan inte komma" required>
                        Kan inte komma
                    </label>
                </div>
            </div>
        </div>
        <div class="form-group">
            <input type="checkbox" name="Car" v-model="car">
            <label>Har bil</label>
        </div>
        <div class="form-group">
            <input type="checkbox" name="Instrument" v-model="instrument">
            <label>Tar med instrument själv</label>
        </div>
        <div class="form-group">
            <label>Kommentar</label>
            <input class="form-control" name="Comment" type="text" v-model="comment">
        </div>
        <div class="form-group">
            <button type="submit" class="btn btn-default">Anmäl</button>
        </div>
    </form>
              
</template>
<script>
    import Spinner from "../Spinner";

    export default {
        components: {
            Spinner
        },
        data() {
            return {
                where: null,
                car: false,
                instrument: false,
                comment: null
            }
        },
        props: ['eventInfo'],
        methods: {
            submitForm(event) {
                console.log(event);
            },
            loadForm() {
                this.where = this.eventInfo.where;
                this.comment = this.eventInfo.comment;
                this.instrument = this.eventInfo.instrument;
                this.car = this.eventInfo.car;
            }
        },
        activated() {
            this.loadForm();
        },
        created() {
            this.loadForm();
        }
    }
</script>
<style lang="scss">
  
</style>
