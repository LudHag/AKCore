export const FeatureUsageTypes = {
  AlbumAIGeneration: "AlbumAIGeneration",
  EventTranslation: "EventTranslation",
} as const;

export type FeatureUsageType =
  (typeof FeatureUsageTypes)[keyof typeof FeatureUsageTypes];

export const trackFeatureUsage = (type: FeatureUsageType): void => {
  void fetch(`/Usage/Track?type=${encodeURIComponent(type)}`, {
    method: "POST",
  }).catch(() => {});
};
