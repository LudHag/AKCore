import { describe, expect, it } from "vitest";
import { FeatureUsageTypes } from "@services/usage";
import { getFeatureUsageLabel } from "@components/Statistics/utils";

describe("getFeatureUsageLabel", () => {
  it("has a readable label for every feature usage type", () => {
    for (const type of Object.values(FeatureUsageTypes)) {
      expect(getFeatureUsageLabel(type)).not.toBe(type);
    }
  });

  it("falls back to the raw type for unknown types", () => {
    expect(getFeatureUsageLabel("SomethingElse")).toBe("SomethingElse");
  });
});
