
$("#update-profile-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var success = form.find(".alert-success");
            var error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: form.attr("method"),
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        success.text(res.message);
                        success.slideDown().delay(3000).slideUp();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(3500).slideUp();
                    }
                },
                error: function(err) {
                    error.text("Uppdateringen misslyckades");
                    error.slideDown().delay(3500).slideUp();
                }
            });
        });
