var albumTemplate = $('<div class="row"><div class="col-md-2 image">' +
    '<img class="album-img" /></div><div class="col-md-4 name">' +
    '<input type="text" class="name-input"></div><div class="col-md-2 actions">' +
    '<a href="#" class="del-album btn glyphicon glyphicon-remove"></a></div>' +
    '<div class="col-md-4 tracks"><span class="tracks-info"></span></div></div>');

function renderAlbums() {
    $('#album-list').empty();
    Object.keys(albums).forEach(function (key) {
        var a = albumTemplate.clone();
        a.find('.name-input').val($("<div>").html(albums[key].name).text());
        a.find('.tracks-info').html(albums[key].tracksCount + " spår uppladdade.<br>Klicka här för att hantera.");
        a.find('.del-album').data('id', albums[key].id);
        a.find('.album-img').data('id', albums[key].id);
        a.find('.name-input').data('id', albums[key].id);
        a.find('.tracks').data('id', albums[key].id);
        a.find('.album-img').prop('src', albums[key].image);
        $('#album-list').append(a);
    });
}

function updateAlbumImage(target) {
    var id = target.data('id');
    $.ajax({
        url: "/AlbumEdit/UpdateImage",
        type: "POST",
        data: {id: id, src: target.attr('src')},
        success: function (res) {
            if (!res.success) {
                target.prop('src', '');
            }
        },
        error: function() {
            target.prop('src', '');
        }
    });
}

$(function () {
    if ($('#album-list').length > 0) {
        var currentAlbum = -1;
        $('#add-album').on('click',function (e) {
            e.preventDefault();
            $('#add-album-container').toggle();
        });
        $('#album-list').on('click','.del-album', function(e) {
            e.preventDefault();
            var self = $(this);
            var id = self.data('id');
            $.ajax({
                url: "/AlbumEdit/DeleteAlbum/" + id,
                type: "POST",
                success: function (res) {
                    if (res.success) {
                        delete albums[id];
                        renderAlbums();
                    } 
                }
            });

        });
        function changeName(input) {
            var id = input.data('id');
            var name = input.val();
            $.ajax({
                url: "/AlbumEdit/ChangeName",
                type: "POST",
                data: { id: id, name: name },
                success: function (res) {
                    if (res.success) {
                        albums[id].name = name;
                    }
                }
            });
        }
        $("#album-list").on('focusout', '.name-input', function (e) {
            changeName($(this));
        });
        $("#album-list").on('keypress', '.name-input', function (e) {
            if (e.which === 13) {
                $(this).blur();
            }
        });

        function renderTracks(id) {
            var album = albums[id];
            var tracks = album.tracks;
            if (Object.keys(tracks).length > 0) {
                Object.keys(tracks).forEach(function(el) {
                    console.log(tracks[el]);
                });
            }
        }

        $("#album-list").on('click', '.tracks', function () {
            currentAlbum = $(this).data('id');
            renderTracks(currentAlbum);
            $('#trackmanagmentmodal').modal('show');
        });

        $("#album-list").on("click", ".album-img", function (e) {
            e.preventDefault();
            var target = $(this);
            $("#imagePickerModal").modal("show");
            $("#picker-images").off("click", ".image-box");
            $("#picker-images")
                .on("click",
                    ".image-box",
                    function (e) {
                        $("#imagePickerModal").modal("hide");
                        target.attr("src", $(this).data("path"));
                        updateAlbumImage(target);
                    });
        });


        $('#add-album-form').on('submit', function (e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (res) {
                    if (res.success) {
                        $('#add-album-container').slideUp();
                        albums[res.id] = {};
                        albums[res.id].id = res.id;
                        albums[res.id].tracksCount = 0;
                        albums[res.id].name = form.find('#name').val();
                        renderAlbums();
                        form.trigger("reset");
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function (err) {
                    error.text("Misslyckades med att skapa album sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });
        });

        renderAlbums();

        function uploadTracks(files) {
            console.log(currentAlbum);
            var mediaData = new FormData();
            for(var i=0; i< files.length; i++){
                mediaData.append("TrackFiles", files[0]);
            }
            mediaData.append("AlbumId", currentAlbum);
            $.ajax({
                type: "POST",
                url: "/AlbumEdit/UploadTracks",
                contentType: false,
                processData: false,
                data: mediaData,
                success: function (res) {
                    console.log(res);
                },
                error: function (err) {
                    console.log(err);
                }
            });

        }

        var trackform = $("#trackform");
        if (trackform.length > 0) {
            var input = trackform.find('input[type="file"]'),
                showFiles = function(files) {
                    uploadTracks(files);
                };

            trackform.on("drop",function (e) {
                 showFiles(e.originalEvent.dataTransfer.files);
             });

            trackform.on("change",function (e) {
                    showFiles(e.target.files);
            });

            trackform.on("drag dragstart dragend dragover dragenter dragleave drop",
                function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                })
            .on("dragover dragenter",
                function () {
                    trackform.addClass("is-dragover");
                })
            .on("dragleave dragend drop",
                function () {
                    trackform.removeClass("is-dragover");
                })
            .on("drop",
                function (e) {
                    trackform = e.originalEvent.dataTransfer.files;
                });

        }
    }
});