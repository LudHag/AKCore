<template>
  <form
    @submit.prevent="submitForm"
    :action="'/upcoming/Signup/' + eventInfo.event.id"
    method="POST"
  >
    <div class="alert alert-danger" style="display: none"></div>
    <div class="alert alert-success" style="display: none"></div>
    <div class="form-group">
      <label>Kommer till:</label>
      <div class="indent">
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Hålan"
              required
            />
            Hålan
          </label>
        </div>
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Direkt"
              required
            />
            Direkt
          </label>
        </div>
        <div class="radio">
          <label>
            <input
              type="radio"
              name="Where"
              v-model="where"
              value="Kan inte komma"
              required
            />
            Kan inte komma
          </label>
        </div>
      </div>
    </div>
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="car" />
        <input type="hidden" name="Car" v-model="car" />
        Har bil
      </label>
    </div>
    <div class="checkbox">
      <label>
        <input type="checkbox" v-model="instrument" />
        <input type="hidden" name="Instrument" v-model="instrument" />
        Tar med instrument själv
      </label>
    </div>
    <div class="form-group">
      <label>Kommentar</label>
      <input
        class="form-control"
        name="Comment"
        type="text"
        v-model="comment"
      />
    </div>
    <div class="form-group">
      <button type="submit" class="btn btn-default">Anmäl</button>
    </div>
  </form>
</template>
<script>
import Spinner from '../Spinner.vue';

export default {
  components: {
    Spinner,
  },
  data() {
    return {
      where: null,
      car: false,
      instrument: false,
      comment: null,
    };
  },
  props: ['eventInfo'],
  methods: {
    submitForm(event) {
      const self = this;
      const form = $(event.target);
      const error = form.find('.alert-danger');
      const success = form.find('.alert-success');
      $.ajax({
        url: form.attr('action'),
        type: 'POST',
        data: form.serialize(),
        success: function (res) {
          if (res.success) {
            self.$emit('update');
            success.text('Anmälan uppdaterad');
            success.slideDown().delay(3000).slideUp();
          } else {
            error.text(res.message);
            error.slideDown().delay(4000).slideUp();
          }
        },
        error: function () {
          error.text('Misslyckades med att anmäla dig');
          error.slideDown().delay(4000).slideUp();
        },
      });
    },
    loadForm() {
      this.where = this.eventInfo.where;
      this.comment = this.eventInfo.comment;
      this.instrument = this.eventInfo.instrument;
      this.car = this.eventInfo.car;
    },
  },
  activated() {
    this.loadForm();
  },
  created() {
    this.loadForm();
  },
};
</script>
<style lang="scss"></style>
