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
  } catch (e) {
    if (error) {
      error.text("Server error");
      error.slideDown().delay(4000).slideUp();
    }
  }
};

export const postByObject = (
  url: string,
  obj: any,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
  callback: (data: any) => void
) => {
  $.ajax({
    url: url,
    dataType: "json",
    data: JSON.stringify(obj),
    contentType: "application/json; charset=utf-8",
    type: "POST",
    success: function (res) {
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
    },
    error: function () {
      if (error) {
        error.text("Server error");
        error.slideDown().delay(4000).slideUp();
      }
    },
  });
};

export const postFormData = (
  url: string,
  obj: any,
  error: JQuery<HTMLElement> | null,
  success: JQuery<HTMLElement> | null,
  callback: (data: any) => void
) => {
  $.ajax({
    url: url,
    dataType: "json",
    data: obj,
    contentType: false,
    processData: false,
    type: "POST",
    success: function (res) {
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
    },
    error: function () {
      if (error) {
        error.text("Server error");
        error.slideDown().delay(4000).slideUp();
      }
    },
  });
};

export const getFromApi = (
  url: string,
  error: JQuery<HTMLElement> | null,
  callback: (data: any) => void
) => {
  $.ajax({
    url: url,
    type: "GET",
    success: function (res) {
      callback(res);
    },
    error: function () {
      if (error) {
        error.text("Server error");
        error.slideDown().delay(4000).slideUp();
      }
    },
  });
};
