import { describe, expect, it, vi } from "vitest";
import {
  FeatureUsageTypes,
  trackFeatureUsage,
} from "@services/usage";
import {
  fetchRequestUrl,
  jsonResponse,
} from "@test/utils/component-test-utils";

describe("trackFeatureUsage", () => {
  it("POSTs to /Usage/Track with the given type", async () => {
    const fetchSpy = vi
      .spyOn(globalThis, "fetch")
      .mockImplementation(() => jsonResponse({ success: true }));

    try {
      trackFeatureUsage(FeatureUsageTypes.AlbumAIGeneration);

      await expect.poll(() => fetchSpy.mock.calls.length).toBe(1);

      const [input, init] = fetchSpy.mock.calls[0];
      expect(fetchRequestUrl(input)).toBe(
        `/Usage/Track?type=${FeatureUsageTypes.AlbumAIGeneration}`,
      );
      expect(init?.method).toBe("POST");
    } finally {
      fetchSpy.mockRestore();
    }
  });

  it("swallows fetch errors", async () => {
    const fetchSpy = vi
      .spyOn(globalThis, "fetch")
      .mockRejectedValue(new Error("network"));

    try {
      expect(() =>
        trackFeatureUsage(FeatureUsageTypes.EventTranslation),
      ).not.toThrow();

      await Promise.resolve();
    } finally {
      fetchSpy.mockRestore();
    }
  });
});
