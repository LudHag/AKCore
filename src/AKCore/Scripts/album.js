$(function() {
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
});