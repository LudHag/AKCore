$(".account")
    .on("click",
        ".login",
        function(e) {
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
                        success.text(res.message);
                        success.slideDown().delay(4000).slideUp();
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
