import { defaultFormSend } from "./services/apiservice";

$(function () {
  $("#open-mobile-menu").on("click", function (e) {
    e.preventDefault();
    $("#mobile-menu").slideToggle();
  });

  $("#mobile-menu").on("click", "a", function (e) {
    const target = $(e.target);
    if (target.hasClass("exp-submenu")) {
      e.preventDefault();
      if (target.hasClass("glyphicon-plus")) {
        target.addClass("glyphicon-minus");
        target.removeClass("glyphicon-plus");
      } else {
        target.addClass("glyphicon-plus");
        target.removeClass("glyphicon-minus");
      }

      $(target.data("id")).slideToggle();
    }
  });

  $("#recruit-form").on("submit", function (e) {
    e.preventDefault();
    const form = $(this);
    const success = form.find(".alert-success");
    const error = form.find(".alert-danger");

    defaultFormSend(this as HTMLFormElement, error, success, () => {
      form.trigger("reset");
    });
  });

  const allowedKeys = { 70: "f", 76: "l", 192: "ö", 74: "j", 84: "t" },
    code = ["f", "l", "ö", "j", "t"];
  let pos = 0;
  document.addEventListener("keydown", function (a) {
    //@ts-ignore
    const b = allowedKeys[a.keyCode],
      c = code[pos];
    b == c ? (pos++, pos == code.length && flojt()) : (pos = 0);
  });
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
