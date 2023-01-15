export default {
  defaultFormSend(
    form: JQuery<HTMLElement>,
    error: JQuery<HTMLElement> | null,
    success: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
    $.ajax({
      url: form.attr("action"),
      type: form.attr("method"),
      data: form.serialize(),
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
  },
  postByUrl(
    url: string,
    error: JQuery<HTMLElement> | null,
    success: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
    $.ajax({
      url,
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
  },
  postByObjectAsForm(
    url: string,
    obj: any,
    error: JQuery<HTMLElement> | null,
    success: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
    $.ajax({
      url: url,
      dataType: "json",
      data: obj,
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
  },
  postByObject(
    url: string,
    obj: any,
    error: JQuery<HTMLElement> | null,
    success: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
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
  },
  postFormData(
    url: string,
    obj: any,
    error: JQuery<HTMLElement> | null,
    success: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
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
  },
  get(
    url: string,
    error: JQuery<HTMLElement> | null,
    callback: (data: any) => void
  ) {
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
  },
};
