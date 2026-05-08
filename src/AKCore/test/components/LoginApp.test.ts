import "@styles/akstyle.scss";
import { render } from "vitest-browser-vue";
import { afterEach, beforeEach, expect, test, vi } from "vitest";

const { reloadWindowMock } = vi.hoisted(() => ({
  reloadWindowMock: vi.fn(),
}));

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

vi.mock("@services/window-utils", () => ({
  reloadWindow: reloadWindowMock,
}));

import LoginApp from "@components/Login/LoginApp.vue";
import {
  fetchRequestUrl,
  jsonResponse,
} from "@test/utils/component-test-utils";

const LOGIN_TRIGGER_ID = "login-trigger";

let loginTrigger: HTMLButtonElement;

beforeEach(() => {
  loginTrigger = document.createElement("button");
  loginTrigger.id = LOGIN_TRIGGER_ID;
  loginTrigger.className = "login";
  loginTrigger.textContent = "Open login";
  document.body.appendChild(loginTrigger);
});

afterEach(() => {
  loginTrigger.remove();
  reloadWindowMock.mockReset();
  vi.restoreAllMocks();
});

async function openModalAndFillCredentials(
  container: HTMLElement,
  username: string,
  password: string,
) {
  loginTrigger.click();

  await expect.poll(() => container.querySelector("#loginForm")).not.toBeNull();

  const usernameInput = container.querySelector<HTMLInputElement>("#username")!;
  const passwordInput = container.querySelector<HTMLInputElement>("#password")!;
  usernameInput.value = username;
  passwordInput.value = password;

  const submitButton = container.querySelector<HTMLButtonElement>(
    "button.submit-login",
  )!;
  submitButton.click();
}

test("happy case: submitting valid credentials posts the form to /Account/Login", async () => {
  const fetchSpy = vi.spyOn(globalThis, "fetch").mockImplementation((input, init) => {
    const url = fetchRequestUrl(input);
    expect(url).toBe("/Account/Login");
    expect(init?.method?.toLowerCase()).toBe("post");
    const body = init?.body as FormData;
    expect(body.get("Username")).toBe("alice");
    expect(body.get("Password")).toBe("secret");
    return jsonResponse({ success: true, message: "ok" });
  });

  const screen = await render(LoginApp);

  await openModalAndFillCredentials(screen.container, "alice", "secret");

  await expect.poll(() => fetchSpy.mock.calls.length).toBe(1);
  await expect.poll(() => reloadWindowMock.mock.calls.length).toBe(1);
});

test("non-happy case: failed login shows the error message in the modal", async () => {
  vi.spyOn(globalThis, "fetch").mockImplementation((input) => {
    const url = fetchRequestUrl(input);
    if (url.includes("/Account/Login")) {
      return jsonResponse({
        success: false,
        message: "Invalid credentials",
      });
    }
    return Promise.reject(new Error(`Unexpected fetch: ${url}`));
  });

  const screen = await render(LoginApp);

  await openModalAndFillCredentials(screen.container, "alice", "wrong");

  await expect
    .poll(() => screen.container.textContent?.includes("Invalid credentials"))
    .toBe(true);

  expect(reloadWindowMock).not.toHaveBeenCalled();
});

test("closing login via header close button hides the modal", async () => {
  const screen = await render(LoginApp);

  loginTrigger.click();
  await expect.poll(() => screen.container.querySelector("#loginForm")).not.toBeNull();

  const headerClose = screen.container.querySelector<HTMLButtonElement>(
    ".modal-header button.close",
  );
  expect(headerClose).not.toBeNull();
  headerClose!.click();

  await expect.poll(() => screen.container.querySelector("#loginForm")).toBeNull();
});
