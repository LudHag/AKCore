$(function() {
    $('#signup-form').on('submit',function(e) {
        e.preventDefault();
        var form = $(this);
        var error = form.find(".alert-danger");
        var success = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (res) {
                if (res.success) {
                    reloadSignups();
                    success.text("Anmälan uppdaterad");
                    success.slideDown().delay(3000).slideUp();
                } else {
                    error.text(res.message);
                    error.slideDown().delay(4000).slideUp();
                }
            },
            error: function () {
                error.text("Misslyckades med att anmäla dig");
                error.slideDown().delay(4000).slideUp();
            }
        });
    });

    $('.expandable').on('click', function (e) {
        var self = $(this);
        var target = $(event.target);
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

    $("#admin-add-signups").on('click', function(e) {
        e.preventDefault();
        $("#edit-signup-modal").modal("show");
    });

    $("#edit-signup-form").on('submit', function (e) {
        e.preventDefault();
        var form = $(this);
        var error = form.find(".alert-danger");
        var success = form.find(".alert-success");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (res) {
                if (res.success) {
                    success.text("Anmälan uppdaterad");
                    success.slideDown().delay(3000).slideUp();
                    form.trigger("reset");
                } else {
                    error.text(res.message);
                    error.slideDown().delay(4000).slideUp();
                }
            },
            error: function () {
                error.text("Misslyckades med att anmäla dig");
                error.slideDown().delay(4000).slideUp();
            }
        });
    });
    
});
