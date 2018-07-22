<template>
    <form @submit.prevent="submitForm" method="POST">
        <div class="alert alert-danger" style="display: none;">
        </div>
        <div class="alert alert-success" style="display: none;">
        </div>
        <div class="form-group">
            <label asp-for="Where"></label>
            <div class="indent">
                <div class="radio">
                    <label>
                        <input type="radio" asp-for="Where" id="where-halan" value="Hålan" required>
                        Hålan
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <input type="radio" asp-for="Where" id="where-direct" value="Direkt" required>
                        Direkt
                    </label>
                </div>
                <div class="radio">
                    <label>
                        <input type="radio" asp-for="Where" id="where-cant" value="Kan inte komma" required>
                        Kan inte komma
                    </label>
                </div>
            </div>
        </div>
        <div class="form-group">
            <input asp-for="Car">
            <label asp-for="Car"></label>
        </div>
        <div class="form-group">
            <input asp-for="Instrument">
            <label asp-for="Instrument"></label>
        </div>
        <div class="form-group">
            <label asp-for="Comment"></label>
            <input class="form-control" asp-for="Comment">
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
        props: ['event'],
        methods: {
            submitForm(event) {
                console.log(event);
            }
        }
    }
</script>
<style lang="scss">
  
</style>
