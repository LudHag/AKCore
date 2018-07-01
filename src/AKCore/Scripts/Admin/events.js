import TinyDatePicker from 'tiny-date-picker';
$(function() {
    if($(".datepicker").length>0){
       TinyDatePicker('.datepicker');
    }

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
                const form = $(this);
                const error = form.find(".alert-danger");
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
                    const self = $(this);
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
                                $('#Secret').prop('checked', event.Secret);
                                $('#Place').val(event.Place);
                                const date = new Date(event.Day);
                                if (date.getFullYear() > 2000) {
                                    $('#Day').val((date.getMonth()+1) + "/" + date.getDate() + "/" + date.getFullYear());
                                } else {
                                    $('#Day').val('');
                                }
                                $('#Halan').val(event.HalanTime);
                                $('#There').val(event.ThereTime);
                                $('#Starts').val(event.StartsTime);
                                $('#Type').val(event.Type);
                                $('#Fika').val(event.Fika);
                                $('#Stand').val(event.Stand);
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

    $("#admin-event-list")
        .on("click",
            ".pagination li",
            function (e) {
                e.preventDefault();
                const self = $(this);
                if (!self.hasClass("active")) {
                    $("#admin-event-list .active").removeClass("active");
                    self.addClass("active");
                    reloadEventPage(self.data("page"));
                }
            });

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
    $('#Id').val(0);
    $('#Day').val('');
}

function reloadEvents(searchString,sort) {
    $.get('?SearchString=' + searchString + '&Future='+sort,
        function(data) {
            $('#admin-event-list').empty().append($(data).find('#admin-event-list').children());
        });
}

function reloadEventPage(page) {
    $.get('?Future=' + $('#sort-future-select').val() + '&page=' + page,
        function (data) {
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