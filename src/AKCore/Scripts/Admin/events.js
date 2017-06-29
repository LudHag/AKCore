$(function() {
    $('#create-new-event')
        .on('click',
            function(e) {
                e.preventDefault();
                $('#edit-event-modal').modal('show');
            });

    $('#edit-event-form')
        .on('submit',
            function(e) {
                e.preventDefault();
                var form = $(this);
                var error = form.find(".alert-danger");
                $.ajax({
                    url: form.attr("action"),
                    type: "POST",
                    data: form.serialize(),
                    success: function(res) {
                        if (res.success) {
                            reloadEvents('', $('#sort-future-select').val());
                            $('#edit-event-modal').modal('hide');
                            clearEventModal();
                        } else {
                            error.text(res.message);
                            error.slideDown().delay(4000).slideUp();
                        }
                    },
                    error: function(err) {
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
                if (window.confirm("Är du säker på att du vill ta bort detta event?")) {
                    var self = $(this);
                    $.ajax({
                        url: self.attr("href"),
                        type: "POST",
                        success: function(res) {
                            if (res.success) {
                                reloadEvents('', $('#sort-future-select').val());
                            } else {
                                console.log(res.message);
                            }
                        },
                        error: function(err) {
                            console.log("FEL!");
                        }
                    });
                }
            });
    function fTime(n) {
        return n < 10 ? '0' + n : n;
    }
    $('#admin-event-list')
        .on('click',
            '.event-row',
            function(e) {
                e.preventDefault();
                if (!$(e.target).hasClass('remove-event')) {
                    $.ajax({
                        url: "/AdminEvent/GetEvent/" + $(this).data('id'),
                        type: "GET",
                        success: function (res) {
                            if (res.success) {
                                var event = JSON.parse(res.e);
                                $('#Id').val(event.Id);
                                $('#Name').val(event.Name);
                                $('#Place').val(event.Place);
                                var date = new Date(event.Day);
                                if (date.getFullYear() > 2000) {
                                    $('#Day').val((date.getMonth()+1) + "/" + date.getDate() + "/" + date.getFullYear());
                                } else {
                                    $('#Day').val('');
                                }
                                var halan = new Date(event.Halan);
                                $('#Halan').val(fTime(halan.getUTCHours()) + ":" + fTime(halan.getMinutes()));
                                var there = new Date(event.There);
                                $('#There').val(fTime(there.getUTCHours()) + ":" + fTime(there.getMinutes()));
                                var starts = new Date(event.Starts);
                                $('#Starts').val(fTime(starts.getUTCHours()) + ":" + fTime(starts.getMinutes()));
                                $('#Type').val(event.Type);
                                $('#Fika').val(event.Fika);
                                if (event.Stand) {
                                    $('#Stand').val('Stå');
                                } else {
                                    $('#Stand').val('Gå');
                                }
                                $('#Description').val(event.Description);
                                $('#InternalDescription').val(event.InternalDescription);
                                replaceEventType(event.Type);
                                $('#edit-event-modal').modal('show');
                            }
                        },
                        error: function(err) {
                            console.log("FEL!");
                        }
                    });

                }
            });

    $('.datepicker').datepicker();
    $('#Day').val('');

    $('#edit-event-modal')
        .on('hidden.bs.modal',
            function() {
                clearEventModal();
            });
    $('#sort-future-select')
        .on('change',
            function() {
                reloadEvents('', $(this).val());
        });

    $('#edit-event-modal').on('change','#Type',function() {
        replaceEventType($(this).val());
    });
    $('#edit-event-modal').on('hidden.bs.modal', function () {
        replaceEventType("");
    });

});
function clearEventModal() {
    $('#edit-event-form')[0].reset();
    $('#Day').val('');
}

function reloadEvents(searchString,sort) {
    $.get('?SearchString=' + searchString + '&Future='+sort,
        function(data) {
            $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
        });
}

function replaceEventType(type) {
    $('#edit-event-modal').removeClass('Kårhusrep');
    $('#edit-event-modal').removeClass('Fikarep');
    $('#edit-event-modal').removeClass('Rep');
    $('#edit-event-modal').removeClass('Fika');
    $('#edit-event-modal').removeClass('Fest');
    $('#edit-event-modal').removeClass('Spelning');
    $('#edit-event-modal').addClass(type);
}