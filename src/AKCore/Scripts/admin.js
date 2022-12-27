import Vue from 'vue/dist/vue.esm.js';
import './Admin/users.js';
import './Admin/pageedit.js';
import PageEdit from './VueComponents/PageEdit/PageEdit.vue';

if ($('#pageedit-app').length > 0) {
  new Vue({
    render: (h) => h(PageEdit),
  }).$mount('#pageedit-app');
}
