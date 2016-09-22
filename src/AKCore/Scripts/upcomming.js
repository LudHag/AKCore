$(function() {
    $('#signup-form').on('submit',function(e) {
        e.preventDefault();
        var form = $(this);
        var error = form.find(".alert-danger");
        $.ajax({
            url: form.attr("action"),
            type: "POST",
            data: form.serialize(),
            success: function (res) {
                if (res.success) {
                    //todo:reload signups
                } else {
                    error.text(res.message);
                    error.slideDown().delay(4000).slideUp();
                }
            },
            error: function (err) {
                error.text("Misslyckades med att anmäla dig");
                error.slideDown().delay(4000).slideUp();
            }
        });
    });
});
