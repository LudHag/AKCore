
$("#update-profile-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            const form = $(this);
            const success = form.find(".alert-success");
            const error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: form.attr("method"),
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        form.parent().get(0).scrollIntoView();
                        success.text("Din profil uppdaterades");
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

$("#change-profile-password")
    .on("submit",
        function(e) {
            e.preventDefault();
            const form = $(this);
            const success = form.find(".alert-success");
            const error = form.find(".alert-danger");
            const newpass = form.find("#newpass");
            const confirmPass = form.find("#confirmpass");
            if (newpass.val() !== confirmPass.val()) {
                error.text("Lösenord matchar ej");
                error.slideDown().delay(3500).slideUp();
                return true;
            }

            $.ajax({
                url: form.attr("action"),
                type: form.attr("method"),
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        success.slideDown().delay(3000).slideUp();
                        newpass.val("");
                        confirmPass.val("");
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(3500).slideUp();
                    }
                },
                error: function(err) {
                    error.text("Misslyckades med att ändra lösenord");
                    error.slideDown().delay(3500).slideUp();
                }
            });
        });

$('#update-profile-form .multi-select').multiSelect({});