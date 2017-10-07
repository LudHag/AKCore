var timeDay = 24 * 60 * 60 * 1000;
var timeWeek = 7 * timeDay;
$(function () {
    var calendarElement = $("#calendar");

    if (calendarElement.length > 0) {
        var cal = new Calendar(calendarElement);
    }
});

function Calendar(container) {
    this.container = container;
    var today = new Date();
    this.month = today.getMonth();
    this.year = today.getFullYear();
    this.day = today.getDay();
    this.shownMonth = new Month(this.year, this.month);
    this.render();
};


Calendar.prototype.render = function () {
    this.container.append(this.shownMonth.render());
};

function Month(year, month) {
    this.weeks = [];
    var firstDayOfMonth = new Date(year, month, 1);
    var lastDayOfMonth = new Date(year, month + 1, 0);
    var firstDayWeekDay = firstDayOfMonth.getDay() - 1;
    if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
    var firstDayOfCalendar = new Date();
    firstDayOfCalendar.setDate(firstDayOfMonth.getDate() - firstDayWeekDay);
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
    var tableHeading =
        $('<thead> <tr><th>Måndag</th> <th>Tisdag</th> <th>Onsdag</th> <th>Torsdag</th> <th>Fredag</th> <th>Lördag</th> <th>Söndag</th> </tr> </thead>');
    self.dom.append(tableHeading);
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
    this.dom = $('<td class="day">' + this.day.getDate() + '</div>');
    if (!inMonth) this.dom.addClass('outside');
};

Day.prototype.render = function () {
    return this.dom;
};
