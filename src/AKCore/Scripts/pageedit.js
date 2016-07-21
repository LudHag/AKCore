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
