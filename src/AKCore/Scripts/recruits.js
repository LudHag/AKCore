$(function () {
    var recruitDom = $('#recruitList');
    if (recruitDom.length > 0) {
        var list = new RecruitList(recruitListData, recruitDom);
        $('#select-instrument').on('change', function(e) {
            list.filter();
        });
        $('#interest-search').on('keyup', function (e) {
            list.filter();
        });
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
    this.dom.append($('<div class="row recruit-head"><div class="col-md-2">Skapad</div><div class="col-md-2">Namn</div><div class="col-md-2">Instrument</div><div class="col-md-2 contact">Kontaktinformation</div><div class="col-md-3 other">Övrigt</div><div class="col-md-1 actions"></div>'));
    this.render();
};

RecruitList.prototype.filter = function() {
    var self = this;
    var keys = Object.keys(this.recruits);
    keys.forEach(function (rec) {
        self.recruits[rec].filter();
    });
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
    this.isVisible = true;
    this.dom = $('<div class="row recruit hover-grey">');
    this.dom.append($('<div class="col-md-2">' + data.created.getFullYear() + '-' + ('0' + (data.created.getMonth() + 1)).slice(-2) + '-' + ('0' + data.created.getDate()).slice(-2) + '</div>'));
    this.dom.append($('<div class="col-md-2">' + data.fname + ' ' + data.lname + '</div>'));
    this.dom.append($('<div class="col-md-2">' + data.instrument + '</div>'));
    this.dom.append($('<div class="col-md-2 contact">' + data.email + (data.email.length > 1 ? '<br>' : '') + data.phone + '</div>'));
    this.dom.append($('<div class="col-md-3 other">' + data.other + '</div>'));
    this.dom.append($('<div class="col-md-1 actions"><input type="checkbox" /></div>'));
};

Recruit.prototype.render = function() {
    return this.dom;
};

Recruit.prototype.filter = function () {
    var instr = $('#select-instrument').val();
    if (instr.length > 0) {
        if (instr !== this.data.instrument) {
            this.hide();
            return;
        }
    }
    var search = $('#interest-search').val();
    if (search.length > 0) {
        if ((this.data.fname + ' ' + this.data.lname).indexOf(search)<0) {
            this.hide();
            return;
        }
    }
    this.show();
};

Recruit.prototype.hide = function () {
    this.isVisible = false;
    this.dom.addClass('hide');
};

Recruit.prototype.show = function() {
    this.isVisible = true;
    this.dom.removeClass('hide');
};