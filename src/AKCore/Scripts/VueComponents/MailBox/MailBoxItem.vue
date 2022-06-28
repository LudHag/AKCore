<template>
  <div class="row mailpost">
    <div class="subject col-xs-12">
      <span class="date"> {{ formatDateMethod(mailitem.created) }}</span> -
      {{ mailitem.subject }}
      <span class="actions">
        <a
          href="#"
          @click.prevent="archive"
          :class="{ green: mailitem.archived }"
          title="Arkivera"
          class="archive glyphicon glyphicon-folder-open"
        ></a>
        <a
          href="#"
          @click.prevent="remove"
          title="Ta bort"
          class="remove glyphicon glyphicon-remove"
        ></a>
      </span>
    </div>
    <div class="message col-xs-12">{{ calculatedMessage }}</div>
    <div v-if="showExpand" class="expand-container col-xs-12">
      <a href="#" @click.prevent="expand" class="expand btn btn-primary"
        >Expandera</a
      >
    </div>
  </div>
</template>
<script>
import { formatDate } from "../../utils/functions";

export default {
  data() {
    return {
      expanded: false,
    };
  },
  props: ["mailitem"],
  computed: {
    showExpand() {
      return !this.expanded && this.mailitem.message.length > 200;
    },
    calculatedMessage() {
      if (this.expanded || this.mailitem.message.length <= 200) {
        return this.mailitem.message;
      } else {
        return this.mailitem.message.substring(0, 200) + "...";
      }
    },
  },
  methods: {
    formatDateMethod(date) {
      return formatDate(date);
    },
    expand() {
      this.expanded = true;
    },
    archive() {
      this.$emit("archive", this.mailitem);
    },
    remove() {
      this.$emit("remove", this.mailitem);
    },
  },
};
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";

.mailpost {
  padding-top: 20px;
}
.mailpost + .mailpost {
  border-top: 2px solid $akred;
}

.subject {
  font-weight: 500;
  font-size: 1.2em;
  min-height: 50px;
}

.date {
  color: #a19f9f;
}
.expand-container {
  height: 34px;
}
.expand {
  position: absolute;
  left: 50%;
  transform: translateX(-50%);
}
.actions {
  position: absolute;
  right: 0;
}
.archive {
  margin-right: 10px;
}
.archive.green {
  color: #02c66f;
}
</style>
