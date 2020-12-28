<template>
  <div id="pageedit-app" v-if="pageModel">
    <div class="row">
      <form method="post" @submit.prevent="save">
        <div class="col-md-6">
          <div class="alert alert-danger" style="display: none;"></div>
          <div class="alert alert-success" style="display: none;"></div>
          <div class="form-group">
            <label for="name">Namn</label>
            <input
              type="text"
              class="form-control"
              required="required"
              name="Name"
              placeholder="Namn"
              v-model="pageModel.name"
            />
          </div>
          <div class="form-group">
            <label for="slug">Sidlänk</label>
            <input
              type="text"
              class="form-control"
              required="required"
              name="Slug"
              placeholder="Sidlänk"
              v-model="pageModel.slug"
            />
          </div>
        </div>
        <div class="col-md-3">
          <div class="form-group">
            <label for="slug">Inställningar</label>
            <div class="checkbox">
              <label> <input type="checkbox" /> Startsida </label>
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" v-model="pageModel.loggedIn" /> Kräver
                inloggning
              </label>
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" v-model="pageModel.loggedOut" /> Enbart
                utloggade
              </label>
            </div>
            <div class="checkbox">
              <label>
                <input type="checkbox" v-model="pageModel.balettOnly" /> För
                baletten
              </label>
            </div>
          </div>
          <button type="submit" class="btn btn-default">
            Spara
          </button>
        </div>
      </form>
    </div>
  </div>
</template>
<script>
import Constants from "../../constants";

export default {
  data() {
    return {
      pageModel: null
    };
  },
  created() {
    const self = this;
    $.ajax({
      url: window.location.href + "/Model",
      type: "GET",
      success: function(res) {
        self.pageModel = res;
      }
    });
  },
  methods: {
    save(e) {
      console.log(e);
    }
  }
};
</script>
<style lang="scss"></style>
