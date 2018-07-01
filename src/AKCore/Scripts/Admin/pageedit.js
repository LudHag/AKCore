$("#new-page-button")
    .on("click",
        function(e) {
            e.preventDefault();
            $("#new-page-form").toggle();

        });

$(".chosen-select").chosen({ width: '100%'});

$(".remove-page").on("click", function(e) {
    e.preventDefault();
    if (window.confirm("Är du säker på att du vill ta bort denna sida?")) {
        window.location.href = $(this).attr("href");
    }
});

$("#new-page-form")
    .on("submit",
        "form",
        function(e) {
            e.preventDefault();
            const form = $(this);
            const error = form.find(".alert-danger");
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

function updateRevisions(revlink) {
    const revisions = $(".revisions");
    if (revisions) {
        const revisionList = revisions.find(".revision");
        if (revisionList.length > 4) {
            revisionList.first().remove();
        }
        revisions.append(revlink);
    }
}

$("#page-edit")
    .on("submit",
        function(e) {
            e.preventDefault();
            const form = $(this);
            const isRev = form.data("isrev");
            const error = form.find(".alert-danger");
            const success = form.find(".alert-success");
            if (!isRev || window.confirm("Är du säker på att du vill ersätta sidan med denna version?")) {
                $.ajax({
                    url: form.attr("action"),
                    type: "POST",
                    data: form.serialize() + "&WidgetsJson=" + jsonifyWidgets(),
                    success: function(res) {
                        if (res.success) {
                            if (isRev) {
                                window.location.href = form.attr("action");
                            } else {
                                success.text(res.message);
                                success.slideDown().delay(4000).slideUp();
                                updateRevisions(res.revlink);
                            }
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
            }
        });


$('#widget-area .multi-select').multiSelect(
{
    selectableHeader: "Uppladdade album",
    selectionHeader: "Valda album"
});

$("#start-page")
    .on("click",
        function() {
            const slug = $("#slug");
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
        { title: '3-delskolumn', block: 'p', classes: 'col-sm-4' },
        { title: 'Block med marginal', block: 'p', classes: 'col-xs-12' }
    ],
    toolbar_items_size: "small",
    height: "200",
    content_css: "/dist/style.css",
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
                    });
        }
        if (type === "file") {
        {
            $("#filePickerModal").modal("show");
            $("#picker-files").off("click", ".image-box");
            $("#picker-files")
                .on("click",
                ".image-box",
                function (e) {
                    $("#filePickerModal").modal("hide");
                    $("#" + field_name).val($(this).data("path"));
                });
        }}
    }
};

$("#picker-images")
    .on("click",
        ".pagination li",
        function(e) {
            e.preventDefault();
            const self = $(this);
            if (!self.hasClass("active")) {
                $("#imagePickerModal .pagination li").removeClass("active");
                self.addClass("active");
                updatePickerSearch();
            }
        });

$("#picker-files")
    .on("click",
        ".pagination li",
        function (e) {
            e.preventDefault();
            const self = $(this);
            if (!self.hasClass("active")) {
                $("#filePickerModal .pagination li").removeClass("active");
                self.addClass("active");
                updateFilePickerSearch();
            }
        });

$("#search-pickermedia-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            $("#imagePickerModal .pagination li").removeClass("active");
            $($("#imagePickerModal .pagination li")[0]).addClass("active");
            updatePickerSearch();
    });

$("#filesearch-pickermedia-form")
    .on("submit",
    function (e) {
        e.preventDefault();
        $("#filePickerModal .pagination li").removeClass("active");
        $($("#filePickerModal .pagination li")[0]).addClass("active");
        updateFilePickerSearch();
    });

$("#search-pickermedia-form").on("change", "#search-mediatags", function (e) {
    $("#imagePickerModal .pagination li").removeClass("active");
    $($("#imagePickerModal .pagination li")[0]).addClass("active");
    updatePickerSearch();
});

$("#imagePickerModal")
    .on("shown.bs.modal",
        function(e) {
            updateMediaPickerList("", "");
    });
$("#filePickerModal")
    .on("shown.bs.modal",
    function (e) {
        updateFilePickerList("", "");
    });


$("#imagePickerModal")
    .on("hidden.bs.modal",
        function(e) {
            $("#searchtext").val("");
    });
$("#filePickerModal")
    .on("hidden.bs.modal",
    function (e) {
        $("#filesearchtext").val("");
    });

$("#widget-area")
    .on("click",'.add-video-link',function(e) {
        e.preventDefault();
        const area = $(this).parent().prev();
        area.append('<div class="form-group video-area row"><div class="col-sm-6"><input class="form-control video-link" value=""></div><div class="col-sm-6"><input class="form-control video-title" value=""><a href="#" class="btn remove-video glyphicon glyphicon-remove"></a></div></div>');
    });
$("#widget-area")
    .on("click", '.remove-video', function (e) {
        e.preventDefault();
        const link = $(this).parent().parent();
        link.remove();
    });

$("#widget-area")
    .on("click",
        ".choose-picture-btn",
        function(e) {
            e.preventDefault();
            const self = $(this);
            const target = self.parent().find(".selected-image");
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
            const widget = $(this).parent().parent();
            widget.toggleClass("minimized");
        });

function updatePickerSearch() {
    const st = $("#searchtext").val();
    const tag = $("#search-mediatags").val();
    const page = $("#imagePickerModal .pagination li.active a").data("page");
    updateMediaPickerList(st, tag, page);
}

function updateMediaPickerList(search, tag, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page + "&Type=Image" + "&Tag=" + tag,
        function(data) {
            $("#picker-images").empty().append(data);
        });
}

