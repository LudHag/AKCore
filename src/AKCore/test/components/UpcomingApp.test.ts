import "@styles/akstyle.scss";
import { render } from "vitest-browser-vue";
import { afterEach, expect, test, vi } from "vitest";

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

import UpcomingApp from "@components/Upcoming/UpcomingApp.vue";
import {
  createUpcomingListDataPayload,
  mockUpcomingEventName,
} from "@test/mocks/upcomingListData";
import {
  fetchRequestUrl,
  jsonResponse,
} from "@test/utils/component-test-utils";

afterEach(() => {
  vi.restoreAllMocks();
});

test("UpcomingApp mounts and shows the loading spinner", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation(
    () => new Promise<Response>(() => {}),
  );

  const screen = await render(UpcomingApp, {
    props: { eventId: -1 },
  });

  await expect
    .poll(() => screen.container.querySelector(".spinner-medium"))
    .not.toBeNull();
});

test("after UpcomingListData loads while logged in, list and member-only", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("UpcomingListData")) {
      return jsonResponse(createUpcomingListDataPayload(true));
    }
    return Promise.reject(new Error(`Unexpected fetch: ${url}`));
  });

  const screen = await render(UpcomingApp, {
    props: { eventId: -1 },
  });

  await expect
    .poll(() => screen.container.querySelector(".event-row"))
    .not.toBeNull();

  await expect
    .poll(() => screen.container.textContent?.includes(mockUpcomingEventName))
    .toBe(true);

  await expect
    .poll(() => screen.container.querySelector(".calendar-actions"))
    .not.toBeNull();
});

test("after UpcomingListData loads while logged out, list appears without member", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("UpcomingListData")) {
      return jsonResponse(createUpcomingListDataPayload(false));
    }
    return Promise.reject(new Error(`Unexpected fetch: ${url}`));
  });

  const screen = await render(UpcomingApp, {
    props: { eventId: -1 },
  });

  await expect
    .poll(() => screen.container.querySelector(".event-row"))
    .not.toBeNull();

  await expect
    .poll(() => screen.container.textContent?.includes(mockUpcomingEventName))
    .toBe(true);

  expect(screen.container.querySelector(".calendar-actions")).toBeNull();
});
