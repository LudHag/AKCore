$(function () {
    if ($('#album-list').length > 0) {
        var albumTemplate = $('<div class="row"><div class="col-md-2 image"><img class="album-img" /></div><div class="col-md-4 name"></div><div class="col-md-2 actions"><a href="#">x</a></div><div class="col-md-4 tracks"></div></div>');

        $('#add-album').on('click',function (e) {
            e.preventDefault();
            $('#add-album-container').toggle();
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
                        updateAlbumList();
                        $('#add-album-container').slideUp();
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

        function updateAlbumList() {
            $.ajax({
                url: "/AlbumEdit/GetAlbums",
                type: "GET",
                success: function (res) {
                    console.log(res);
                },
                error: function (err) {
                    console.log("Failed to update updateAlbumList list");
                }
            });
        }
        function renderAlbums() {
            Object.keys(albums).forEach(function (key) {
                var a = albumTemplate.clone();
                a.find('.name').text(albums[key].name);
                $('#album-list').append(a);
            });
            
        }

        renderAlbums();

    }
});