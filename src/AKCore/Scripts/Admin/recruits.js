$(function () {
    const recruitDom = $('#recruitList');
    if (recruitDom.length > 0) {
        const list = new RecruitList(recruitListData, recruitDom);
        $('#select-instrument').on('change', function(e) {
            list.filter();
        });
        $('#interest-search').on('keyup', function (e) {
            list.filter();
        });
        $('#archived-flip').on('change', function (e) {
            list.filter();
        });
        $('#recruitList').on('click', '.archive', function (e) {
            e.preventDefault();
            const id = $(this).data('id');
            const arch = !($(this).hasClass('green'));
            $.ajax({
                url: "/Signup/Archive",
                type: "POST",
                data: {id:id, arch:arch},
                success: function (res) {
                    if (res.success) {
                        list.archive(id, arch);
                    }
                },
                error: function (err) {
                    console.log(err);
                }
            });

        });
        $('#recruitList').on('click', '.remove',function(e) {
            e.preventDefault();
            const id = $(this).data('id');
            if (window.confirm("Är du säker att du vill ta bort den här intresseanmälan?")) {
                $.ajax({
                    url: "/Signup/Remove",
                    type: "POST",
                    data: { id: id},
                    success: function (res) {
                        if (res.success) {
                            list.remove(id);
                        }
                    },
                    error: function (err) {
                        console.log(err);
                    }
                });
            }
        });
        $('#export-interested').on('click', function (e) {
            e.preventDefault();
            $('#export-modal').modal('show');
            list.export();
        });
    }
});

function RecruitList(data, dom) {
    const self = this;
    this.dom = dom;
    this.data = data;
    this.recruits = {};
    const recruitKeys = Object.keys(data);
    recruitKeys.forEach(function (id) {
        self.recruits[id]=new Recruit(data[id]);
    });
    this.dom.append($('<div class="row recruit-head"><div class="col-sm-2">Skapad</div><div class="col-sm-3">Namn</div><div class="col-sm-2">Instrument</div><div class="col-sm-4 contact">Kontaktinformation</div><div class="col-sm-1 actions"></div>'));
    this.render();
};

RecruitList.prototype.archive = function (id, arch) {
    this.recruits[id].archive(arch);
};

RecruitList.prototype.export = function () {
    const self = this;
    const field = $('#export-field');
    field.empty();
    const keys = Object.keys(this.recruits);
    keys.forEach(function (key) {
        const rec = self.recruits[key];
        if (rec.isVisible) {
            field.append(rec.export()+'\n');
        }
    });
};

RecruitList.prototype.remove = function (id) {
    this.recruits[id].hide();
    delete this.recruits[id];
};

RecruitList.prototype.filter = function() {
    const self = this;
    const keys = Object.keys(this.recruits);
    keys.forEach(function (rec) {
        self.recruits[rec].filter();
    });
};

RecruitList.prototype.render = function () {
    const self = this;
    const keys = Object.keys(this.recruits);
    keys.forEach(function(rec) {
        self.dom.append(self.recruits[rec].render());
    });
};

function Recruit(data) {
    this.data = data;
    this.isVisible = true;
    this.dom = $('<div class="row recruit hover-grey">');
    this.dom.append($('<div class="col-sm-2">' + data.created.getFullYear() + '-' + ('0' + (data.created.getMonth() + 1)).slice(-2) + '-' + ('0' + data.created.getDate()).slice(-2) + '</div>'));
    this.dom.append($('<div class="col-sm-3">' + data.fname + ' ' + data.lname + '</div>'));
    this.dom.append($('<div class="col-sm-2">' + data.instrument + '</div>'));
    this.dom.append($('<div class="col-sm-4 contact">' + data.email + (data.email.length > 1 ? '<br>' : '') + data.phone + '</div>'));
    this.actions = $('<div class="col-sm-1 actions"></div>');
    this.actions.append($('<a href="#" title="Arkivera" data-id="' +
        data.id +
        '" class="archive glyphicon glyphicon-folder-open"></a>'));
    this.actions.append($('<a href="#" title="Ta bort" data-id="' +
        data.id +
        '" class="remove glyphicon glyphicon-remove"></a>'));
    this.dom.append(this.actions);
    if (data.archived) {
        this.archive(true);
    }
};

Recruit.prototype.archive = function (arch) {
    this.data.archived = arch;
    if (arch) {
        const el = this.dom.find('.archive');
        el.addClass('green');
        el.attr('title', 'Aktivera');
    } else {
        const el = this.dom.find('.archive');
        el.removeClass('green');
        el.attr('title', 'Arkivera');
    }
    this.filter();
};

Recruit.prototype.export = function () {
    const data = this.data;
    return `${data.fname}\t${data.lname}\t${data.instrument}\t${data.email}\t${data.phone}`;
};

Recruit.prototype.render = function() {
    return this.dom;
};

Recruit.prototype.filter = function () {
    const instr = $('#select-instrument').val();
    if (!!($('#archived-flip').is(':checked') ^ this.data.archived)) {
        this.hide();
        return;
    }
    if (instr.length > 0) {
        if (instr !== this.data.instrument) {
            this.hide();
            return;
        }
    }
    const search = $('#interest-search').val().toLowerCase();
    if (search.length > 0) {
        if ((this.data.fname.toLowerCase() + ' ' + this.data.lname.toLowerCase()).indexOf(search) < 0) {
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