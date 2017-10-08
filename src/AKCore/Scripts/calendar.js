var timeDay = 24 * 60 * 60 * 1000;
var timeWeek = 7 * timeDay;
var today = new Date();
var monthNames = [
    "Januari", "Februari", "Mars", "April", "Maj", "Juni", "Juli", "Augusti", "September", "Oktober", "November",
    "December"
];


$(function () {
    var calendarElement = $("#calendar");
    var calendarList = $('#calendar-list');
    if (calendarElement.length > 0) {
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
    this.month--;
    if (this.month < 0) {
        this.month = 11;
        this.year--;
    }
    this.render();
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
        monday = new Date(monday.getTime() + timeWeek);
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
        var date = new Date(this.firstDay.getTime() + timeDay * i);
        var inMonth = date >= firstDayOfMonth && date <= lastDayOfMonth && (today <= date || today.getDate() <= date.getDate());
        if (events) {
            this.days.push(new Day(date, inMonth, events[date.getDate()]));
        } else {
            this.days.push(new Day(date, inMonth, null));
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

function Day(date, inMonth, events) {
    this.date = date;
    this.inMonth = inMonth;
    this.dom = $('<td class="day"><span class="date">' + this.date.getDate() + '</span></div>');
    if (!inMonth) this.dom.addClass('outside');
    if (inMonth && events && events.length > 0) {
        for (var i = 0; i < events.length; i++) {
            var event = events[i];
            this.dom.append($('<a href="#" class="dayEvent">' + event.name + '</a>'));
        }
    }
};

Day.prototype.render = function () {
    return this.dom;
};