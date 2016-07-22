$("#create-user")
    .on("click",
        function(e) {
            e.preventDefault();
            $("#createUserModal").modal("show");
        });

$("#create-user-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var success = $(".alert-success");
            var error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: form.attr("method"),
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        updateUserList();
                        $("#createUserModal").modal("hide");
                        success.text(res.message);
                        success.slideDown().delay(3000).slideUp();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(3500).slideUp();
                    }
                },
                error: function(err) {
                    error.text("Lyckades ej lägga till användare");
                    error.slideDown().delay(3500).slideUp();
                }
            });

        });

$("#user-table")
    .on("click",
        ".remove-user",
        function(e) {
            e.preventDefault();
            e.stopPropagation();
            var self = $(this);
            if (confirm("Vill du verkligen ta bort " + self.data("user") + "?")) {
                var success = $(".alert-success");
                var error = $(".alert-danger");
                $.ajax({
                    url: "/User/RemoveUser?userName=" + self.data("user"),
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            updateUserList();
                            success.text(res.message);
                            success.slideDown().delay(3000).slideUp();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(3500).slideUp();
                        }
                    },
                    error: function(err) {
                        error.text("Lyckades ej lägga till användare");
                        error.slideDown().delay(3500).slideUp();
                    }
                });
            }
        });

function updateUserList(search) {
    $.get("/User/UserList",
        function(data) {
            $("#user-table").empty().append(data);
        });
}