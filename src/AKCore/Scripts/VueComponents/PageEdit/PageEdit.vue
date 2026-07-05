<template>
  <div id="pageedit-app" v-if="pageModel && usedModel">
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
          :model-value="pageModel"
          :selected-revision="selectedRevision"
          @select="selectRevision"
        ></page-versions>
      </form>
    </div>
    <add-widget @add="widgetAdd"></add-widget>
    <ul class="widget-area">
      <VueDraggable
        :model-value="usedWidgets"
        @update:modelValue="sortWidgets($event)"
        @start="drag = true"
        @end="drag = false"
        handle=".widget-header"
        item-key="id"
      >
        <widget
          v-for="element in usedWidgets"
          :key="element.id"
          :model-value="element"
          :albums="pageModel.albums"
          @remove="removeWidget(element)"
          @update:modelValue="updateWidget($event)"
        >
        </widget>
      </VueDraggable>
    </ul>
    <image-picker-modal
      v-if="imageInsert"
      :show-modal="!!imageInsert"
      :insert-callback="imageInsert"
      :notransition="true"
      @close="imageInsert = null"
    ></image-picker-modal>
    <document-picker-modal
      v-if="fileInsert"
      :show-modal="!!fileInsert"
      :insert-callback="fileInsert"
      :notransition="true"
      @close="fileInsert = null"
    >
    </document-picker-modal>
  </div>
</template>
<script setup lang="ts">
import PageMeta from "./PageMeta.vue";
import AddWidget from "./AddWidget.vue";
import Widget from "./Widget.vue";
import { getFromApi, postByObject } from "@services/apiservice";
import ImagePickerModal from "../ImagePickerModal.vue";
import DocumentPickerModal from "../DocumentPickerModal.vue";
import { VueDraggable } from "vue-draggable-plus";
import PageVersions from "./PageVersions.vue";
import { EventBus } from "@utils/eventbus";
import { onMounted, ref, watch, computed } from "vue";
import {
  PageEditModel,
  PageRevisionEditModel,
  WidgetEditModel,
} from "./models";

const pageModel = ref<PageEditModel | null>(null);
const imageInsert = ref<((url: string) => void) | null>(null);
const fileInsert = ref<((url: string) => void) | null>(null);
const drag = ref(false);
const selectedRevision = ref<PageRevisionEditModel | null>(null);
const usedModel = ref<PageEditModel | PageRevisionEditModel | null>(null);

onMounted(() => {
  EventBus.on("loadimage", (callback: (url: string) => void) => {
    selectImage(callback);
  });

  EventBus.on("loadfile", (callback: (url: string) => void) => {
    selectfile(callback);
  });

  getFromApi<PageEditModel>(window.location.href + "/Model").then((res) => {
    pageModel.value = res;
    usedModel.value = pageModel.value;
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
    false,
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

const sortWidgets = (updatedWidgets: WidgetEditModel[]) => {
  if (!usedModel.value) {
    return;
  }
  usedModel.value.widgets = updatedWidgets;
};

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
  usedModel.value!.widgets = usedModel.value!.widgets.filter(
    (x) => x.id != widget.id,
  );
};

const selectImage = (callback: (url: string) => void) => {
  imageInsert.value = callback;
};

const selectfile = (callback: (url: string) => void) => {
  fileInsert.value = callback;
};

const save = () => {
  if (selectedRevision.value) {
    if (
      !window.confirm(
        "Är du säker på att du vill ersätta sidan med denna version?",
      )
    ) {
      return;
    }
  }
  const success = document.getElementsByClassName(
    "alert-success",
  )[0] as HTMLElement;
  const error = document.getElementsByClassName(
    "alert-danger",
  )[0] as HTMLElement;

  postByObject(
    window.location.href,
    usedModel.value,
    error,
    success,
    (res: any) => {
      selectedRevision.value = null;
      pageModel.value = res.newModel;
    },
  );
};

watch(
  () => drag.value,
  (value) => {
    EventBus.trigger("widgetDrag", value);
  },
);

watch(
  () => selectedRevision.value,
  (value) => {
    if (value) {
      usedModel.value = value;
    } else {
      usedModel.value = pageModel.value;
    }
  },
);
</script>
<style lang="scss" scoped>
.widget-area {
  list-style-type: none;
  padding: 0;
}
</style>
