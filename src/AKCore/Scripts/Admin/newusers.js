const userapp = $("#user-app");
if (userapp.length > 0) {
    $("#user-app").on("click", ".edit-user-info", function (e) {
        e.preventDefault();
        const userName = $(this).data("user");

        $.get("/User/EditUserInfo?userName=" + userName,
            function (data) {
                $("#editUserModal").empty().append(data);
                $("#editUserModal").modal("show");
                $("#editUserModal .multi-select").multiSelect({});
            });
    });

    $("#change-pass-form")
        .on('submit',
            function (e) {
                e.preventDefault();
                const form = $(this);
                const error = form.find(".alert-danger");
                const success = $(".alert-success");
                $.ajax({
                    url: form.attr("action"),
                    type: form.attr("method"),
                    data: form.serialize(),
                    success: function (res) {
                        if (res.success) {
                            $('#change-user-pass').val("");
                            $('#changePasswordModal').modal('hide');
                            success.text('Användarens lösenord ändrat');
                            success.slideDown().delay(3000).slideUp();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(3000).slideUp();
                        }
                    },
                    error: function (err) {
                        error.text("Misslyckades med att byta lösenord");
                        error.slideDown().delay(3000).slideUp();
                    }
                });
            });

}
