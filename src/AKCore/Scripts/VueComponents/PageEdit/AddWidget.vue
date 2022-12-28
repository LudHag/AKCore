<template>
  <div class="row">
    <div class="col-xs-12">
      <div class="dropdown dropdown-normal" :class="{ open: widgetExanded }">
        <button
          class="btn btn-default"
          type="button"
          @click.prevent="widgetExanded = !widgetExanded"
        >
          Lägg till widget
          <span class="caret"></span>
        </button>
        <ul
          class="dropdown-menu widget-choose"
          aria-labelledby="add-widgets-btn"
        >
          <li>
            <a href="#" @click.prevent="click('TextImage')">
              <span class="glyphicon glyphicon-align-left"></span>
              <span class="glyphicon glyphicon-picture"></span>
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('Text')">
              <span class="glyphicon glyphicon-align-justify nomargin"></span>
              <span class="glyphicon glyphicon-align-left"></span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Image')"
              style="text-align: center"
            >
              <span class="glyphicon glyphicon-picture"></span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Video')"
              style="text-align: center"
            >
              <span class="glyphicon glyphicon-facetime-video"></span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Music')"
              style="text-align: center"
            >
              <span class="glyphicon glyphicon-music"></span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('HeaderText')"
              style="text-align: center"
            >
              <span class="glyphicon glyphicon-header"></span>
            </a>
          </li>
        </ul>
      </div>
      <div
        class="dropdown dropdown-special"
        :class="{ open: specialWidgetExpanded }"
      >
        <button
          class="btn btn-default"
          type="button"
          @click.prevent="specialWidgetExpanded = !specialWidgetExpanded"
        >
          Lägg till specialwidget
          <span class="caret"></span>
        </button>
        <ul
          class="dropdown-menu widget-choose-special"
          aria-labelledby="add-special-widgets"
        >
          <li>
            <a href="#" @click.prevent="click('MemberList')">
              Adressregister
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('PostList')">
              Kamererspostlista
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('Join')"> Gå med-formulär </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('Hire')"> Anlita oss-sida </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('MailBox')"> Anonym brevlåda </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('ThreePuffs')"> Tre puffar </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  data() {
    return {
      widgetExanded: false,
      specialWidgetExpanded: false,
    };
  },
  methods: {
    click(type) {
      this.$emit("add", type);
      this.specialWidgetExpanded = false;
      this.widgetExanded = false;
    },
  },
  created() {
    document.addEventListener("click", (e) => {
      if (!e.target.closest(".dropdown-normal")) {
        this.widgetExanded = false;
      }
      if (!e.target.closest(".dropdown-special")) {
        this.specialWidgetExpanded = false;
      }
    });
  },
};
</script>
<style lang="scss"></style>
