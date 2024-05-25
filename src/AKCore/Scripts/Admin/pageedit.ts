import { defaultFormSend } from "../services/apiservice";

const newPageButton = document.getElementById("new-page-button");
const newPageForm = document.getElementById("new-page-form");
const removePageButtons = document.getElementsByClassName(
  "remove-page",
) as HTMLCollectionOf<HTMLElement>;

if (newPageButton) {
  newPageButton.addEventListener("click", (e) => {
    e.preventDefault();
    newPageForm!.classList.toggle("open");
  });
}

Array.from(removePageButtons).forEach((removePageButton) => {
  removePageButton.addEventListener("click", (e) => {
    e.preventDefault();
    if (window.confirm("Är du säker på att du vill ta bort denna sida?")) {
      window.location.href = removePageButton.getAttribute("href") as string;
    }
  });
});

if (newPageForm) {
  const form = newPageForm.getElementsByTagName("form")[0]!;

  form.addEventListener("submit", (e) => {
    e.preventDefault();
    const error = form.querySelector(".alert-danger")! as HTMLElement;

    defaultFormSend(form, error, null, () => {
      window.location.reload();
    });
  });
}
