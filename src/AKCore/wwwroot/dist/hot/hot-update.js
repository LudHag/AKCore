webpackHotUpdate(0,{

/***/ 24:
/***/ (function(module, exports, __webpack_require__) {

"use strict";


$(function () {
    $('#signup-form').on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error = form.find(".alert-danger");
        var _success = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    reloadSignups();
                    _success.text("Anmälan uppdaterad");
                    _success.slideDown().delay(3000).slideUp();
                } else {
                    _error.text(res.message);
                    _error.slideDown().delay(4000).slideUp();
                }
            },
            error: function error() {
                _error.text("Misslyckades med att anmäla dig");
                _error.slideDown().delay(4000).slideUp();
            }
        });
    });

    $('.expandable').on('click', function (e) {
        var self = $(this);
        var target = $(e.target);
        if (!target.is("a")) {
            self.toggleClass("expanded");
        }
    });

    function reloadSignups() {
        $('#signup-list').load(' #signup-list > *');
    }
    $('#ical-link-anchor').on('click', function (e) {
        e.preventDefault();
        $('.ical-copy').toggleClass('hide');
    });

    $("#admin-add-signups").on('click', function (e) {
        e.preventDefault();
        $("#edit-signup-modal").modal("show");
    });

    $("#edit-signup-form").on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var _error2 = form.find(".alert-danger");
        var _success2 = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function success(res) {
                if (res.success) {
                    _success2.text("Anmälan uppdaterad");
                    _success2.slideDown().delay(3000).slideUp();
                    form.trigger("reset");
                } else {
                    _error2.text(res.message);
                    _error2.slideDown().delay(4000).slideUp();
                }
            },
            error: function error() {
                _error2.text("Misslyckades med att anmäla dig");
                _error2.slideDown().delay(4000).slideUp();
            }
        });
    });

    $(".copy-btn").on('click', function (e) {
        e.preventDefault();
        var copyText = document.querySelector("#ical-link");
        copyText.select();
        document.execCommand("copy");
    });
});

/***/ })

})