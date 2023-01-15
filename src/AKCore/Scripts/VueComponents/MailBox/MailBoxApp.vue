<template>
  <div id="mailbox-app">
    <mail-box-form @sent="loadMediaList"></mail-box-form>
    <div v-if="isUserBoard()">
      <div class="list-header">
        <h2>Brevlådeposter</h2>
        <div class="toggle">
          <input
            class="toggle-switch"
            id="archived-mailbox-flip"
            type="checkbox"
            v-model="isArchived"
            @change="filterChange()"
          />
          <label
            class="toggle-switch-btn"
            for="archived-mailbox-flip"
            data-tg-off="Aktiva"
            data-tg-on="Arkiverade"
          ></label>
        </div>
      </div>
      <mail-box-item
        v-for="mailitem in mailBoxItems"
        :key="mailitem.id"
        :mailitem="mailitem"
        @archive="archive(mailitem.id)"
        @remove="remove(mailitem.id)"
      ></mail-box-item>
    </div>
  </div>
</template>
<script setup lang="ts">
import { onMounted, ref } from "vue";
import ApiService from "../../services/apiservice";
import MailBoxForm from "./MailBoxForm.vue";
import MailBoxItem from "./MailBoxItem.vue";
import { MailItem } from "./models";

const mailBoxItems = ref<MailItem[]>([]);
const isArchived = ref(false);

//@ts-ignore
// eslint-disable-next-line no-undef
const isUserBoard = (): boolean => isBoard;

const loadMediaList = () => {
  if (isUserBoard()) {
    ApiService.get(
      "/MailBox/GetItems?archived=" + isArchived.value,
      null,
      (response: MailItem[]) => {
        mailBoxItems.value = response;
      }
    );
  }
};

const archive = (id: number) => {
  ApiService.postByUrl(`/MailBox/${id}/Archive`, null, null, () => {
    loadMediaList();
  });
};

const remove = (id: number) => {
  if (window.confirm("är du säker på att du vill ta bort post?")) {
    ApiService.postByUrl(`/MailBox/${id}/Delete`, null, null, () => {
      loadMediaList();
    });
  }
};

const filterChange = () => {
  loadMediaList();
};

onMounted(() => {
  loadMediaList();
});
</script>
<style lang="scss" scoped>
@import "../../../Styles/variables.scss";

.list-header {
  position: relative;
}

.toggle {
  position: absolute;
  right: 0;
  top: 0;
}
</style>
