var timeDay = 24 * 60 * 60 * 1000;
var today = new Date();
var calendarModal = $('#event-info-modal');

var monthNames = [
    "Januari", "Februari", "Mars", "April", "Maj", "Juni", "Juli", "Augusti", "September", "Oktober", "November",
    "December"
];

Date.prototype.addDays = function (days) {
    var dat = new Date(this.valueOf());
    dat.setDate(dat.getDate() + days);
    return dat;
}

$(function () {
    var calendarElement = $("#calendar");
    var calendarList = $('#calendar-list');
    if (calendarElement.length > 0) {

        var modalTitle = calendarModal.find('.modal-title');
        var modalDay = calendarModal.find('.modal-day');
        var modalPlace = calendarModal.find('.modal-place');
        var modalHalan = calendarModal.find('.modal-halan');
        var modalThere = calendarModal.find('.modal-there');
        var modalStart = calendarModal.find('.modal-start');
        var modalSignup = calendarModal.find('.modal-signup');
        var modalComming = calendarModal.find('.modal-comming');
        var modalStand = calendarModal.find('.modal-stand');
        var modalFika = calendarModal.find('.modal-fika'); 
        var modalDesc = calendarModal.find('.modal-description');
        var modalIntDesc = calendarModal.find('.modal-intdescription');
        
        function Calendar(container, events) {
            this.events = events;
            this.container = container;
            this.month = today.getMonth();
            this.year = today.getFullYear();
            this.day = today.getDay();
            this.render();
        };

        Calendar.prototype.nextMonth = function () {
            this.month++;
            if (this.month > 11) {
                this.month = 0;
                this.year++;
            }
            this.render();
        };

        Calendar.prototype.prevMonth = function () {
            if (this.year > today.getFullYear() || this.month - 1 >= today.getMonth()) {
                this.month--;
                if (this.month < 0) {
                    this.month = 11;
                    this.year--;
                }
                this.render();
            }
        };

        Calendar.prototype.render = function () {
            var monthEvents;
            if (this.events[this.year] != null) {
                monthEvents = this.events[this.year][monthNames[this.month].toLowerCase()];
            }
            var shownMonth = new Month(this.year, this.month, monthEvents);
            var controls = $('<div class="controls"><a href="" class="prev-month glyphicon glyphicon-chevron-left"></a><span class="date">' +
                monthNames[this.month] + ' ' + this.year +
                '</span><a href="" class="next-month glyphicon glyphicon-chevron-right"></a></div>');
            this.container.empty();
            this.container.append(controls);
            this.container.append(shownMonth.render());
        };

        function Month(year, month, events) {
            this.weeks = [];
            var firstDayOfMonth = new Date(year, month, 1);
            var lastDayOfMonth = new Date(year, month + 1, 0);
            var firstDayWeekDay = firstDayOfMonth.getDay() - 1;
            if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
            var firstDayOfCalendar = new Date(firstDayOfMonth.getTime() - (firstDayWeekDay * timeDay));
            this.tableHeading =
                $('<thead> <tr><th>Måndag</th> <th>Tisdag</th> <th>Onsdag</th> <th>Torsdag</th> <th>Fredag</th> <th>Lördag</th> <th>Söndag</th> </tr> </thead>');
            var monday = new Date(firstDayOfCalendar.getTime());
            while (monday < lastDayOfMonth) {
                var week = new Week(monday, firstDayOfMonth, lastDayOfMonth, events);
                this.weeks.push(week);
                monday = monday.addDays(7);
            }
            this.dom = $('<table class="month table table-bordered"></div>');
        };

        Month.prototype.render = function () {
            var self = this;
            self.dom.empty();
            self.dom.append(this.tableHeading);
            var tableBody = $('<tbody></tbody>');
            this.weeks.forEach(function (week) {
                tableBody.append(week.render());
            });
            self.dom.append(tableBody);
            return this.dom;
        };



        function Week(firstDay, firstDayOfMonth, lastDayOfMonth, events) {
            this.firstDay = firstDay;
            this.days = [];
            for (var i = 0; i < 7; i++) {
                var date = this.firstDay.addDays(i);
                var shown = date >= firstDayOfMonth && date <= lastDayOfMonth && (today <= date || today.getDate() <= date.getDate());
                if (events) {
                    this.days.push(new Day(date, shown, events[date.getDate()]));
                } else {
                    this.days.push(new Day(date, shown, null));
                }
            }
            this.dom = $('<tr class="week"></div>');
        };

        Week.prototype.render = function () {
            var self = this;
            this.days.forEach(function (day) {
                self.dom.append(day.render());
            });
            return this.dom;
        };

        function Day(date, shown, events) {
            this.date = date;
            this.shown = shown;
            this.dom = $('<td class="day"><span class="date">' + this.date.getDate() + '</span></div>');
            if (!shown) this.dom.addClass('outside');
            if (shown && events && events.length > 0) {
                for (var i = 0; i < events.length; i++) {
                    var event = events[i];
                    var eventDom = $('<a href="#" class="dayEvent">' + event.halan + ' ' + event.name + '</a>');
                    this.dom.append(eventDom);
                    eventDom.on('click', function (e) {
                        e.preventDefault();
                        modalTitle.html(event.name);
                        modalDay.html(event.day);
                        modalPlace.html(event.place);
                        modalHalan.html('Samling i hålan: ' + event.halan);
                        if (event.there !== '00:00') {
                            modalThere.html('Samling på plats: ' + event.there);
                        } else {
                            modalThere.empty();
                        }
                        if (event.start !== '00:00') {
                            modalStart.html('Spelning startar: ' + event.start);
                        } else {
                            modalStart.empty();
                        }
                        if (event.type==='Spelning') {
                            modalSignup.attr("href", '/upcoming/Event/' + event.id);
                            modalSignup.html('Anmäl');
                            modalStand.html('Speltyp: ' + event.stand);
                            modalComming.html(event.cancome + ' Kommer - ' + event.cantcome + ' Kommer inte');
                            modalFika.empty();
                        } else {
                            modalSignup.empty();
                            modalStand.empty();
                            modalComming.empty();
                            modalFika.html('Fika: ' + event.fika);
                        }
                        if (event.desc) {
                            modalDesc.html(event.desc);
                        } else {
                            modalDesc.empty();
                        }
                        if (event.intdesc) {
                            modalIntDesc.html(event.intdesc);
                        } else {
                            modalIntDesc.empty();
                        }
                        calendarModal.modal('show');
                    });
                }
            }
        };

        Day.prototype.render = function () {
            return this.dom;
        };

        var cal = new Calendar(calendarElement, calendarEvents);
        $('#calendar').on('click', '.prev-month', function(e) {
            e.preventDefault();
            cal.prevMonth();
        });
        $('#calendar').on('click', '.next-month', function (e) {
            e.preventDefault();
            cal.nextMonth();
        });
        $('.calendar-toggle').on('click', function(e) {
            e.preventDefault();
            $('.calendar-toggle').removeClass('active');
            $(this).addClass('active');
            if ($(this).hasClass('event')) {
                calendarElement.addClass('hide');
                calendarList.removeClass('hide');
            } else {
                calendarElement.removeClass('hide');
                calendarList.addClass('hide');
            }
        });
    }
});

