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

function clearEventModal() {
    $('#edit-event-form')[0].reset();
}

function reloadEvents(searchString) {
    $.get('?SearchString=' + searchString,
        function(data) {
            $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
        });
}