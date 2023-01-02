$("#new-page-button").on("click", function (e) {
  e.preventDefault();
  $("#new-page-form").toggle();
});

$(".remove-page").on("click", function (e) {
  e.preventDefault();
  if (window.confirm("Är du säker på att du vill ta bort denna sida?")) {
    window.location.href = $(this).attr("href") as string;
  }
});

$("#new-page-form").on("submit", "form", function (e) {
  e.preventDefault();
  const form = $(this);
  const error = form.find(".alert-danger");
  $.ajax({
    url: form.attr("action"),
    type: "POST",
    data: form.serialize(),
    success: function (res) {
      if (res.success) {
        window.location.reload();
      } else {
        error.text(res.message);
        error.slideDown().delay(4000).slideUp();
      }
    },
    error: function (err) {
      error.text("Misslyckades med att skapa sida");
      error.slideDown().delay(4000).slideUp();
    },
  });
});
