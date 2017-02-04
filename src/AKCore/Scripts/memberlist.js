$(function () {
    var adressList = $('.adress-list');
    if (adressList.length > 0) {
        
        function renderPerson(pers) {
            var kamrer = $('<div class="kamerer">');
            var group1 = $('<div class="name-email">');
            group1.append(pers.name + '<br><a href="mailto:' + pers.email + '">' + pers.email + '</a>');
            var group2 = $('<div class="address">');
            group2.append(pers.adress + '<br>' + pers.zip + ' ' + pers.city);
            var group3 = $('<div class="phone-nation">');
            group3.append(pers.nation + '<br><a href="tel:' + pers.phone + '">' + pers.phone + '</a>');
            kamrer.append(group1);
            kamrer.append(group2);
            kamrer.append(group3);
            return kamrer;
        }
        function renderList() {
            var instruments = Object.keys(memberList);
            adressList.empty();
            instruments.forEach(function(instr) {
                var instrCat = $('<div class="instr-cat">');
                instrCat.append('<h2>' + instr + '</h2>');
                var persons = memberList[instr];
                persons.forEach(function(pers) {
                    instrCat.append(renderPerson(pers));
                });
                adressList.append(instrCat);
            });
        }

        renderList();
        $("#search-member-form")
         .on("submit",
             function (e) {
                 e.preventDefault();
                 console.log('implement me please!!!');
             });
    }
});
