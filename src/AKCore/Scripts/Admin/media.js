var uploadForm = $("#upload-form");
var droppedFiles = false;

if (uploadForm.length > 0) {
    var $input = uploadForm.find('input[type="file"]'),
        $choose = uploadForm.find(".choose-file"),
        $label = uploadForm.find(".file-name"),
        showFiles = function(files) {
            droppedFiles = files;
            $label.text(files[0].name);
            $choose.hide();
            $label.show();
        };

    uploadForm.on("drop",
        function(e) {
            showFiles(e.originalEvent.dataTransfer.files);
        });

    $input.on("change",
        function(e) {
            showFiles(e.target.files);
        });

    uploadForm.on("drag dragstart dragend dragover dragenter dragleave drop",
            function(e) {
                e.preventDefault();
                e.stopPropagation();
            })
        .on("dragover dragenter",
            function() {
                uploadForm.addClass("is-dragover");
            })
        .on("dragleave dragend drop",
            function() {
                uploadForm.removeClass("is-dragover");
            })
        .on("drop",
            function(e) {
                droppedFiles = e.originalEvent.dataTransfer.files;
            });
}

$("#upload-file-btn")
    .on("click",
        function(e) {
            e.preventDefault();
            uploadForm.submit();
        });

uploadForm.on("submit",
    function(e) {
        e.preventDefault();
        var form = $(this);
        var mediaData = new FormData();
        mediaData.append("UploadFile", droppedFiles[0]);
        mediaData.append("Tag", $('#upload-mediatags').val());
        var success = $(".alert-success");
        var error = $(".alert-danger");
        $.ajax({
            type: "POST",
            url: "/Media/UploadFile",
            contentType: false,
            processData: false,
            data: mediaData,
            success: function(res) {
                if (res.success) {
                    success.slideDown().delay(3000).slideUp();
                    updateMediaList("", "Image", "1");
                    form.find('.box__file').val('');
                    droppedFiles = false;
                    form.find('.file-name').text('Dra hit filen');
                    $('#upload-mediatags').val("Allmän")
                } else {
                    error.text(res.message);
                    error.slideDown().delay(3000).slideUp();
                }
            },
            error: function(err) {
                error.text("Misslyckades med att ladda upp filen");
                error.slideDown().delay(3000).slideUp();
            }
        });

    });
$("#search-media-form")
    .on("submit",
        function(e) {
            e.preventDefault();
            $(".pagination li").removeClass("active");
            $($(".pagination li")[0]).addClass("active");
            updateSearch();
        });
$("#search-media-form").on("change", "#search-mediatags", function (e) {
    $(".pagination li").removeClass("active");
    $($(".pagination li")[0]).addClass("active");
    updateSearch();
});
$("#uploaded-files")
    .on("click",
        ".remove-media",
        function(e) {
            e.preventDefault();
            var self = $(this);
            if (window.confirm("Är du säker på att du vill ta bort filen: " + self.data("name"))) {
                var error = $(".alert-danger");
                $.ajax({
                    url: "/Media/RemoveFile?filename=" + self.data("name"),
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            updateSearch();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function() {
                        error.text("Misslyckades med att ta bort fil");
                        error.slideDown().delay(4000).slideUp();
                    }
                });
            }
        });

$("#uploaded-files")
    .on("click",
        ".pagination li",
        function(e) {
            e.preventDefault();
            var self = $(this);
            if (!self.hasClass("active")) {
                $(".pagination li").removeClass("active");
                self.addClass("active");
                updateSearch();
            }
        });

$("#uploaded-files")
    .on("click", ".folder-box", function (e) {
        e.preventDefault();
        var tag = $(this).data("tag");
        $("#search-mediatags").val(tag);
        updateSearch();
    });

$("#uploaded-files")
    .on("click", ".back-arrow", function (e) {
        e.preventDefault();
        $("#searchtext").val("");
        $("#search-mediatags").val("");
        updateMediaList("", "", 1);
    });


function updateSearch() {
    var st = $("#searchtext").val();
    var tag = $("#search-mediatags").val();
    var page = $(".pagination li.active a").data('page');
    updateMediaList(st, tag, page);
}

function updateMediaList(search, tag, page) {
    $.get("/Media/MediaList?SearchPhrase=" + search + "&Page=" + page + "&Tag=" + tag,
        function(data) {
            $("#uploaded-files").empty().append(data);
        });
}