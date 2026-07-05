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
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-align-left"></span>
                <span class="glyphicon glyphicon-picture"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("TextImage") }}</span>
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('Text')">
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-align-justify nomargin"></span>
                <span class="glyphicon glyphicon-align-left"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("Text") }}</span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Image')"
            >
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-picture"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("Image") }}</span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Video')"
            >
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-facetime-video"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("Video") }}</span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('Music')"
            >
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-music"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("Music") }}</span>
            </a>
          </li>
          <li>
            <a
              href="#"
              @click.prevent="click('HeaderText')"
            >
              <span class="widget-choice-icons">
                <span class="glyphicon glyphicon-header"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("HeaderText") }}</span>
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('TextOverlap')">
              <span class="widget-choice-icons widget-choice-icons--overlap">
                <span class="glyphicon glyphicon-align-left"></span>
                <span class="glyphicon glyphicon-picture"></span>
                <span class="glyphicon glyphicon-arrow-left widget-choice-overlap-arrow"></span>
              </span>
              <span class="widget-choice-label">{{ getHeader("TextOverlap") }}</span>
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
            <a href="#" @click.prevent="click('CountDown')"> Nedräknare </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('ThreePuffs')"> Tre puffar </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('VideosHeader')">
              Videos rubrik med sök
            </a>
          </li>
          <li>
            <a href="#" @click.prevent="click('StartPageHero')">
              Startsida hero 
            </a>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import { getHeader } from "./functions";

const emit = defineEmits<{
  (e: "add", type: string): void;
}>();

const widgetExanded = ref(false);
const specialWidgetExpanded = ref(false);

const click = (type: string) => {
  emit("add", type);
  specialWidgetExpanded.value = false;
  widgetExanded.value = false;
};

onMounted(() => {
  document.addEventListener("click", (e: Event) => {
    const target = e.target as HTMLElement;

    if (!target.closest(".dropdown-normal")) {
      widgetExanded.value = false;
    }
    if (!target.closest(".dropdown-special")) {
      specialWidgetExpanded.value = false;
    }
  });
});
</script>
<style lang="scss"></style>
