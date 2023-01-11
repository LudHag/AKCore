<template>
  <div id="pageedit-app" v-if="pageModel">
    <div class="row">
      <form method="post" @submit.prevent="save">
        <div class="alert alert-danger" style="display: none"></div>
        <div class="alert alert-success" style="display: none"></div>
        <div class="col-xs-12">
          <h2 v-if="selectedRevision">
            Version från {{ selectedRevision.modified }}
          </h2>
        </div>
        <page-meta v-model="usedModel"></page-meta>
        <page-versions
          :modelValue="pageModel"
          :selectedRevision="selectedRevision"
          @select="selectRevision"
        ></page-versions>
      </form>
    </div>
    <add-widget @add="widgetAdd"></add-widget>
    <ul class="widget-area">
      <draggable
        v-model="usedWidgets"
        @start="drag = true"
        @end="drag = false"
        handle=".widget-header"
        item-key="id"
      >
        <template #item="{ element }">
          <widget
            :modelValue="element"
            :albums="pageModel.albums"
            @remove="removeWidget(element)"
            @update:modelValue="updateWidget($event)"
          >
          </widget>
        </template>
      </draggable>
    </ul>
    <image-picker-modal
      v-if="saveImageDest"
      :show-modal="!!saveImageDest"
      :destination="saveImageDest"
      :notransition="true"
      @close="saveImageDest = null"
    ></image-picker-modal>
    <document-picker-modal
      v-if="saveDocumentDest"
      :show-modal="!!saveDocumentDest"
      :destination="saveDocumentDest"
      :notransition="true"
      @close="saveDocumentDest = null"
    >
    </document-picker-modal>
  </div>
</template>
<script setup lang="ts">
import PageMeta from "./PageMeta.vue";
import AddWidget from "./AddWidget.vue";
import Widget from "./Widget.vue";
import ApiService from "../../services/apiservice";
import ImagePickerModal from "../ImagePickerModal.vue";
import DocumentPickerModal from "../DocumentPickerModal.vue";
import draggable from "vuedraggable";
import PageVersions from "./PageVersions.vue";
import { EventBus } from "../../utils/eventbus";
import { onMounted, ref, watch, computed } from "vue";
import {
  PageEditModel,
  PageRevisionEditModel,
  WidgetEditModel,
} from "./models";

const pageModel = ref<PageEditModel | null>(null);
const saveImageDest = ref<JQuery<HTMLElement> | null>(null);
const saveDocumentDest = ref<JQuery<HTMLElement> | null>(null);
const drag = ref(false);
const selectedRevision = ref<PageRevisionEditModel | null>(null);
const usedModel = ref<PageEditModel | PageRevisionEditModel | null>(null);

onMounted(() => {
  EventBus.on("loadimage", (field: any) => {
    selectImage(field);
  });

  EventBus.on("loadfile", (field: any) => {
    selectfile(field);
  });

  $.ajax({
    url: window.location.href + "/Model",
    type: "GET",
    success: function (res: PageEditModel) {
      pageModel.value = res;
      usedModel.value = pageModel.value;
    },
  });
  document.addEventListener(
    "keydown",
    (e) => {
      if (
        (window.navigator.platform.match("Mac") ? e.metaKey : e.ctrlKey) &&
        e.keyCode == 83
      ) {
        e.preventDefault();
        save();
      }
    },
    false
  );
});

const updateWidget = (newWidget: WidgetEditModel) => {
  if (!usedModel.value) {
    return;
  }
  usedModel.value.widgets = usedModel.value.widgets.map((x) => {
    if (x.id === newWidget.id) {
      return newWidget;
    }
    return x;
  });
};

const usedWidgets = computed(() => {
  if (usedModel.value) {
    return usedModel.value.widgets;
  }
  return [];
});

const selectRevision = (revision: PageRevisionEditModel | null) => {
  selectedRevision.value = revision;
};

const widgetAdd = (type: string) => {
  let newId = pageModel.value!.widgets.length;
  while (pageModel.value!.widgets.some((x) => x.id === newId)) {
    newId++;
  }

  pageModel.value!.widgets.push({
    id: newId,
    type: type,
    albums: [],
    text: "",
  });
};

const removeWidget = (widget: WidgetEditModel) => {
  pageModel.value!.widgets = pageModel.value!.widgets.filter(
    (x) => x.id != widget.id
  );
};

const selectImage = (destination: JQuery<HTMLElement> | null) => {
  saveImageDest.value = destination;
};

const selectfile = (destination: JQuery<HTMLElement> | null) => {
  saveDocumentDest.value = destination;
};

const save = () => {
  if (selectedRevision.value) {
    if (
      !window.confirm(
        "Är du säker på att du vill ersätta sidan med denna version?"
      )
    ) {
      return;
    }
  }
  const success = $(".alert-success");
  const error = $(".alert-danger");
  ApiService.postByObject(
    window.location.href,
    usedModel.value,
    error,
    success,
    (res: any) => {
      selectedRevision.value = null;
      pageModel.value = res.newModel;
    }
  );
};

watch(
  () => drag.value,
  (value) => {
    EventBus.trigger("widgetDrag", value);
  }
);

watch(
  () => selectedRevision.value,
  (value) => {
    if (value) {
      usedModel.value = value;
    } else {
      usedModel.value = pageModel.value;
    }
  }
);
</script>
<style lang="scss" scoped>
.widget-area {
  list-style-type: none;
  padding: 0;
}
</style>
