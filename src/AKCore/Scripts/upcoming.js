$(function() {
    const upcomingApp = $("#upcoming-app");
    if (upcomingApp.length > 0) {
        $("#edit-signup-form").on('submit', 
            function(e) {
                e.preventDefault();
                const form = $(this);
                const error = form.find(".alert-danger");
                const success = form.find(".alert-success");
                $.ajax({
                    url: form.attr("action"),
                    type: "POST",
                    data: form.serialize(),
                    success: function(res) {
                        if (res.success) {
                            success.text("Anmälan uppdaterad");
                            success.slideDown().delay(3000).slideUp();
                            form.trigger("reset");
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function() {
                        error.text("Misslyckades med att anmäla dig");
                        error.slideDown().delay(4000).slideUp();
                    }
                });
            });
    }
});
