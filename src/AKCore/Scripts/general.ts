import { defaultFormSend } from "./services/apiservice";

const menu = document.getElementById("mobile-menu")!;

const openMobileMenu = document.getElementById("open-mobile-menu")!;
const subMenusButtons = document.getElementsByClassName(
  "exp-submenu",
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

const allowedKeys: { [key: number]: string } = {
    70: "f",
    76: "l",
    192: "ö",
    74: "j",
    84: "t",
  },
  code = ["f", "l", "ö", "j", "t"];
let pos = 0;

document.addEventListener("keydown", function (event: KeyboardEvent) {
  const key = allowedKeys[event.keyCode],
    expectedKey = code[pos];
  if (key === expectedKey) {
    pos++;
    if (pos === code.length) {
      flojt();
    }
  } else {
    pos = 0;
  }
});

function flojt() {
  const document: Document = window.document;
  let nodesContainer: HTMLDivElement | null = document.getElementById(
    "__cornify_nodes",
  ) as HTMLDivElement;

  if (nodesContainer) {
    (window as any).cornify_add();
  } else {
    nodesContainer = document.createElement("div");
    nodesContainer.id = "__cornify_nodes";
    document.body.appendChild(nodesContainer);

    const scriptUrls = [
      "https://cornify.com/js/cornify.js",
      "https://cornify.com/js/cornify_run.js",
    ];

    scriptUrls.forEach((url) => {
      const script = document.createElement("script") as HTMLScriptElement;
      script.src = url;
      nodesContainer?.appendChild(script);
    });
  }
}

const languageButton = document.getElementById("language-change");
const languageButtonMobile = document.getElementById("language-change-mob");

if (languageButton) {
  languageButton.addEventListener("click", (e) => {
    e.preventDefault();
    const lang = (e.target as HTMLElement).classList[0];
    document.cookie = `language=${lang};path=/`;
    window.location.reload();
  });
}
if (languageButtonMobile) {
  languageButtonMobile.addEventListener("click", (e) => {
    e.preventDefault();
    const lang = (e.target as HTMLElement).classList[0];
    document.cookie = `language=${lang};path=/`;
    window.location.reload();
  });
}
