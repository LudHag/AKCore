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
                    //todo:reload signups
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
});
