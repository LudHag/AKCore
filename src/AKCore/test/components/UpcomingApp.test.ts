import { render } from "vitest-browser-vue";
import { afterEach, expect, test, vi } from "vitest";

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

import UpcomingApp from "@components/Upcoming/UpcomingApp.vue";

const EVENT_NAME = "Component Test Gig";

/** Minimal payload matching `/Upcoming/UpcomingListData` JSON shape used by UpcomingApp. */
function upcomingListData(loggedIn: boolean) {
  return {
    years: {
      "2026": {
        year: 2026,
        months: {
          "5": [
            {
              id: 4242,
              type: "Spelning",
              name: EVENT_NAME,
              place: "Test Venue",
              description: "",
              descriptionEng: "",
              internalDescription: "",
              internalDescriptionEng: "",
              fikaCollection: [] as string[],
              year: 2026,
              month: 5,
              day: "monday 05/05",
              dayDate: "2026-05-05",
              dayInMonth: 5,
              halanTime: "",
              thereTime: "",
              startsTime: "19:00",
              playDuration: "",
              stand: "",
              secret: false,
              signupState: null,
              coming: 0,
              notComing: 0,
              disabled: false,
            },
          ],
        },
      },
    },
    loggedIn,
    member: false,
    icalLink: "https://example.test/upcoming/akevents.ics",
  };
}

function jsonResponse(data: unknown) {
  return Promise.resolve(
    new Response(JSON.stringify(data), {
      status: 200,
      headers: { "Content-Type": "application/json" },
    }),
  );
}

function fetchRequestUrl(input: string | Request | URL): string {
  if (typeof input === "string") return input;
  if (input instanceof URL) return input.href;
  return input.url;
}

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

  // Playwright visibility requires a non-zero layout; scoped spinner styles may not
  // apply the same way in the test page, so assert DOM presence instead.
  await expect
    .poll(() => screen.container.querySelector(".spinner-medium"))
    .not.toBeNull();
});

test("after UpcomingListData loads while logged in, list and member-only chrome appear", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("UpcomingListData")) {
      return jsonResponse(upcomingListData(true));
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
    .poll(() => screen.container.textContent?.includes(EVENT_NAME))
    .toBe(true);

  await expect
    .poll(() => screen.container.querySelector(".calendar-actions"))
    .not.toBeNull();
});

test("after UpcomingListData loads while logged out, list appears without member chrome", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("UpcomingListData")) {
      return jsonResponse(upcomingListData(false));
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
    .poll(() => screen.container.textContent?.includes(EVENT_NAME))
    .toBe(true);

  expect(screen.container.querySelector(".calendar-actions")).toBeNull();
});
