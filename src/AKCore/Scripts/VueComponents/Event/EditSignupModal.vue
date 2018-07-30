<template>
    <div>
        <div class="modal show" @click="close">
            <div class="modal-dialog modal-sm">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" @click.prevent="close">&times;</button>
                        <h4 class="modal-title">Lägg till anmälan</h4>
                    </div>
                    <form action="/upcoming/EditSignup" method="POST" @submit.prevent="submitForm">
                        <div class="modal-body">
                            <div class="alert alert-danger" style="display: none;">
                            </div>
                            <div class="row">
                                <div class="col-sm-12">
                                    <input type="hidden" name="eventId" :value="eventId">
                                    <div class="form-group">
                                        <label>Medlem</label>
                                        <select name="memberId" class="form-control" required>
                                            <option value="">Välj en medlem</option>
                                            <option v-for="member in members" :value="member.id">{{member.fullName}}</option>
                                        </select>
                                    </div>
                                    <div class="form-group">
                                        <label>Status</label>
                                        <select class="form-control" name="type" required>
                                            <option value="">Välj anmälningstyp</option>
                                            <option v-for="signupType in signupTypes">{{signupType}}</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <button class="btn btn-primary" type="reset">Återställ</button>
                            <button type="submit" class="btn btn-primary">Spara</button>
                        </div>
                    </form>
                </div>
            </div>
        </div>
        <div class="modal-backdrop fade in"></div>
    </div>
</template>
<script>
    import Constants from '../../constants';

    export default {
        props: ['members', 'eventId'],
        computed: {
            signupTypes() {
                return Constants.SIGNUPTYPES;
            }
        },
        methods: {
            submitForm(event) {
                const form = $(event.target);
                const self = this;
                const error = form.find(".alert-danger");
                const success = $(".alert-success");
                $.ajax({
                    url: form.attr("action"),
                    type: "POST",
                    data: form.serialize(),
                    success: function (res) {
                        if (res.success) {
                            success.text("Anmälan uppdaterad");
                            success.slideDown().delay(3000).slideUp();
                            form.trigger("reset");
                            self.$emit("update");
                            self.$emit("close");
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function () {
                        error.text("Misslyckades med att anmäla dig");
                        error.slideDown().delay(4000).slideUp();
                    }
                });
            },
            close(event) {
                if (event.target.classList.contains("modal") ||
                    event.target.classList.contains("close")) {
                    this.$emit("close");
                }
            }
        }
    }
</script>
<style lang="scss">
  
</style>