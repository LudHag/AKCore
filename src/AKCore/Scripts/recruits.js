$(function () {
    var recruitDom = $('#recruitList');
    if (recruitDom.length > 0) {
        var list = new RecruitList(recruitListData, recruitDom);

        
    }
});

function RecruitList(data, dom) {
    var self = this;
    this.dom = dom;
    this.data = data;
    this.recruits = {};
    var recruitKeys = Object.keys(data);
    recruitKeys.forEach(function (id) {
        self.recruits[id]=new Recruit(data[id]);
    });
    this.render();
};

RecruitList.prototype.render = function () {
    var self = this;
    var keys = Object.keys(this.recruits);
    keys.forEach(function(rec) {
        self.dom.append(self.recruits[rec].render());
    });
};

function Recruit(data) {
    this.data = data;
    this.dom = $('<div>' + this.data.email + '</div>');
};

Recruit.prototype.render = function() {
    return this.dom;
};