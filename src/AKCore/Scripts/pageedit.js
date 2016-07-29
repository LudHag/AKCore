$("#new-page-button")
    .on("click",
        function(e) {
            e.preventDefault();
            $("#new-page-form").toggle();

        });

$("#new-page-form")
    .on("submit",
        "form",
        function(e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function(res) {
                    if (res.success) {
                        window.location.reload();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function(err) {
                    error.text("Misslyckades med att skapa sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });


        });

$("#page-edit")
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
                        success.text(res.message);
                        success.slideDown().delay(4000).slideUp();
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

$("#start-page")
    .on("click",
        function() {
            var slug = $("#slug");
            if (slug.prop("readonly")) {
                slug.prop("readonly", false);
                slug.val(slug.data("oldslug"));
            } else {
                slug.data("oldslug", slug.val());
                slug.val("/");
                slug.prop("readonly", true);
            }

        });


var options = {
    selector: "#widget-area .mce-content",
    theme: "modern",
    plugins: [
        "advlist link image imagetools lists charmap print hr anchor spellchecker searchreplace wordcount code fullscreen media nonbreaking",
        "table contextmenu directionality emoticons template textcolor paste textcolor colorpicker textpattern"
    ],
    toolbar1:
        "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | formatselect code",
    toolbar2:
        "searchreplace bullist | undo redo | link unlink image | hr removeformat | charmap emoticons |  fullscreen ",
    menubar: false,
    elementpath: false,
    convert_urls: false,
    toolbar_items_size: "small",
    width: "500",
    height: "200",
    content_css: "/css/akstyle.css",
    body_class: "body-content",
    browser_spellcheck: true,
    file_browser_callback: function(field_name, url, type, win) {
        if (type === "image") {
            $("#imagePickerModal").modal("show");
            $("#picker-images").off("click", ".image-box");
            $("#picker-images")
                .on("click",
                    ".image-box",
                    function(e) {
                        $("#imagePickerModal").modal("hide");
                        $("#" + field_name).val($(this).data("path"));
                        $("#imagePickerModal").modal("hide");
                    });

        }
    }
};

$("#picker-images")
    .on("click",
        ".pagination li",
        function(e) {
            e.preventDefault();
            var self = $(this);
            if (!self.hasClass("active")) {
                $(".pagination li").removeClass("active");
                self.addClass("active");
                updatePickerSearch();
            }
        });

$("#search-pickermedia-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            $(".pagination li").removeClass("active");
            $($(".pagination li")[0]).addClass("active");
            updatePickerSearch();
        });
$("#imagePickerModal")
    .on("shown.bs.modal",
        function(e) {
            updateMediaPickerList("", "");
        });
$("#imagePickerModal")
    .on("hidden.bs.modal",
        function(e) {
            $("#searchtext").val("");
        });

$("#widget-area")
    .on("click",
        ".choose-picture-btn",
        function(e) {
            e.preventDefault();
            var self = $(this);
            var target = self.parent().find(".selected-image");
            $("#imagePickerModal").modal("show");
            $("#picker-images").off("click", ".image-box");
            $("#picker-images")
                .on("click",
                    ".image-box",
                    function(e) {
                        $("#imagePickerModal").modal("hide");
                        target.attr("src", $(this).data("path"));
                    });
        });

$("#widget-area")
    .on("click",
        ".remove-widget",
        function (e) {
            e.preventDefault();
            $(this).parent().parent().remove();
        });

function updatePickerSearch() {
    var st = $("#searchtext").val();
    var page = $(".pagination li.active a").data("page");
    updateMediaPickerList(st, page);
}

function updateMediaPickerList(search, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page,
        function(data) {
            $("#picker-images").empty().append(data);
        });
}

var templates = $("#widget-templates");
if (templates.length > 0) {
    var textImageTemplate = templates.find(".textimage");
    var textTemplate = templates.find(".text");
    var imageTemplate = templates.find(".image");
    $(".widget-choose")
        .on("click",
            "a",
            function(e) {
                e.preventDefault();
                var type = $(this).data("type");
                if (type === "textimage"){
                    $("#widget-area").append(textImageTemplate.clone());
                    tinymce.init(options);
                }else if (type === "text") {
                    $("#widget-area").append(textTemplate.clone());
                    tinymce.init(options);
                } else if (type === "image") {
                    $("#widget-area").append(imageTemplate.clone());
                }
            });
    $("#widget-area").sortable({
        axis: "y",
        handle: ".widget-header",
        start: function (e, ui) {
            $(ui.item).find('textarea').each(function () {
                tinymce.execCommand('mceRemoveEditor', false, $(this).attr('id'));
            });
        },
        stop: function (e, ui) {
            $(ui.item).find('textarea').each(function () {
                tinymce.execCommand('mceAddEditor', true, $(this).attr('id'));
            });
        }
    });
}

