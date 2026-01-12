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
      <a href="#" @click.prevent="expand" class="expand btn btn-primary">
        Expandera
      </a>
    </div>
  </div>
</template>
<script setup lang="ts">
import { ref, computed } from "vue";
import { formatDateNoTime } from "@utils/functions";
import { MailItem } from "./models";

const emit = defineEmits<{
  (e: "archive", mailItem: MailItem): void;
  (e: "remove", mailItem: MailItem): void;
}>();

const props = defineProps<{
  mailitem: MailItem;
}>();

const expanded = ref(false);

const showExpand = computed(() => {
  return !expanded.value && props.mailitem.message.length > 200;
});

const calculatedMessage = computed(() => {
  if (expanded.value || props.mailitem.message.length <= 200) {
    return props.mailitem.message;
  } else {
    return props.mailitem.message.substring(0, 200) + "...";
  }
});

const formatDateMethod = (date: string) => {
  return formatDateNoTime(date);
};

const expand = () => {
  expanded.value = true;
};

const archive = () => {
  emit("archive", props.mailitem);
};

const remove = () => {
  emit("remove", props.mailitem);
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
