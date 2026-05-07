import { render } from "vitest-browser-vue";
import { expect, test, vi } from "vitest";

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

import UpcomingApp from "@components/Upcoming/UpcomingApp.vue";

test("UpcomingApp mounts and shows the loading spinner", async () => {
  const screen = await render(UpcomingApp, {
    props: { eventId: -1 },
  });

  // Playwright visibility requires a non-zero layout; scoped spinner styles may not
  // apply the same way in the test page, so assert DOM presence instead.
  await expect
    .poll(() => screen.container.querySelector(".spinner-medium"))
    .not.toBeNull();
});
