$(function () {
    var hireDom = $('#hireList');
    if (hireDom.length > 0) {
        var list = new HireList(hireListData, hireDom);
        $('#hire-search').on('keyup', function (e) {
            list.filter();
        });
        $('#archived-flip').on('change', function (e) {
            list.filter();
        });
        hireDom.on('click', '.archive', function (e) {
            e.preventDefault();
            var id = $(this).data('id');
            var arch = !($(this).hasClass('green'));
            $.ajax({
                url: "/Hire/Archive",
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
        hireDom.on('click', '.remove',function(e) {
            e.preventDefault();
            var id = $(this).data('id');
            if (window.confirm("Är du säker att du vill ta bort den här spelningsförfrågan?")) {
                $.ajax({
                    url: "/Hire/Remove",
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

function HireList(data, dom) {
    var self = this;
    this.dom = dom;
    this.data = data;
    this.hires = {};
    var hireKeys = Object.keys(data);
    hireKeys.forEach(function (id) {
        self.hires[id]=new Hire(data[id]);
    });
    this.dom.append($('<div class="row recruit-head"><div class="col-md-2">Skapad</div><div class="col-md-2">Namn</div><div class="col-md-3 col-lg-2 contact">Kontaktinformation</div><div class="col-md-2 col-lg-3 other">Övrigt</div><div class="col-md-1 actions"></div>'));
    this.render();
};

HireList.prototype.archive = function (id, arch) {
    this.hires[id].archive(arch);
};

HireList.prototype.export = function () {
    var self = this;
    var field = $('#export-field');
    field.empty();
    var keys = Object.keys(this.hires);
    keys.forEach(function (key) {
        var rec = self.hires[key];
        if (rec.isVisible) {
            field.append(rec.export()+'\n');
        }
    });
};

HireList.prototype.remove = function (id) {
    this.hires[id].hide();
    delete this.hires[id];
};

HireList.prototype.filter = function() {
    var self = this;
    var keys = Object.keys(this.hires);
    keys.forEach(function (rec) {
        self.hires[rec].filter();
    });
};

HireList.prototype.render = function () {
    var self = this;
    var keys = Object.keys(this.hires);
    keys.forEach(function(rec) {
        self.dom.append(self.hires[rec].render());
    });
};

function Hire(data) {
    this.data = data;
    this.isVisible = true;
    this.dom = $('<div class="row recruit hover-grey">');
    this.dom.append($('<div class="col-md-2">' + data.created.getFullYear() + '-' + ('0' + (data.created.getMonth() + 1)).slice(-2) + '-' + ('0' + data.created.getDate()).slice(-2) + '</div>'));
    this.dom.append($('<div class="col-md-2">' + data.name + '</div>'));
    this.dom.append($('<div class="col-md-3 col-lg-2 contact">' + data.email + (data.email.length > 1 ? '<br>' : '') + data.phone + '</div>'));
    this.dom.append($('<div class="col-md-2 col-lg-3 other">' + data.other + '</div>'));
    this.actions = $('<div class="col-md-1 actions"></div>');
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

Hire.prototype.archive = function (arch) {
    this.data.archived = arch;
    if (arch) {
        this.dom.find('.archive').addClass('green');
    } else {
        this.dom.find('.archive').removeClass('green');
    }
    this.filter();
};

Hire.prototype.export = function () {
    var data = this.data;
    return data.name + '\t' + data.email + '\t' + data.phone + '\t' + data.other;
};

Hire.prototype.render = function() {
    return this.dom;
};

Hire.prototype.filter = function () {
    if (!!($('#archived-flip').is(':checked') ^ this.data.archived)) {
        this.hide();
        return;
    }

    var search = $('#hire-search').val().toLowerCase();
    if (search.length > 0) {
        if ((this.data.name.toLowerCase()).indexOf(search) < 0) {
            this.hide();
            return;
        }
    }
    this.show();
};

Hire.prototype.hide = function () {
    this.isVisible = false;
    this.dom.addClass('hide');
};

Hire.prototype.show = function() {
    this.isVisible = true;
    this.dom.removeClass('hide');
};