<template>
  <div class="row" id="search-widget">
    <div class="col-md-12">
      <div class="form-inline ak-search">
        <div class="form-group">
          <input
            type="text"
            v-model="searchPhrase"
            class="form-control"
            placeholder="Sök här"
          />
        </div>
        <div class="form-group">
          <select class="form-control" v-model="selectedInstrument">
            <option value>Sök efter instrument</option>
            <option v-for="i in instruments" :key="i">{{ i }}</option>
          </select>
        </div>
      </div>
      <div id="adress-register">
        <h1>Adressregister</h1>
        <div class="adress-list">
          <div v-for="instr in instruments" :key="instr">
            <h2
              v-html="instr"
              v-if="filteredMembers[instr] && filteredMembers[instr].length > 0"
            ></h2>
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
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
<script>
export default {
  props: ['members', 'instruments'],
  data: function () {
    return {
      selectedInstrument: '',
      searchPhrase: '',
    };
  },
  methods: {
    instrumentMembers: function (instr) {
      return this.members[instr];
    },
  },
  computed: {
    filteredMembers: function () {
      if (this.selectedInstrument === '' && this.searchPhrase === '') {
        return this.members;
      }
      const filteredList = {};
      const lsearch = this.searchPhrase.toLowerCase();
      this.instruments.forEach((instr) => {
        if (
          !this.members[instr] ||
          (this.selectedInstrument !== '' && this.selectedInstrument !== instr)
        ) {
          filteredList[instr] = [];
        } else if (lsearch === '') {
          filteredList[instr] = this.members[instr];
        } else {
          filteredList[instr] = this.members[instr].filter((el) => {
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
    },
  },
};
</script>
<style lang="scss" scoped>
@import 'bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss';
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
}
</style>
