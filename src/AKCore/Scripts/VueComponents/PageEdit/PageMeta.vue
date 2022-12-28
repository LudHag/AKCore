<template>
  <div>
    <div class="col-md-6">
      <div class="form-group">
        <label for="name">Namn</label>
        <input
          type="text"
          class="form-control"
          required="required"
          name="Name"
          placeholder="Namn"
          v-model="modelValue.name"
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
          v-model="modelValue.slug"
        />
      </div>
    </div>
    <div class="col-md-3">
      <div class="form-group">
        <label for="slug">Inställningar</label>
        <div class="checkbox">
          <label>
            <input
              type="checkbox"
              name="LoggedIn"
              v-model="modelValue.loggedIn"
            />
            Kräver inloggning
          </label>
        </div>
        <div class="checkbox">
          <label>
            <input
              type="checkbox"
              name="LoggedOut"
              v-model="modelValue.loggedOut"
            />
            Enbart utloggade
          </label>
        </div>
        <div class="checkbox">
          <label>
            <input
              type="checkbox"
              name="BalettOnly"
              v-model="modelValue.balettOnly"
            />
            För baletten
          </label>
        </div>
      </div>
      <button type="submit" class="btn btn-default">Spara</button>
    </div>
  </div>
</template>
<script>
export default {
  props: ["modelValue"],
};
</script>
<style lang="scss"></style>
