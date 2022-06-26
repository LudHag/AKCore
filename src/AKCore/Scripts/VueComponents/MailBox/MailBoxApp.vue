<template>
  <div id="mailbox-app">
    <mail-box-form></mail-box-form>
    <div v-if="mailBoxItems.length">
      <h2>Brevlådeposter</h2>
      <div
        class="row mailpost"
        v-for="mailitem in mailBoxItems"
        :key="mailitem.id"
      >
        <div class="subject col-xs-12">
          <span class="date"> {{ formatDateMethod(mailitem.created) }}</span> -
          {{ mailitem.subject }}
        </div>
        <div class="message col-xs-12">{{ mailitem.message }}</div>
      </div>
    </div>
  </div>
</template>
<script>
import ApiService from "../../services/apiservice";
import { formatDate } from "../../utils/functions";
import MailBoxForm from "./MailBoxForm";

export default {
  components: {
    MailBoxForm,
  },
  data() {
    return {
      mailBoxItems: [],
    };
  },
  methods: {
    loadMediaList() {
      if (isBoard) {
        ApiService.get("/MailBox/GetItems", null, (response) => {
          this.mailBoxItems = response;
          console.log(this.mailBoxItems);
        });
      }
    },
    formatDateMethod(date) {
      return formatDate(date);
    },
  },
  created() {
    this.loadMediaList();
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
}

.date {
  color: #a19f9f;
}
</style>
