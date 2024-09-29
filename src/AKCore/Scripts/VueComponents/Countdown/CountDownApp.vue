<template>
  <div class="countdown-container" v-if="timeLeft">
    <div class="countdown">
      <div class="countdown-item">
        <span class="countdown-number">{{ days }}</span>
        <span class="countdown-label">{{ t("days") }}</span>
      </div>
      <div class="countdown-item">
        <span class="countdown-number">{{ hours }}</span>
        <span class="countdown-label">{{ t("hours") }}</span>
      </div>
      <div class="countdown-item">
        <span class="countdown-number">{{ minutes }}</span>
        <span class="countdown-label">{{ t("minutes") }}</span>
      </div>
      <div class="countdown-item">
        <span class="countdown-number">{{ seconds }}</span>
        <span class="countdown-label">{{ t("seconds") }}</span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onBeforeUnmount } from "vue";
import { translate, TranslationDomain } from "../../translations";

const props = defineProps<{
  selectedDate: string;
}>();

const timeLeft = ref<number | null>(null);
let timerInterval: number | null = null;

const calculateTimeLeft = (endDate: number) => {
  const now = new Date().getTime();
  const difference = endDate - now;
  return difference > 0 ? difference : null;
};

const updateTime = () => {
  const endDate = new Date(props.selectedDate).getTime();
  timeLeft.value = calculateTimeLeft(endDate);
  if (timeLeft.value === null && timerInterval !== null) {
    clearInterval(timerInterval);
  }
};

const days = computed(() =>
  Math.floor((timeLeft.value || 0) / (1000 * 60 * 60 * 24)),
);
const hours = computed(() =>
  Math.floor(
    ((timeLeft.value || 0) % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60),
  ),
);
const minutes = computed(() =>
  Math.floor(((timeLeft.value || 0) % (1000 * 60 * 60)) / (1000 * 60)),
);
const seconds = computed(() =>
  Math.floor(((timeLeft.value || 0) % (1000 * 60)) / 1000),
);

onMounted(() => {
  updateTime(); // Initial update
  timerInterval = window.setInterval(updateTime, 1000);
});

onBeforeUnmount(() => {
  if (timerInterval !== null) {
    clearInterval(timerInterval);
  }
});

const t = (key: string, domain: TranslationDomain = "countdown") => {
  return translate(domain, key);
};
</script>

<style lang="scss" scoped>
@import "../../../Styles/variables.scss";
@import "bootstrap-sass/assets/stylesheets/bootstrap/_variables.scss";

.countdown-container {
  display: flex;
  flex-direction: column;
  align-items: center;
  justify-content: center;
  padding: 16px;
  text-align: center;
  font-family: sans-serif;
}

.countdown-title,
.countdown-over-title {
  font-size: 32px;
  color: #333;
  margin-bottom: 32px;
  font-weight: 600;
}

.countdown {
  display: flex;
  gap: 16px;
}

.countdown-item {
  background-color: #e6e6e6;
  padding: 12px 24px;
  display: flex;
  flex-direction: column;
  border-radius: 12px;
  box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);
  text-align: center;
  transition: transform 0.3s;
}

.countdown-item:hover {
  transform: scale(1.05);
}

.countdown-number {
  font-size: 3rem;
  color: $akred;
  font-weight: 600;
}

.countdown-label {
  font-size: 1rem;
  color: #666;
  text-transform: uppercase;
  letter-spacing: 1px;
}

.countdown-over-title {
  font-size: 3rem;
  color: $akred;
  font-weight: 600;
}

@media screen and (max-width: $screen-xs-max) {
  .countdown {
    flex-direction: row;
    gap: 8px;
  }

  .countdown-item {
    padding: 10px;
  }

  .countdown-number {
    font-size: 2.5rem;
  }

  .countdown-title {
    font-size: 2rem;
  }

  .countdown-over-title {
    font-size: 2.5rem;
  }
}
</style>
