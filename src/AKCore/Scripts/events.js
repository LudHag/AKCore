$('#create-new-event')
    .on('click',
        function(e) {
            e.preventDefault();
            $('#edit-event-modal').modal('show');
        });

$('#edit-event-form')
    .on('submit',
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
                        reloadEvents('');
                        $('#edit-event-modal').modal('hide');
                        clearEventModal();
                    } else {
                        error.text(res.message);
                        error.slideDown().delay(4000).slideUp();
                    }
                },
                error: function (err) {
                    error.text("Misslyckades med att spara eventet");
                    error.slideDown().delay(4000).slideUp();
                }
            });
        });
$('#admin-event-list')
    .on('click',
        '.remove-event',
        function(e) {
            e.preventDefault();
            if (window.confirm("Är du säker på att du vill ta bort detta event?")){
                var self = $(this);
                $.ajax({
                    url: self.attr("href"),
                    type: "POST",
                    success: function (res) {
                        if (res.success) {
                            reloadEvents('');
                        } else {
                            console.log(res.message);
                        }
                    },
                    error: function (err) {
                        console.log("FEL!");
                    }
                });
            }
        });


function clearEventModal() {
    $('#edit-event-form')[0].reset();
}

function reloadEvents(searchString) {
    $.get('?SearchString=' + searchString,
        function(data) {
            $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
        });
}