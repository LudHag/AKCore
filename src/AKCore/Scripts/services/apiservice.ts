import { slideUpAndDown } from "./slidehandler";

const handleResponse = async (
  response: Response,
  error: HTMLElement | null,
  success: HTMLElement | null,
  callback: (data: any) => void
) => {
  const res = await response.json();

  if (res.success) {
    if (success) {
      slideUpAndDown(success, 4000, res.message);
    }
    if (callback) {
      callback(res);
    }
  } else {
    if (error) {
      slideUpAndDown(error, 4000, res.message);
    }
  }
};

export const defaultFormSend = async (
  formElement: HTMLFormElement,
  error: HTMLElement | null,
  success: HTMLElement | null,
  callback: (data: any) => void
) => {
  try {
    const response = await fetch(formElement.getAttribute("action") as string, {
      method: formElement.getAttribute("method") as string,
      body: new FormData(formElement),
    });

    await handleResponse(response, error, success, callback);
  } catch (e) {
    if (error) {
      slideUpAndDown(error, 4000, "Server error");
    }
  }
};

export const postToApi = async (
  url: string,
  obj: any | null,
  error: HTMLElement | null,
  success: HTMLElement | null,
  callback: (data: any) => void
) => {
  try {
    const data = obj !== null ? new URLSearchParams(obj) : null;

    const response = await fetch(url, {
      method: "POST",
      body: data,
    });

    await handleResponse(response, error, success, callback);
  } catch (e) {
    if (error) {
      slideUpAndDown(error, 4000, "Server error");
    }
  }
};

export const postByObject = async (
  url: string,
  obj: any,
  error: HTMLElement | null,
  success: HTMLElement | null,
  callback: (data: any) => void
) => {
  try {
    const response = await fetch(url, {
      method: "POST",
      body: JSON.stringify(obj),
      headers: {
        "Content-Type": "application/json; charset=utf-8",
      },
    });

    handleResponse(response, error, success, callback);
  } catch (e) {
    if (error) {
      slideUpAndDown(error, 4000, "Server error");
    }
  }
};

export const postFormData = async (
  url: string,
  obj: FormData,
  error: HTMLElement | null,
  success: HTMLElement | null,
  callback: (data: any) => void
) => {
  try {
    const response = await fetch(url, {
      method: "POST",
      body: obj,
    });

    handleResponse(response, error, success, callback);
  } catch (e) {
    if (error) {
      slideUpAndDown(error, 4000, "Server error");
    }
  }
};

export const getFromApi = async <T>(
  url: string,
  error?: HTMLElement
): Promise<T> => {
  try {
    const response = await fetch(url);

    return (await response.json()) as T;
  } catch (e) {
    if (error) {
      slideUpAndDown(error, 4000, "Server error");
    }
    throw e;
  }
};
