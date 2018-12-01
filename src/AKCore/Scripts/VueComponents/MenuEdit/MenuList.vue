<template>
  <div class="row menu-display">
    <div class="col-sm-12 menu-item" v-for="menu in menus" :key="menu.id">
      <div class="menu-row">
        <a class="btn btn-default menu">{{menu.name}}</a>
        <a class="btn add-sub-menu glyphicon glyphicon-plus"></a>
        <a class="btn remove-menu glyphicon glyphicon-remove"></a>
        <a class="btn move-right glyphicon glyphicon-chevron-down"></a>
        <a class="btn move-left glyphicon glyphicon-chevron-up"></a>
      </div>
      <div
        class="menu-row sub-row"
        v-if="menu.children"
        v-for="child in menu.children"
        :key="child.id"
      >
        <a class="btn submenu">{{child.name}}</a>
        <a class="btn remove-sub-menu glyphicon glyphicon-remove"></a>
        <a class="btn move-down glyphicon glyphicon-chevron-down"></a>
        <a class="btn move-up glyphicon glyphicon-chevron-up"></a>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ["menus"]
};
</script>
<style lang="scss" scoped>
@import "~bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
@import "../../../Styles/variables.scss";
.menu-item {
  margin-bottom: 5px;
}

.menu-row {
  display: inline-block;
  width: 100%;
  &.sub-row {
    margin-top: 5px;
    padding-left: 50px;
  }

  .btn {
    padding: 3px 23px;
  }

  &:hover {
    background-color: #1a0000;
  }
}

.submenu {
  background-color: #999999;
  color: white;
  &:focus {
    background-color: $akred;
    border-color: #6e1601;
    color: white;
  }

  &:hover {
    color: #333;
    background-color: $akwhite;
    border-color: #ccc;
  }
}
@media screen and (max-width: $screen-xs-max) {
  .menu-row.sub-row {
    padding-left: 0;
  }
}
</style>
