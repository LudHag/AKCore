import { defaultFormSend } from "../services/apiservice";

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

  defaultFormSend(this as HTMLFormElement, error, null, () => {
    window.location.reload();
  });
});
