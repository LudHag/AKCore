export const FeatureUsageTypes = {
  AlbumAIGeneration: "AlbumAIGeneration",
  EventTranslation: "EventTranslation",
  PageTranslation: "PageTranslation",
  FlojtEvent: "FlojtEvent",
} as const;

export type FeatureUsageType =
  (typeof FeatureUsageTypes)[keyof typeof FeatureUsageTypes];

export const recordFeatureUsage = (type: FeatureUsageType): void => {
  void fetch(`/Usage/Record?type=${encodeURIComponent(type)}`, {
    method: "POST",
  }).catch(() => {});
};
