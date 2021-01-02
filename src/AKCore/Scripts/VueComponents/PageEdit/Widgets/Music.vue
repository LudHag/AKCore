<template>
  <div class="col-sm-12">
    <select multiple="multiple" class="multi-select album" asp-for="Albums">
      @foreach (var a in albums) {
      <option value="@a.Id">@a.Name</option>
      }
    </select>
  </div>
</template>
<script>
import TextEdit from "../WidgetParts/TextEdit.vue";
export default {
  components: { TextEdit },
  props: ["value"]
};
</script>
<style lang="scss"></style>
