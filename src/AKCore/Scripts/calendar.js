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
    this.weeks = [];
    this.create();
    this.render();
};

Calendar.prototype.create = function() {
    var firstDayOfMonth = new Date(this.year, this.month, 1);
    var lastDayOfMonth = new Date(this.year, this.month + 1, 0);
    var firstDayWeekDay = firstDayOfMonth.getDay() - 1;
    if (firstDayWeekDay < 0) firstDayWeekDay = 7 + firstDayWeekDay;
    var firstDayOfCalendar = new Date();
    firstDayOfCalendar.setDate(firstDayOfMonth.getDate() - firstDayWeekDay);
    var monday = new Date(firstDayOfCalendar.getTime());
    while (monday < lastDayOfMonth) {
        var week=new Week(monday);
        this.weeks.push(week);
        monday = new Date(monday.getTime() + timeWeek);
    }
};

Calendar.prototype.render = function () {
    this.weeks.forEach(function (week) {
        console.log(week.render());
    });
};

function Week(firstDay) {
    this.firstDay = firstDay;
    this.days = [];
    for (var i = 0; i < 7; i++) {
        var date = new Date(this.firstDay.getTime() + timeDay*i);
        var day = new Day(date);
        this.days.push(day);
    }
};

Week.prototype.render = function () {
    var result = '';
    this.days.forEach(function (day) {
        result += day.render() + '---';
    });
    return result.substring(0, result.length - 3);
};

function Day(day) {
    this.day = day;
};

Day.prototype.render = function () {
    return this.day.getDate();
};
