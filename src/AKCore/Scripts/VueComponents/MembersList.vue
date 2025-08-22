<template>
  <div id="search-widget">
    <div class="col-md-12">
      <div class="form-inline ak-search">
        <div class="form-group search-group">
          <input
            type="text"
            v-model="searchPhrase"
            class="form-control"
            :placeholder="t('search-here')"
          />
        </div>
        <div class="form-group">
          <select class="form-control" v-model="selectedInstrument">
            <option value>{{ t("search-for-instruments") }}</option>
            <option v-for="i in instruments" :key="i" :value="i">
              {{ t(i, "instruments") }}
            </option>
          </select>
        </div>
      </div>
      <div id="adress-register">
        <h1>{{ t("adress-register") }}</h1>
        <div class="adress-list">
          <div v-for="instr in instruments" :key="instr">
            <h2
              v-if="filteredMembers[instr] && filteredMembers[instr].length > 0"
            >
              {{ t(instr, "instruments") }}
            </h2>
            <div
              class="kamerer"
              v-for="member in filteredMembers[instr]"
              :key="member.email + member.name"
            >
              <div class="name-email">
                {{ member.name }}
                <br />
                <a :href="'mailto:' + member.email">{{ member.email }}</a>
              </div>
              <div class="phone-nation">
                <a :href="'tel:' + member.phone">{{ member.phone }}</a>
              </div>
              <div class="">
               {{ t("food-preference") }}: {{ member.foodPreference }}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from "vue";
import { Member } from "./models";
import { TranslationDomain, translate } from "../translations";

const props = defineProps<{
  members: Record<string, Array<Member>>;
  instruments: string[];
}>();

const selectedInstrument = ref("");
const searchPhrase = ref("");

const filteredMembers = computed(() => {
  if (selectedInstrument.value === "" && searchPhrase.value === "") {
    return props.members;
  }
  const filteredList: Record<string, Array<Member>> = {};
  const lsearch = searchPhrase.value.toLowerCase();
  props.instruments.forEach((instr) => {
    if (
      !props.members[instr] ||
      (selectedInstrument.value !== "" && selectedInstrument.value !== instr)
    ) {
      filteredList[instr] = [];
    } else if (lsearch === "") {
      filteredList[instr] = props.members[instr];
    } else {
      filteredList[instr] = props.members[instr].filter((el) => {
        return (
          el.name.toLowerCase().indexOf(lsearch) >= 0 ||
          el.email.toLowerCase().indexOf(lsearch) >= 0 ||
          el.phone.indexOf(lsearch) >= 0 ||
          el.instrument.toLowerCase().indexOf(lsearch) >= 0
        );
      });
    }
  });
  return filteredList;
});

const t = (key: string, domain: TranslationDomain = "memberlist") => {
  return translate(domain, key);
};
</script>
<style lang="scss" scoped>
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";
#adress-register .kamerer {
  padding-bottom: 4px;
}

#adress-register .kamerer:nth-of-type(n + 2) {
  border-top: solid 1px #333333;
  padding-top: 4px;
}

#adress-register .kamerer > div {
  display: inline-block;
  vertical-align: top;
}

#adress-register .kamerer div:first-child {
  width: 20em;
}

#adress-register .kamerer div:nth-child(2) {
  width: 20em;
}

#adress-register .kamerer div:nth-child(3) {
  width: 15em;
}

#adress-register a {
  white-space: nowrap;
}

.search-group {
  margin-right: 20px;
}

@media screen and (max-width: $screen-xs-max) {
  #adress-register .kamerer > div {
    &.name-email {
      width: 60%;
      overflow: hidden;
    }

    &.phone-nation {
      width: 40%;
      text-align: right;
      float: right;
      overflow: hidden;
    }
  }
  .search-group {
    margin-right: 0;
  }
}
</style>
