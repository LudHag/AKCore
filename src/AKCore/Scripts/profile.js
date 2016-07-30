
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

$("#change-profile-password")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var success = form.find(".alert-success");
            var error = form.find(".alert-danger");
            var newpass = form.find("#newpass");
            var confirmPass = form.find("#confirmpass");
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

$("#fb-connect")
    .on("click",
        function(e) {
            e.preventDefault();
            fbGetLoginId(function(fbId) {
                $.ajax({
                    url: "/Profile/FbConnect?fbId=" + fbId,
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            $("#fb-connect").hide();
                            $("#fb-disconnect").show();
                        } else {
                        }
                    },
                    error: function(err) {
                        console.log(err);
                    }
                });
            });
        });

$("#fb-disconnect")
    .on("click",
        function(e) {
            e.preventDefault();
            fbGetLoginId(function(fbId) {
                $.ajax({
                    url: "/Profile/RemoveFbConnect?fbId=" + fbId,
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            $("#fb-disconnect").hide();
                            $("#fb-connect").show();
                        } else {
                        }
                    },
                    error: function(err) {
                        console.log(err);
                    }
                });
            });

        });