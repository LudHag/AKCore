import { defaultFormSend } from "./services/apiservice";

const menu = document.getElementById("mobile-menu")!;

const openMobileMenu = document.getElementById("open-mobile-menu")!;
const subMenusButtons = document.getElementsByClassName(
  "exp-submenu"
) as HTMLCollectionOf<HTMLElement>;

openMobileMenu.addEventListener("click", (e) => {
  e.preventDefault();

  if (menu.style.height === "0px" || menu.style.height === "") {
    menu.style.height = menu.scrollHeight + "px";
  } else {
    menu.style.height = menu.scrollHeight + "px";

    setTimeout(() => (menu.style.height = "0px"), 0);
  }
});

Array.from(subMenusButtons).forEach((subMenuButton) => {
  subMenuButton.addEventListener("click", (e) => {
    const targetElement = subMenuButton;

    if (targetElement.classList.contains("exp-submenu")) {
      e.preventDefault();
      if (targetElement.classList.contains("glyphicon-plus")) {
        targetElement.classList.add("glyphicon-minus");
        targetElement.classList.remove("glyphicon-plus");
      } else {
        targetElement.classList.add("glyphicon-plus");
        targetElement.classList.remove("glyphicon-minus");
      }

      const subMenu = document.getElementById(targetElement.dataset.id!)!;

      if (subMenu.style.height === "0px" || subMenu.style.height === "") {
        menu.style.height = "auto";
        subMenu.style.height = subMenu.scrollHeight + "px";
      } else {
        subMenu.style.height = "0px";
      }
    }
  });
});

const recruitForm = document.getElementById("recruit-form");

if (recruitForm) {
  recruitForm.addEventListener("submit", (e) => {
    e.preventDefault();
    const form = recruitForm as HTMLFormElement;
    const success = form.querySelector(".alert-success")! as HTMLElement;
    const error = form.querySelector(".alert-danger")! as HTMLElement;

    defaultFormSend(form, error, success, () => {
      form.reset();
    });
  });
}

const allowedKeys = { 70: "f", 76: "l", 192: "ö", 74: "j", 84: "t" },
  code = ["f", "l", "ö", "j", "t"];
let pos = 0;
document.addEventListener("keydown", function (a) {
  //@ts-ignore
  const b = allowedKeys[a.keyCode],
    c = code[pos];
  b == c ? (pos++, pos == code.length && flojt()) : (pos = 0);
});

function flojt() {
  const a = document;
  let b = a.getElementById("__cornify_nodes");
  //@ts-ignore
  if (b) cornify_add();
  else {
    let c = null;
    (c = a.createElement("div")),
      (c.id = "__cornify_nodes"),
      a.getElementsByTagName("body")[0].appendChild(c);
    const d = [
      "https://cornify.com/js/cornify.js",
      "https://cornify.com/js/cornify_run.js",
    ];
    for (let e = 0; e < d.length; e++)
      //@ts-ignore
      (b = a.createElement("script")), (b.src = d[e]), c.appendChild(b);
  }
}
