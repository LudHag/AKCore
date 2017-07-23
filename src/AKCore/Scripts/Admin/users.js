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
                        updateUserList("");
                        $("#createUserModal").modal("hide");
                        success.text(res.message);
                        success.slideDown().delay(3000).slideUp();
                        $('#create-user-form').trigger("reset");
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

$("#user-table").on("click", ".edit-user-info", function (e) {
    e.preventDefault();
    var userName = $(this).data("user");

    $.get("/User/EditUserInfo?userName=" + userName,
        function (data) {
            $("#editUserModal").empty().append(data);
            $("#editUserModal").modal("show");
            $("#editUserModal .multi-select").multiSelect({});
        });
});

$("#editUserModal").on("submit", "#edit-user-form", function (e) {
    e.preventDefault();
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
                $("#editUserModal").modal("hide");
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
                            updateUserList("");
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

$("#search-user-form").on("submit", function (e) {
    e.preventDefault();
    updateUserList($("#searchtext").val());
});


$("#user-table").on("submit", ".add-role", function (e) {
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
                var id = "#roles-" + form.find("input[name=UserName]").val();
                updateRoles(id);
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
$("#user-table").on("submit", ".save-medal", function (e) {
    e.preventDefault();
    var form = $(this);
    var error = $(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function (res) {
            if (!res.success) {
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


$('.add-post .multi-select').multiSelect({});
$("#user-table").on("reset", ".add-post", function (e) {
    e.preventDefault();
    var multi = $(this).find('.multi-select');
    multi.val([]);
    multi.multiSelect('refresh');
});

$("#user-table").on("submit", ".add-post", function (e) {
    e.preventDefault();
    var form = $(this);
    var success = $(".alert-success");
    var error = $(".alert-danger");
    $.ajax({
        url: form.attr("action"),
        type: form.attr("method"),
        data: form.serialize(),
        success: function (res) {
            if (res.success) {
                success.text("Post(er) tillagda");
                success.slideDown().delay(4000).slideUp();
            } else {
                error.text(res.message);
                error.slideDown().delay(4000).slideUp();
            }
        },
        error: function (err) {
            error.text("Misslyckades med att lägga till poster");
            error.slideDown().delay(4000).slideUp();
        }
    });
});
$("#user-table")
    .on("reset",
        ".add-post",
        function(e) {
            e.preventDefault();
            var form = $(this);
            form.find('select').val('');
        });

$("#user-table")
    .on('click',
        '.reset-pass-btn',
        function(e) {
            e.preventDefault();
            $('#change-user-name').val($(this).data("user"));
            $('#changePasswordModal').modal('show');
        });
$("#change-pass-form")
    .on('submit',
        function(e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            var success = $(".alert-success");
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

$("#user-table")
    .on('click',
        '.remove-role',
        function(e) {
            e.preventDefault();
            e.stopPropagation();
            var self = $(this);

            var error = $(".alert-danger");
            $.ajax({
                url: '/User/RemoveRole?UserName=' + self.data('user') + '&Role=' + self.data('role'),
                type: 'POST',
                success: function (res) {
                    if (res.success) {
                        self.parent().remove();
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

function updateRoles(roleId) {
    $.get("/User/UserList",
        function (data) {
            $(roleId).empty().append($(data).find(roleId).children());
        });
}