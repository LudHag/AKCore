var albumTemplate = $('<div class="row"><div class="col-md-2 image"><img class="album-img" /></div><div class="col-md-4 name"></div><div class="col-md-2 actions"><a href="#" class="del-album">x</a></div><div class="col-md-4 tracks"></div></div>');

function renderAlbums() {
    $('#album-list').empty();
    Object.keys(albums).forEach(function (key) {
        var a = albumTemplate.clone();
        a.find('.name').html(albums[key].name);
        a.find('.del-album').data('id', albums[key].id);
        a.find('.album-img').prop('src', albums[key].image);
        $('#album-list').append(a);
    });

}

$(function () {
    if ($('#album-list').length > 0) {

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

    }
});