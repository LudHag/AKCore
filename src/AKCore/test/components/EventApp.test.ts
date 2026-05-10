import "@styles/akstyle.scss";
import { render } from "vitest-browser-vue";
import { afterEach, expect, test, vi } from "vitest";

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

import EventApp from "@components/Event/EventApp.vue";
import {
  createEventDataPayload,
  mockEventDetailId,
  mockEventDetailName,
} from "@test/mocks/eventData";
import {
  fetchRequestUrl,
  jsonResponse,
} from "@test/utils/component-test-utils";

afterEach(() => {
  vi.restoreAllMocks();
});

test("EventApp mounts and shows the loading spinner", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation(
    () => new Promise<Response>(() => {}),
  );

  const screen = await render(EventApp, {
    props: { eventId: mockEventDetailId },
  });

  await expect
    .poll(() => screen.container.querySelector(".spinner-medium"))
    .not.toBeNull();
});

test("after EventData loads, event title and signup form are shown", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("Event/EventData")) {
      return jsonResponse(createEventDataPayload());
    }
    return Promise.reject(new Error(`Unexpected fetch: ${url}`));
  });

  const screen = await render(EventApp, {
    props: { eventId: mockEventDetailId },
  });

  await expect.poll(() => screen.container.querySelector("h1")).not.toBeNull();

  await expect
    .poll(() => screen.container.textContent?.includes(mockEventDetailName))
    .toBe(true);

  await expect
    .poll(() => screen.container.querySelector("#event-app form"))
    .not.toBeNull();
});

test("signup flow: submitting the form POSTs to /upcoming/Signup/{id} with field data", async () => {
  const fetchSpy = vi
    .spyOn(globalThis, "fetch")
    .mockImplementation((input, init) => {
      const url = fetchRequestUrl(input);
      if (url.includes("Event/EventData")) {
        return jsonResponse(createEventDataPayload());
      }
      if (url.includes(`/upcoming/Signup/${mockEventDetailId}`)) {
        expect(init?.method?.toLowerCase()).toBe("post");
        const body = init?.body as FormData;
        expect(body.get("Where")).toBe("Hålan");
        expect(body.get("SelectedInstrument")).toBe("Flute");
        expect(body.get("Comment")).toBe("See you there");
        return jsonResponse({ success: true, message: "ok" });
      }
      return Promise.reject(new Error(`Unexpected fetch: ${url}`));
    });

  const screen = await render(EventApp, {
    props: { eventId: mockEventDetailId },
  });

  await expect
    .poll(() => screen.container.querySelector("#event-app form"))
    .not.toBeNull();

  const form =
    screen.container.querySelector<HTMLFormElement>("#event-app form")!;
  const halan = form.querySelector<HTMLInputElement>(
    'input[name="Where"][value="Hålan"]',
  )!;
  halan.click();

  const comment = form.querySelector<HTMLInputElement>(
    'input[name="Comment"]',
  )!;
  comment.value = "See you there";
  comment.dispatchEvent(new Event("input", { bubbles: true }));

  form.querySelector<HTMLButtonElement>('button[type="submit"]')!.click();

  await expect
    .poll(
      () =>
        fetchSpy.mock.calls.filter(([input, init]) => {
          const url = fetchRequestUrl(input);
          return (
            url.includes(`/upcoming/Signup/${mockEventDetailId}`) &&
            (init as RequestInit | undefined)?.method?.toLowerCase() === "post"
          );
        }).length,
    )
    .toBe(1);

  await expect
    .poll(
      () =>
        fetchSpy.mock.calls.filter(([input]) =>
          fetchRequestUrl(input).includes("Event/EventData"),
        ).length,
    )
    .toBe(2);
});
