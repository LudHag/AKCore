import { render } from "vitest-browser-vue";
import { afterEach, beforeEach, expect, test, vi } from "vitest";

vi.mock("@scripts/general", () => ({
  getCookie: () => undefined,
  setCookie: () => {},
  getImageLink: () => "",
}));

const { defaultFormSendMock } = vi.hoisted(() => ({
  defaultFormSendMock:
    vi.fn<
      (
        form: HTMLFormElement,
        error: HTMLElement | null,
        success: HTMLElement | null,
        callback: (data: unknown) => void,
      ) => void
    >(),
}));
vi.mock("@services/apiservice", () => ({
  defaultFormSend: defaultFormSendMock,
}));

import LoginApp from "@components/Login/LoginApp.vue";

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
  defaultFormSendMock.mockReset();
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
  const screen = await render(LoginApp);

  await openModalAndFillCredentials(screen.container, "alice", "secret");

  await expect.poll(() => defaultFormSendMock.mock.calls.length).toBe(1);

  const [submittedForm, errorEl, successEl, callback] =
    defaultFormSendMock.mock.calls[0];
  expect(submittedForm.getAttribute("action")).toBe("/Account/Login");
  expect(submittedForm.getAttribute("method")).toBe("post");
  expect(errorEl).not.toBeNull();
  expect(successEl).toBeNull();
  expect(typeof callback).toBe("function");

  const submittedData = new FormData(submittedForm);
  expect(submittedData.get("Username")).toBe("alice");
  expect(submittedData.get("Password")).toBe("secret");
});

test("non-happy case: failed login shows the error message in the modal", async () => {
  defaultFormSendMock.mockImplementation((_form, error) => {
    if (error) {
      error.innerHTML = "Invalid credentials";
    }
  });

  const screen = await render(LoginApp);

  await openModalAndFillCredentials(screen.container, "alice", "wrong");

  await expect
    .poll(() => screen.container.textContent?.includes("Invalid credentials"))
    .toBe(true);
});
