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
}
