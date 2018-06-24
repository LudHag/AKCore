$(function () {
    $(".account")
        .on("click",
            ".login",
            function(e) {
                e.preventDefault();
                $("#loginModal").modal("show"); 
            });

    $('#loginModal').on('shown.bs.modal', function () {
        $('#username').focus();
    })
    
    $("#mobile-menu")
     .on("click",
         ".login",
         function (e) {
             e.preventDefault();
             $("#loginModal").modal("show");
         });
    $("#loginForm")
        .on("submit",
            function(e) {
                e.preventDefault();
                var form = $(this);
                var success = form.find(".alert-success");
                var error = form.find(".alert-danger");
                $.ajax({
                    url: form.attr("action"),
                    type: "POST",
                    data: form.serialize(),
                    success: function(res) {
                        if (res.success) {
                            window.location.reload();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function(err) {
                        error.text("Misslyckades att logga in");
                        error.slideDown().delay(4000).slideUp();
                    }
                });
            });

    $("#open-mobile-menu")
        .on("click",
        function (e) {
            e.preventDefault();
                $("#mobile-menu").slideToggle();
            });

    $("#mobile-menu")
        .on("click",
            "a",
            function(e) {
                var target = $(e.target);
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

    if ($(".youtubelist").length>0)
        $(".youtubelist").youtubegallery();

    $('#recruit-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var success = form.find(".alert-success");
        var error = form.find(".alert-danger");
        
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (res) {
                if (res.success) {
                    form.trigger("reset");
                    success.text(res.message);
                    success.slideDown().delay(3000).slideUp();
                } else {
                    error.text(res.message);
                    error.slideDown().delay(5000).slideUp();
                }
            },
            error: function (err) {
                error.text("Ett fel uppstod när ansökan skickades");
                error.slideDown().delay(5000).slideUp();
            }
        });
    });

    $('#hire-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var success = form.find(".alert-success");
        var error = form.find(".alert-danger");

        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (res) {
                if (res.success) {
                    form.trigger("reset");
                    success.text(res.message);
                    success.slideDown().delay(3000).slideUp();
                } else {
                    error.text(res.message);
                    error.slideDown().delay(5000).slideUp();
                }
            },
            error: function (err) {
                error.text("Ett fel uppstod när ansökan skickades");
                error.slideDown().delay(5000).slideUp();
            }
        });
    });

    var allowedKeys = { 70: "f", 76: "l", 192: "ö", 74: "j", 84: "t" }, code = ["f", "l", "ö", "j", "t"], pos = 0; document.addEventListener("keydown", function (a) { var b = allowedKeys[a.keyCode], c = code[pos]; b == c ? (pos++ , pos == code.length && flojt()) : pos = 0 });
});
function flojt() { var a = document, b = a.getElementById("__cornify_nodes"), c = null, d = ["https://cornify.com/js/cornify.js", "https://cornify.com/js/cornify_run.js"]; if (b) cornify_add(); else { c = a.createElement("div"), c.id = "__cornify_nodes", a.getElementsByTagName("body")[0].appendChild(c); for (var e = 0; e < d.length; e++) b = a.createElement("script"), b.src = d[e], c.appendChild(b) } }