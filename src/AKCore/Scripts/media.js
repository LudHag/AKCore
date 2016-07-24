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
        var mediaData = new FormData();
        mediaData.append("UploadFile", droppedFiles[0]);
        var success = $(".alert-success");
        var error = $(".alert-danger");
        $.ajax({
            type: "POST",
            url: "/Media/UploadFile",
            contentType: false, 
            processData: false,
            data: mediaData,
            success: function (res) {
                if (res.success) {
                    success.slideDown().delay(3000).slideUp();
                } else {
                    error.text(res.message);
                    error.slideDown().delay(3000).slideUp();
                }
            },
            error: function (err) {
                error.text("Misslyckades med att ladda upp filen");
                error.slideDown().delay(3000).slideUp();
            }
        });

    });