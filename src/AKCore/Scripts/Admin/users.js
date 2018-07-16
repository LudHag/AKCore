import Vue from "vue";
import UsersApp from "../VueComponents/Users/UsersApp";

const usercontainer = $("#user-container");
if (usercontainer.length > 0) {
    let usersApp = new Vue({
        el: `#user-app`,
        template: "<users-app />",
        components: { UsersApp }
    });
    Vue.directive('tooltip',
        function(el, binding) {
            $(el).tooltip({
                title: binding.value,
                placement: binding.arg,
                trigger: 'hover'
            });
        });

    usercontainer.on("click",
        ".edit-user-info",
        function(e) {
            e.preventDefault();
            const userName = $(this).data("user");

            $.get("/User/EditUserInfo?userName=" + userName,
                function(data) {
                    $("#editUserModal").empty().append(data);
                    $("#editUserModal").modal("show");
                    $("#editUserModal .multi-select").multiSelect({});
                });
        });

    $("#change-pass-form")
        .on('submit',
            function(e) {
                e.preventDefault();
                const form = $(this);
                const error = form.find(".alert-danger");
                const success = $(".alert-success");
                $.ajax({
                    url: form.attr("action"),
                    type: form.attr("method"),
                    data: form.serialize(),
                    success: function(res) {
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
                    error: function(err) {
                        error.text("Misslyckades med att byta lösenord");
                        error.slideDown().delay(3000).slideUp();
                    }
                });
            });
    usercontainer.on("click",
        "#create-user",
        function(e) {
            e.preventDefault();
            $("#createUserModal").modal("show");
        });

    $("#create-user-form")
        .on("submit",
            function(e) {
                e.preventDefault();
                const form = $(this);
                var success = $(".alert-success");
                var error = form.find(".alert-danger");
                $.ajax({
                    url: form.attr("action"),
                    type: form.attr("method"),
                    data: form.serialize(),
                    success: function(res) {
                        if (res.success) {
                            usersApp = new Vue({
                                el: `#user-app`,
                                template: "<users-app />",
                                components: { UsersApp }
                            });
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
}