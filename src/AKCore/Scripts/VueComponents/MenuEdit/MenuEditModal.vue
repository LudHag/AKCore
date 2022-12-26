<template>
  <modal :show-modal="showModal" :header="'Redigera meny'" @close="close">
    <div slot="body" class="modal-body">
      <form
        :action="formEndpoint"
        method="post"
        @submit.prevent="submitForm"
        ref="editform"
      >
        <div class="alert alert-danger" style="display: none"></div>
        <input
          type="hidden"
          name="parentId"
          class="parentId"
          :value="parentId"
        />
        <input type="hidden" name="menuId" class="menuId" :value="menuId" />
        <div class="form-group">
          <label>Namn</label>
          <input
            type="text"
            class="form-control name"
            name="text"
            placeholder="Namn"
            :value="menuName"
            required
          />
        </div>
        <div class="form-group">
          <select
            name="pageId"
            class="form-control"
            :value="linkId"
            placeholder="Sida"
            required
          >
            <option value>Välj en sida</option>
            <option :value="page.id" v-for="page in pages" :key="page.id">
              {{ page.name }}
            </option>
          </select>
        </div>
        <div class="checkbox" v-if="!parentId">
          <label>
            <input
              name="loggedIn"
              class="logged"
              :checked="menuLoggedIn"
              type="checkbox"
            />
            Kräv inloggning
          </label>
        </div>
        <div class="checkbox" v-if="!parentId">
          <label>
            <input
              name="balett"
              class="balett"
              type="checkbox"
              :checked="menuBalett"
            />
            Visa enbart för balett
          </label>
        </div>
      </form>
    </div>
    <div slot="footer" class="modal-footer">
      <button type="button" class="btn btn-default" @click.prevent="close">
        Stäng
      </button>
      <button type="submit" class="btn btn-primary" @click.prevent="submitForm">
        Spara
      </button>
    </div>
  </modal>
</template>
<script>
import ApiService from '../../services/apiservice';
import Modal from '../Modal.vue';

export default {
  props: ['menu', 'parentId', 'showModal', 'pages'],
  components: {
    Modal,
  },
  data() {
    return {
      menuLoggedIn: false,
      menuBalett: false,
    };
  },
  computed: {
    menuName() {
      if (this.menu) {
        return this.menu.name;
      } else {
        return '';
      }
    },
    linkId() {
      if (this.menu && this.menu.linkId > 0) {
        return this.menu.linkId;
      } else {
        return '';
      }
    },
    menuId() {
      if (this.menu && this.menu.id > 0) {
        return this.menu.id;
      } else {
        return '';
      }
    },
    formEndpoint() {
      return this.menu ? '/MenuEdit/EditMenu' : '/MenuEdit/AddSubMenu';
    },
  },
  methods: {
    close() {
      this.$emit('close');
    },
    submitForm(event) {
      const error = $('.alert-danger');
      const form = $(this.$refs.editform);
      ApiService.defaultFormSend(form, error, null, () => {
        this.$emit('update');
        this.close();
      });
    },
    updateValues() {
      if (this.menu) {
        this.menuLoggedIn = this.menu.menuLoggedIn;
        this.menuBalett = this.menu.menuBalett;
      }
    },
  },
  watch: {
    menu() {
      this.updateValues();
    },
  },
  created() {
    this.updateValues();
  },
};
</script>
<style lang="scss" scoped></style>
