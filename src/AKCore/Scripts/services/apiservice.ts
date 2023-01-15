﻿const handleResponse = async (
  response: Response,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
  callback: (data: any) => void
) => {
  const res = await response.json();

  if (res.success) {
    if (success) {
      success.text(res.message);
      success.slideDown().delay(4000).slideUp();
    }
    if (callback) {
      callback(res);
    }
  } else {
    if (error) {
      error.text(res.message);
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const defaultFormSend = async (
  formElement: HTMLFormElement,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
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
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const postToApi = async (
  url: string,
  obj: any | null,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
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
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const postByObject = async (
  url: string,
  obj: any,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
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
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const postFormData = async (
  url: string,
  obj: FormData,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
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
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const getFromApi = async <T>(
  url: string,
  error: JQuery<HTMLElement> | null
): Promise<T> => {
  try {
    const response = await fetch(url);

    return (await response.json()) as T;
  } catch (e) {
    if (error) {
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
    throw e;
  }
};
