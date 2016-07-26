$("#new-page-button")
    .on("click",
        function (e) {
            e.preventDefault();
            $('#new-page-form').toggle();

        });

$('#new-page-form')
    .on('submit', 'form',
        function (e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (res) {
                    if (res.success) {
                        window.location.reload();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function (err) {
                    error.text("Misslyckades med att skapa sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });


        });

$('#page-edit')
    .on('submit',
        function (e) {
            e.preventDefault();
            var form = $(this);
            var error = form.find(".alert-danger");
            var success = form.find(".alert-success");
            $.ajax({
                url: form.attr("action"),
                type: "POST",
                data: form.serialize(),
                success: function (res) {
                    if (res.success) {
                        success.text(res.message);
                        success.slideDown().delay(4000).slideUp();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function (err) {
                    error.text("Misslyckades med att spara sida");
                    error.slideDown().delay(4000).slideUp();
                }
            });
        });

$('#start-page')
    .on('click',
        function () {
            var slug = $('#slug');
            if (slug.prop('readonly')) {
                slug.prop('readonly', false);
                slug.val(slug.data('oldslug'));
            } else {
                slug.data('oldslug', slug.val());
                slug.val('/');
                slug.prop('readonly', true);
            }

        });

$('.widget-choose')
    .on('click',
        'a',
        function(e) {
            e.preventDefault();
            console.log('do something');
        });
if($(".mce-content").length>0){
    var options = {
        selector: ".mce-content",
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
        file_browser_callback: function (field_name, url, type, win) {
            if (type === "image") {
                console.log('hej');
            }
        }
    };
    tinymce.init(options);

}