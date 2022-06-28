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
<script>
import ApiService from "../../services/apiservice";
import MailBoxForm from "./MailBoxForm";
import MailBoxItem from "./MailBoxItem";

export default {
  components: {
    MailBoxForm,
    MailBoxItem,
  },
  data() {
    return {
      mailBoxItems: [],
      isArchived: false,
    };
  },
  methods: {
    isUserBoard() {
      return isBoard;
    },
    loadMediaList() {
      if (isBoard) {
        ApiService.get(
          "/MailBox/GetItems?archived=" + this.isArchived,
          null,
          (response) => {
            this.mailBoxItems = response;
          }
        );
      }
    },
    archive(id) {
      ApiService.postByUrl(`/MailBox/${id}/Archive`, null, null, (response) => {
        this.loadMediaList();
      });
    },
    remove(id) {
      if (window.confirm("är du säker på att du vill ta bort post?")) {
        ApiService.postByUrl(
          `/MailBox/${id}/Delete`,
          null,
          null,
          (response) => {
            this.loadMediaList();
          }
        );
      }
    },

    filterChange() {
      this.loadMediaList();
    },
  },
  created() {
    this.loadMediaList();
  },
};
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
