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

$('#search-user-form').on('submit', function (e) {
    e.preventDefault();
    updateUserList($('#searchtext').val());
});


$("#user-table").on('submit', '.add-role', function (e) {
    e.preventDefault();
    var form = $(this);
    var error = $(".alert-danger");
    var success = $(".alert-success");
    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function (res) {
            if (res.success) {
                success.text("Roll tillagd");
                success.slideDown().delay(4000).slideUp();
                updateUserList($('#searchtext').val());
            } else {
                error.text(res.message);
                error.slideDown().delay(4000).slideUp();
            }
        },
        error: function (err) {
            error.text("Misslyckades med att spara sida");
            error.slideDown().delay(4000).slideUp();
        }
    });
});


function updateUserList(search) {
    $.get("/User/UserList?SearchPhrase=" + search,
        function(data) {
            $("#user-table").empty().append(data);
        });
}