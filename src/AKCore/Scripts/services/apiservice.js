export default {
  defaultFormSend(form, error, success, callback) {
    $.ajax({
      url: form.attr("action"),
      type: form.attr("method"),
      data: form.serialize(),
      success: function(res) {
        if (res.success) {
          if (success) {
            success.text(res.message);
            success
              .slideDown()
              .delay(4000)
              .slideUp();
          }
          if (callback) {
            callback();
          }
        } else {
          if (error) {
            error.text(res.message);
            error
              .slideDown()
              .delay(4000)
              .slideUp();
          }
        }
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  },
  postByUrl(url, error, success, callback) {
    $.ajax({
      url,
      type: "POST",
      success: function(res) {
        if (res.success) {
          if (success) {
            success.text(res.message);
            success
              .slideDown()
              .delay(4000)
              .slideUp();
          }
          if (callback) {
            callback();
          }
        } else {
          if (error) {
            error.text(res.message);
            error
              .slideDown()
              .delay(4000)
              .slideUp();
          }
        }
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  },
  postByObjectAsForm(url, obj, error, success, callback) {
    $.ajax({
      url: url,
      dataType: "json",
      data: obj,
      type: "POST",
      success: function(res) {
        if (res.success) {
          if (success) {
            success.text(res.message);
            success
              .slideDown()
              .delay(4000)
              .slideUp();
          }
          if (callback) {
            callback();
          }
        } else {
          if (error) {
            error.text(res.message);
            error
              .slideDown()
              .delay(4000)
              .slideUp();
          }
        }
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  },
  postByObject(url, obj, error, success, callback) {
    $.ajax({
      url: url,
      dataType: "json",
      data: JSON.stringify(obj),
      contentType: "application/json; charset=utf-8",
      type: "POST",
      success: function(res) {
        if (res.success) {
          if (success) {
            success.text(res.message);
            success
              .slideDown()
              .delay(4000)
              .slideUp();
          }
          if (callback) {
            callback(res);
          }
        } else {
          if (error) {
            error.text(res.message);
            error
              .slideDown()
              .delay(4000)
              .slideUp();
          }
        }
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  },
  postFormData(url, obj, error, success, callback) {
    $.ajax({
      url: url,
      dataType: "json",
      data: obj,
      contentType: false,
      processData: false,
      type: "POST",
      success: function(res) {
        if (res.success) {
          if (success) {
            success.text(res.message);
            success
              .slideDown()
              .delay(4000)
              .slideUp();
          }
          if (callback) {
            callback();
          }
        } else {
          if (error) {
            error.text(res.message);
            error
              .slideDown()
              .delay(4000)
              .slideUp();
          }
        }
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  },
  get(url, error, callback) {
    $.ajax({
      url: url,
      type: "GET",
      success: function(res) {
        callback(res);
      },
      error: function() {
        if (error) {
          error.text("Server error");
          error
            .slideDown()
            .delay(4000)
            .slideUp();
        }
      }
    });
  }
};
