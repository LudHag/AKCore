if ($('#album-list').length > 0) {
    const albumTemplate = $('<div class="row"><div class="col-md-2 image">' +
        '<img class="album-img" /></div><div class="col-md-4 name">' +
        '<input type="text" class="name-input"></div><div class="col-md-2 actions">' +
        '<a href="#" class="del-album btn glyphicon glyphicon-remove"></a></div>' +
        '<div class="col-md-4 tracks"><span class="tracks-info"></span></div></div>');

    const tListTemplate = $('<ul class="tracklist"></ul>');

    function createListElement(number, name, id) {
        return $(
            `<li><span data-id="${id}" class="name">${name}</span><a class="rem-track" data-id="${id
            }" href="#">x</a></li>`);
    }

    function renderAlbums() {
        $('#album-list').empty();
        Object.keys(albums).forEach(function(key) {
            const a = albumTemplate.clone();
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
        const id = target.data('id');
        $.ajax({
            url: "/AlbumEdit/UpdateImage",
            type: "POST",
            data: { id: id, src: target.attr('src') },
            success: function(res) {
                if (!res.success) {
                    target.prop('src', '');
                } else {
                    albums[id].image = target.attr('src');
                }
            },
            error: function() {
                target.prop('src', '');
            }
        });
    }

    $(function() {
        let currentAlbum = -1;
        $('#add-album').on('click',
            function(e) {
                e.preventDefault();
                $('#add-album-container').toggle();
            });
        $('#album-list').on('click',
            '.del-album',
            function(e) {
                e.preventDefault();
                var self = $(this);
                var id = self.data('id');
                $.ajax({
                    url: "/AlbumEdit/DeleteAlbum/" + id,
                    type: "POST",
                    success: function(res) {
                        if (res.success) {
                            delete albums[id];
                            renderAlbums();
                        }
                    }
                });

            });

        function changeName(input) {
            const id = input.data('id');
            const name = input.val();
            $.ajax({
                url: "/AlbumEdit/ChangeName",
                type: "POST",
                data: { id: id, name: name },
                success: function(res) {
                    if (res.success) {
                        albums[id].name = name;
                    }
                }
            });
        }

        $("#album-list").on('focusout',
            '.name-input',
            function(e) {
                changeName($(this));
            });
        $("#album-list").on('keypress',
            '.name-input',
            function(e) {
                if (e.which === 13) {
                    $(this).blur();
                }
            });

        function renderTracks(id) {
            const album = albums[id];
            $('#trackmanagmentmodal').find('.modal-title').html(album.name);
            const tracks = album.tracks;
            $('#tracklist').empty();
            const list = tListTemplate.clone();
            if (Object.keys(tracks).length > 0) {
                Object.keys(tracks)
                    .forEach(function(el) {
                        list.append(createListElement(el, tracks[el].name, tracks[el].id));
                    });
                $('#tracklist').append(list);
            } else {
                $('#tracklist').html('Inga spår uppladdade hittills');
            }
            album.tracksCount = Object.keys(tracks).length;
            renderAlbums();
        }

        $("#album-list").on('click',
            '.tracks',
            function() {
                currentAlbum = $(this).data('id');
                renderTracks(currentAlbum);
                $('#trackmanagmentmodal').modal('show');
            });

        $("#album-list").on("click",
            ".album-img",
            function(e) {
                e.preventDefault();
                const target = $(this);
                $("#imagePickerModal").modal("show");
                $("#picker-images").off("click", ".image-box");
                $("#picker-images")
                    .on("click",
                        ".image-box",
                        function(e) {
                            $("#imagePickerModal").modal("hide");
                            target.attr("src", $(this).data("path"));
                            updateAlbumImage(target);
                        });
            });

        $('#tracklist').on('click',
            '.rem-track',
            function(e) {
                e.preventDefault();
                const id = $(this).data('id');
                $.ajax({
                    url: "/AlbumEdit/DeleteTrack",
                    type: "POST",
                    data: { id: id, album: currentAlbum },
                    success: function(res) {
                        if (res.success) {
                            const tracksres = JSON.parse(res.tracks);
                            albums[currentAlbum].tracks = {};
                            Object.keys(tracksres).forEach(function(el) {
                                albums[currentAlbum].tracks[el] = {};
                                albums[currentAlbum].tracks[el].filename = tracksres[el].FileName;
                                let clearName = tracksres[el].Name;
                                if (!clearName) {
                                    clearName = tracksres[el].FileName;
                                }
                                albums[currentAlbum].tracks[el].name = clearName;
                                albums[currentAlbum].tracks[el].id = tracksres[el].Id;
                            });
                            renderTracks(currentAlbum);
                        }
                    }
                });
            });

        $('#add-album-form').on('submit',
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
                            $('#add-album-container').slideUp();
                            albums[res.id] = {};
                            albums[res.id].id = res.id;
                            albums[res.id].tracksCount = 0;
                            albums[res.id].tracks = {};
                            albums[res.id].name = form.find('#name').val();
                            renderAlbums();
                            form.trigger("reset");
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function(err) {
                        error.text("Misslyckades med att skapa album sida");
                        error.slideDown().delay(4000).slideUp();
                    }
                });
            });

        renderAlbums();

        function uploadTracks(files) {
            const mediaData = new FormData();
            for (var i = 0; i < files.length; i++) {
                mediaData.append("TrackFiles", files[i]);
            }
            mediaData.append("AlbumId", currentAlbum);
            $.ajax({
                type: "POST",
                url: "/AlbumEdit/UploadTracks",
                contentType: false,
                processData: false,
                data: mediaData,
                success: function(res) {
                    if (res.success) {
                        const tracksres = JSON.parse(res.tracks);
                        albums[currentAlbum].tracks = {};
                        Object.keys(tracksres).forEach(function(el) {
                            albums[currentAlbum].tracks[el] = {};
                            let clearName = tracksres[el].Name;
                            if (!clearName) {
                                clearName = tracksres[el].FileName;
                            }
                            albums[currentAlbum].tracks[el].name = clearName;
                            albums[currentAlbum].tracks[el].filename = tracksres[el].FileName;
                            albums[currentAlbum].tracks[el].id = tracksres[el].Id;
                        });
                        renderTracks(currentAlbum);
                    }
                },
                error: function(err) {
                    console.log(err);
                }
            });

        }

        var trackform = $("#trackform");
        if (trackform.length > 0) {
            const input = trackform.find('input[type="file"]'),
                showFiles = function(files) {
                    uploadTracks(files);
                };

            trackform.on("drop",
                function(e) {
                    showFiles(e.originalEvent.dataTransfer.files);
                });

            trackform.on("change",
                'input[type="file"]',
                function(e) {
                    showFiles(e.target.files);
                });

            trackform.on("drag dragstart dragend dragover dragenter dragleave drop",
                    function(e) {
                        e.preventDefault();
                        e.stopPropagation();
                    })
                .on("dragover dragenter",
                    function() {
                        trackform.addClass("is-dragover");
                    })
                .on("dragleave dragend drop",
                    function() {
                        trackform.removeClass("is-dragover");
                    })
                .on("drop",
                    function(e) {
                        trackform = e.originalEvent.dataTransfer.files;
                    });


            trackform.on("click",
                ".name",
                function(e) {
                    e.preventDefault();
                    const self = $(this);
                    self.replaceWith('<input type="text" class="name-input" data-id="' +
                        self.data("id") +
                        '" value="' +
                        self.text() +
                        '"\>');
                    $(".name-input").focus();
                });

            function submitTrackName(element) {

                $.ajax({
                    url: "/AlbumEdit/ChangeTrackName",
                    type: "POST",
                    data: { id: element.data("id"), name: element.val() },
                    success: function(res) {
                    }
                });
                element.replaceWith(`<span class="name" data-id="${element.data("id")}">${element.val()}</span>`);

            }

            trackform.on("keypress",
                ".name-input",
                function(e) {
                    if (e.which == 13) {
                        trackform.off("blur", ".name-input");
                        submitTrackName($(this));
                        trackform.on("blur",
                            ".name-input",
                            function(e) {
                                submitTrackName($(this));
                            });
                    }
                });
            trackform.on("blur",
                ".name-input",
                function(e) {
                    submitTrackName($(this));
                });

            trackform.on("submit",
                function(e) {
                    e.preventDefault();
                });
        }
    });
}