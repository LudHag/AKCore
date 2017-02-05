$(function () {
    var adressList = $('.adress-list');
    if (adressList.length > 0) {
        var kamererList = [];
        function renderList() {
            var instruments = Object.keys(memberList);
            adressList.empty();
            instruments.forEach(function(instr) {
                var instrCat = $('<div class="instr-cat" data-instr="' + instr + '">');
                instrCat.append('<h2>' + instr + '</h2>');
                var persons = memberList[instr];
                persons.forEach(function (pers) {
                    var k = new Kamerer(pers);
                    kamererList.push(k);
                    instrCat.append(k.dom);
                });
                adressList.append(instrCat);
            });
        }

        renderList();
        $("#member-search")
         .on("change",
             function (e) {
                 e.preventDefault();
                 var self = $(this);
                 kamererList.forEach(function(pers) {
                     pers.filter(self.val());
                 });
                 $('.instr-cat').each(function() {
                     if ($(this).find('.show').length > 0) {
                         $(this).show();
                     } else {
                         $(this).hide();
                     }
                 });
             });
        $('#select-instrument').on('change', function(e) {
            var val = $(this).val();
            
            $('.instr-cat').each(function () {
                if (val.length < 1) {
                    $(this).show();
                }
                if ($(this).data('instr')===val) {
                    $(this).show();
                } else {
                    $(this).hide();
                }
            });

        });
    }
});

function Kamerer(pers) {
    this.pers = pers;
    this.dom = this.renderPerson();
};

Kamerer.prototype.filter = function (phrase) {
    if (phrase.length < 1) {
        this.show();
        return;
    }
    var lPhrase = phrase.toLowerCase();
    if (this.pers.name.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    } else if (this.pers.instrument.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    }else if (this.pers.phone.toLowerCase().indexOf(lPhrase) > -1) {
        this.show();
        return;
    }else {
        this.hide();
        return;
    }
};

Kamerer.prototype.hide = function () {
    this.dom.addClass('hide');
    this.dom.removeClass('show');
};

Kamerer.prototype.show = function () {
    this.dom.removeClass('hide');
    this.dom.addClass('show');
};

Kamerer.prototype.renderPerson = function () {
    var kamrer = $('<div class="kamerer">');
    var group1 = $('<div class="name-email">');
    group1.append(this.pers.name + '<br><a href="mailto:' + this.pers.email + '">' + this.pers.email + '</a>');
    var group2 = $('<div class="address">');
    group2.append(this.pers.adress + '<br>' + this.pers.zip + ' ' + this.pers.city);
    var group3 = $('<div class="phone-nation">');
    group3.append(this.pers.nation + '<br><a href="tel:' + this.pers.phone + '">' + this.pers.phone + '</a>');
    kamrer.append(group1);
    kamrer.append(group2);
    kamrer.append(group3);
    return kamrer;
};
