var timeDay = 24 * 60 * 60 * 1000;
var timeWeek = 7 * timeDay;
var monthNames = [
    "Januari", "Februari", "Mars", "April", "Maj", "Juni", "Juli", "Augusti", "September", "Oktober", "November",
    "December"
];


$(function () {
    var calendarElement = $("#calendar");

    if (calendarElement.length > 0) {
        var cal = new Calendar(calendarElement);
        $('#calendar').on('click', '.prev-month', function(e) {
            e.preventDefault();
            cal.prevMonth();
        });
        $('#calendar').on('click', '.next-month', function (e) {
            e.preventDefault();
            cal.nextMonth();
        });
    }
});

function Calendar(container) {
    this.container = container;
    var today = new Date();
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
    var shownMonth = new Month(this.year, this.month);
    var controls = $('<div class="controls"><a href="" class="prev-month glyphicon glyphicon-chevron-left"></a><span class="date">' +
        monthNames[this.month] + ' ' + this.year +
        '</span><a href="" class="next-month glyphicon glyphicon-chevron-right"></a></div>');
    this.container.empty();
    this.container.append(controls);
    this.container.append(shownMonth.render());
};

function Month(year, month) {
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
        var week = new Week(monday, firstDayOfMonth, lastDayOfMonth);
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



function Week(firstDay, firstDayOfMonth, lastDayOfMonth) {
    this.firstDay = firstDay;
    this.days = [];
    for (var i = 0; i < 7; i++) {
        var date = new Date(this.firstDay.getTime() + timeDay * i);
        var inMonth = date >= firstDayOfMonth && date <= lastDayOfMonth;
        var day = new Day(date, inMonth);
        this.days.push(day);
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

function Day(day, inMonth) {
    this.day = day;
    this.inMonth = inMonth;
    this.dom = $('<td class="day"><span class="date">' + this.day.getDate() + '</span></div>');
    if (!inMonth) this.dom.addClass('outside');
};

Day.prototype.render = function () {
    return this.dom;
};
