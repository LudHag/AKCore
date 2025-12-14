import { reactive } from "vue";

// Shared reactive state for video search
export const sharedVideoState = reactive({
  searchPhrase: "",
  updateSearchPhrase: (newPhrase: string) => {
    sharedVideoState.searchPhrase = newPhrase;
  },
});

// Global window object for cross-app communication
declare global {
  interface Window {
    sharedVideoState: typeof sharedVideoState;
  }
}

// Make it available globally
if (typeof window !== "undefined") {
  window.sharedVideoState = sharedVideoState;
}
