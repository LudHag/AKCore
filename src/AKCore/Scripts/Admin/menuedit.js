$("#add-menu")
    .on("click",
        function(e) {
            e.preventDefault();
            $("#edit-menu-container").toggle();
        });
$("#add-top-menu")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            var success = form.find(".alert-success");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        updateMenuList();
                        success.slideDown().delay(3000).slideUp();
                        setTimeout(function() {
                                $("#edit-menu-container").slideUp();
                                $("#name").val("");
                            },
                            2000);
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function(err) {
                    error.text("Misslyckades med att spara sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });

        });

$("#menus")
    .on("click",
        ".remove-menu",
        function(e) {
            e.preventDefault();
            var success = $("#menualerts").find(".alert-success");
            var error = $("#menualerts").find(".alert-danger");
            var self = $(this);
            if (window.confirm("Vill du verkligen ta bort menyn: " + self.data("name"))) {
                $.ajax({
                    url: "/MenuEdit/RemoveTopMenu?id=" + self.data("id"),
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            updateMenuList();
                            success.text("Meny borttagen");
                            success.slideDown().delay(3500).slideUp();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(3500).slideUp();
                        }
                    },
                    error: function(err) {
                        error.text("Lyckades ej ta bort meny");
                        error.slideDown().delay(3500).slideUp();
                    }
                });
            }
        });
$("#menus")
    .on("click",
        ".add-sub-menu",
        function(e) {
            e.preventDefault();
            $("#addSubMenuModal").find(".parentId").val($(this).data("id"));
            $("#addSubMenuModal").modal("show");
        });

$("#add-submenu-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var success = $("#menualerts").find(".alert-success");
            var error = $("#menualerts").find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (res) {
                    $("#addSubMenuModal").modal("hide");
                    if (res.success) {
                        updateMenuList();
                        success.text("Submeny tillagd");
                        success.slideDown().delay(3500).slideUp();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function (err) {
                    $("#addSubMenuModal").modal("hide");
                    error.text("Misslyckades med att spara sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });
        });
$("#menus")
    .on("click",
        ".remove-sub-menu",
        function (e) {
            e.preventDefault();
            var success = $("#menualerts").find(".alert-success");
            var error = $("#menualerts").find(".alert-danger");
            var self = $(this);
            if (window.confirm("Vill du verkligen ta bort menyn: " + self.data("name"))) {
                $.ajax({
                    url: "/MenuEdit/RemoveSubMenu?id=" + self.data("id"),
                    type: "POST",
                    success: function (res) {
                        if (res.success) {
                            updateMenuList();
                            success.text("Meny borttagen");
                            success.slideDown().delay(3500).slideUp();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(3500).slideUp();
                        }
                    },
                    error: function (err) {
                        error.text("Lyckades ej ta bort meny");
                        error.slideDown().delay(3500).slideUp();
                    }
                });
            }
        });

$("#menus")
    .on("click",
        ".menu",
        function(e) {
            e.preventDefault();
            var self = $(this);
            var form = $('#edit-menu-form');
            var id = form.find('.menuId');
            var name = form.find('.name');
            var link = form.find('.page');
            var parent = form.find('.parentId');
            var loggedIn = form.find('.logged');
            var balett = form.find('.balett');
            loggedIn.prop('checked', self.data('logged') === "True");
            balett.prop('checked', self.data('balett') === "True");
            name.val(self.text());
            link.val(self.data('link'));
            link.removeAttr('required');
            link.trigger("chosen:updated");
            id.val(self.data('id'));
            parent.val('false');
            $('#editMenuModal').modal('show');
        });

$("#menus")
    .on("click",
        ".submenu",
        function (e) {
            e.preventDefault();
            var self = $(this);
            var form = $('#edit-menu-form');
            var id = form.find('.menuId');
            var parent = form.find('.parentId');
            var name = form.find('.name');
            var link = form.find('.page');
            link.attr('required', true);
            name.val(self.text());
            link.val(self.data('link'));
            link.trigger("chosen:updated");
            id.val(self.data('id'));
            parent.val('true');
            $('#editMenuModal').modal('show');
        });

$("#edit-menu-form")
    .on("submit",
         function (e) {
             e.preventDefault();
             var form = $(this);
             var success = $("#menualerts").find(".alert-success");
             var error = form.find(".alert-danger");
             $.ajax({
                 url: form.attr("action"),
                 type: "POST",
                 data: form.serialize(),
                 success: function (res) {
                     if (res.success) {
                         updateMenuList();
                         $('#editMenuModal').modal('hide');
                         success.text("Meny redigerad");
                         success.slideDown().delay(3000).slideUp();
                     } else {
                         error.text(res.message);
                         error.slideDown().delay(4000).slideUp();
                     }
                 },
                 error: function (err) {
                     error.text("Misslyckades med att redigera sida");
                     error.slideDown().delay(4000).slideUp();
                 }
             });
         });

$("#menus")
    .on("click",
        ".move-left",
        function(e) {
            e.preventDefault();
            var self = $(this);

            $.ajax({
                url: "/MenuEdit/MoveLeft?id=" + self.data("id"),
                type: "POST",
                success: function (res) {
                    if (res.success) {
                        updateMenuList();
                    } else {
                        console.log(res);
                    }
                },
                error: function(err) {
                    console.log(err);
                }
            });
        });
$("#menus")
    .on("click",
        ".move-right",
        function (e) {
            e.preventDefault();
            var self = $(this);

            $.ajax({
                url: "/MenuEdit/MoveRight?id=" + self.data("id"),
                type: "POST",
                success: function (res) {
                    if(res.success){
                        updateMenuList();
                    } else {
                        console.log(res);
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
$("#menus")
    .on("click",
        ".move-up",
        function (e) {
            e.preventDefault();
            var self = $(this);

            $.ajax({
                url: "/MenuEdit/MoveUp?id=" + self.data("id")+"&parent="+ self.data("top"),
                type: "POST",
                success: function (res) {
                    if (res.success) {
                        updateMenuList();
                    } else {
                        console.log(res);
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });
$("#menus")
    .on("click",
        ".move-down",
        function (e) {
            e.preventDefault();
            var self = $(this);

            $.ajax({
                url: "/MenuEdit/MoveDown?id=" + self.data("id") + "&parent=" + self.data("top"),
                type: "POST",
                success: function (res) {
                    if (res.success) {
                        updateMenuList();
                    } else {
                        console.log(res);
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });
        });

function updateMenuList() {
    $.get("/MenuEdit/MenuList",
        function(data) {
            $("#menus").empty().append(data);
        });
}