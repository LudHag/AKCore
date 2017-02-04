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
                data: form.serialize() + "&WidgetsJson=" + jsonifyWidgets(),
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


$('#widget-area .multi-select').multiSelect(
{
    selectableHeader: "Uppladdade album",
    selectionHeader: "Valda album"
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
        "bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | styleselect code fullscreen",
    toolbar2:
        "searchreplace bullist | undo redo | link unlink image | hr removeformat | charmap table",
    table_appearance_options: false,
    menubar: false,
    elementpath: true,
    convert_urls: false,
    style_formats: [
        { title: 'Vanlig text', block: 'p' },
        { title: 'Stor text', block: 'p', classes: 'big' },
        { title: 'Rubrik 1', block: 'h1'},
        { title: 'Rubrik 2', block: 'h2' },
        { title: 'Infobox', selector: 'p', classes: 'infobox' },
        { title: '3-delskolumn', block: 'p', classes: 'col-sm-4' }
    ],
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
        if (type === "file") {
        {
            console.log('not implemented yet!');
        }}
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
    .on("click",'.add-video-link',function(e) {
        e.preventDefault();
        var area = $(this).parent().prev();
        area.append('<div class="form-group video-area row"><div class="col-sm-6"><input class="form-control video-link" value=""></div><div class="col-sm-6"><input class="form-control video-title" value=""><a href="#" class="btn remove-video glyphicon glyphicon-remove"></a></div></div>');
    });
$("#widget-area")
    .on("click", '.remove-video', function (e) {
        e.preventDefault();
        var link = $(this).parent().parent();
        link.remove();
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
        function(e) {
            e.preventDefault();
            if (window.confirm("Är du säker att du vill ta bort den här widgeten?")) {
                $(this).parent().parent().remove();
            }
        });
$("#widget-area")
    .on("click",
        ".min-widget",
        function(e) {
            e.preventDefault();
            var widget = $(this).parent().parent();
            widget.toggleClass("minimized");
        });

function updatePickerSearch() {
    var st = $("#searchtext").val();
    var page = $(".pagination li.active a").data("page");
    updateMediaPickerList(st, page);
}

function updateMediaPickerList(search, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page + "&Type=Image",
        function(data) {
            $("#picker-images").empty().append(data);
        });
}

var templates = $("#widget-templates");
if (templates.length > 0) {
    var textImageTemplate = templates.find(".TextImage");
    var textTemplate = templates.find(".Text");
    var imageTemplate = templates.find(".Image");
    var videoTemplate = templates.find(".Video");
    var musicTemplate = templates.find(".Music");
    var joinTemplate = templates.find(".Join");
    var memberListTemplate = templates.find(".MemberList");
    tinymce.init(options);
    $(".widget-choose")
        .on("click",
            "a",
            function(e) {
                e.preventDefault();
                var type = $(this).data("type");
                if (type === "TextImage") {
                    $("#widget-area").append(textImageTemplate.clone());
                    tinymce.init(options);
                } else if (type === "Text") {
                    $("#widget-area").append(textTemplate.clone());
                    tinymce.init(options);
                } else if (type === "Image") {
                    $("#widget-area").append(imageTemplate.clone());
                }
                else if (type === "Video") {
                    $("#widget-area").append(videoTemplate.clone());
                }else if (type === "Music") {
                    $("#widget-area").append(musicTemplate.clone());
                    $('#widget-area .multi-select').multiSelect(
                    {
                        selectableHeader: "Uppladdade album",
                        selectionHeader: "Valda album"
                    });
                }
            });
    $(".widget-choose-special")
        .on("click",
            "a",
            function(e) {
                e.preventDefault();
                var type = $(this).data("type");
                if (type === "Join") {
                    $("#widget-area").append(joinTemplate.clone());
                    tinymce.init(options);
                } else if (type === "MemberList") {
                    $("#widget-area").append(memberListTemplate.clone());
                }
            });

    $("#widget-area")
        .sortable({
            axis: "y",
            handle: ".widget-header",
            distance: 30,
            start: function(e, ui) {
                $(ui.item)
                    .find("textarea")
                    .each(function() {
                        tinymce.execCommand("mceRemoveEditor", false, $(this).attr("id"));
                    });
            },
            stop: function(e, ui) {
                $(ui.item)
                    .find("textarea")
                    .each(function() {
                        tinymce.execCommand("mceAddEditor", true, $(this).attr("id"));
                    });
            }
        });
}

function jsonifyWidgets() {
    var widgets = [];
    $("#widget-area")
        .find(".widget")
        .each(function(i, o) {
            var wig = new Object();
            var type = $(o).data("type");
            wig.Type = type;
            var tId;
            if (type === "Text") {
                tId = $(o).find(".mce-content").attr("id");
                wig.Text = tinymce.get(tId).getContent();
            } else if (type === "Image") {
                wig.Image = $(o).find(".selected-image").attr("src");
            }
            else if (type === "Video") {
                wig.Videos = [];
                $(o)
                    .find(".video-area")
                    .each(function() {
                        var Video = new Object();
                        Video.Link = $(this).find('.video-link').val();
                        Video.Title = $(this).find('.video-title').val();
                        wig.Videos.push(Video);
                    });
            } else if (type === "Music") {
                wig.Albums = $(o).find(".album").val();
            } else if (type === "TextImage") {
                tId = $(o).find(".mce-content").attr("id");
                wig.Text = tinymce.get(tId).getContent();
                wig.Image = $(o).find(".selected-image").attr("src");
            }
            widgets.push(wig);
        });
    var res = JSON.stringify(widgets);
    return encodeURIComponent(res);
}