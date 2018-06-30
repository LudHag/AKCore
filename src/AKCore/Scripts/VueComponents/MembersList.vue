<template>
    <div class="row" id="search-widget">
        <div class="col-md-12">
            <div class="form-inline ak-search">
                <div class="form-group">
                    <input type="text" v-model="searchPhrase" class="form-control" placeholder="Sök här">
                </div>
                <div class="form-group">
                    <select class="form-control" v-model="selectedInstrument">
                        <option value="">Sök efter instrument</option>
                        <option v-for="i in instruments">{{i}}</option>
                    </select>
                </div>
            </div>
            <div id="adress-register">
                <h1>Adressregister</h1>
                <div class="adress-list">
                    <div v-for="instr in instruments">
                        <h2 v-html="instr" v-if="filteredMembers[instr] && filteredMembers[instr].length > 0"></h2>
                        <div class="kamerer" v-for="member in filteredMembers[instr]">
                            <div class="name-email">
                                {{member.name}}<br />
                                               <a :href="'mailto:' + member.email">{{member.email}}</a>
                            </div>
                            <div class="phone-nation">
                                <a :href="'tel:' + member.phone">{{member.phone}}</a>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>
<script>

    export default {
        props: ['members', 'instruments'],
        data: function () {
            return {
                selectedInstrument: "",
                searchPhrase: ""
            }
        },
        methods: {
            instrumentMembers: function (instr) {
                return this.members[instr];
            }
        },
        computed: {
            filteredMembers: function () {
                if (this.selectedInstrument === "" && this.searchPhrase === "") {
                    return this.members;
                }
                const filteredList = {};
                const lsearch = this.searchPhrase.toLowerCase();
                this.instruments.forEach((instr) => {
                    if (!this.members[instr] ||
                        (this.selectedInstrument !== "" &&
                        this.selectedInstrument !== instr)) {
                        filteredList[instr] = [];
                    }
                    else if (lsearch === "") {
                        filteredList[instr] = this.members[instr];
                    }
                    else {
                        filteredList[instr] = this.members[instr].filter((el) => {
                            return el.name.toLowerCase().indexOf(lsearch) >= 0 ||
                                el.email.toLowerCase().indexOf(lsearch) >= 0 ||
                                el.phone.indexOf(lsearch) >= 0 ||
                                el.instrument.toLowerCase().indexOf(lsearch) >= 0;
                        });
                    }
                    
                });
                return filteredList;
            }
        }
    }
</script>
<style lang="scss">

</style>