function updateFilePickerSearch() {
    const st = $("#filesearchtext").val();
    const page = $("#filePickerModal .pagination li.active a").data("page");
    updateFilePickerList(st, page);
}

function updateFilePickerList(search, page) {
    $.get("/Media/MediaPickerList?SearchPhrase=" + search + "&Page=" + page + "&Type=Document",
        function (data) {
            $("#picker-files").empty().append(data);
        });
}


var templates = $("#widget-templates");
if (templates.length > 0) {
    const textImageTemplate = templates.find(".TextImage");
    const headerTextTemplate = templates.find(".HeaderText");
    const threePuffsTemplate = templates.find(".ThreePuffs");
    const textTemplate = templates.find(".Text");
    const imageTemplate = templates.find(".Image");
    const videoTemplate = templates.find(".Video");
    const musicTemplate = templates.find(".Music");
    const joinTemplate = templates.find(".Join");
    const hireTemplate = templates.find(".Hire");
    const memberListTemplate = templates.find(".MemberList");
    const postListTemplate = templates.find(".PostList");
    tinymce.init(options);
    $(".widget-choose")
        .on("click",
            "a",
            function(e) {
                e.preventDefault();
                const type = $(this).data("type");
                if (type === "TextImage") {
                    $("#widget-area").append(textImageTemplate.clone());
                    tinymce.init(options);
                } else if (type === "Text") {
                    $("#widget-area").append(textTemplate.clone());
                    tinymce.init(options);
                } else if (type === "HeaderText") {
                    $("#widget-area").append(headerTextTemplate.clone());
                    tinymce.init(options);
                } else if (type === "Image") {
                    $("#widget-area").append(imageTemplate.clone());
                } else if (type === "Video") {
                    $("#widget-area").append(videoTemplate.clone());
                } else if (type === "Music") {
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
                const type = $(this).data("type");
                if (type === "Join") {
                    $("#widget-area").append(joinTemplate.clone());
                    tinymce.init(options);
                }else if (type === "Hire") {
                    $("#widget-area").append(hireTemplate.clone());
                    tinymce.init(options);
                } else if (type === "MemberList") {
                    $("#widget-area").append(memberListTemplate.clone());
                } else if (type === "PostList") {
                    $("#widget-area").append(postListTemplate.clone());
                } else if (type === "ThreePuffs") {
                    $("#widget-area").append(threePuffsTemplate.clone());
                    tinymce.init(options);
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
    const widgets = [];
    $("#widget-area")
        .find(".widget")
        .each(function(i, o) {
            const wig = new Object();
            const type = $(o).data("type");
            wig.Type = type;
            let tId;
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
                    .each(function () {
                        const Video = new Object();
                        Video.Link = $(this).find('.video-link').val();
                        Video.Title = $(this).find('.video-title').val();
                        wig.Videos.push(Video);
                    });
            } else if (type === "Music") {
                wig.Albums = $(o).find(".album").val();
            } else if (type === "TextImage" || type === "Join" || type === "Hire" || type === "HeaderText" || type === "ThreePuffs") {
                tId = $(o).find(".mce-content").attr("id");
                wig.Text = tinymce.get(tId).getContent();
                wig.Image = $(o).find(".selected-image").attr("src");
            } 
            widgets.push(wig);
        });
    const res = JSON.stringify(widgets);
    return encodeURIComponent(res);
}