<template>
    <div>
        <div id="signup-list">
            <p class="can-come">Kommer: {{coming.length}} - Kommer inte: {{notComing.length}}</p>
            <h2>Kommer</h2>
           
            <div class="row signup-row" v-for="signup in coming">
                <div class="col-sm-2 signup-element">
                    <p>{{signup.personName}}</p>
                </div>
                <div class="col-sm-2 signup-element">
                    <p>{{signup.instrumentName}}{{otherInstrumentsList(signup)}}</p>
                </div>
                <div class="col-sm-2 signup-element">
                    <p>{{getInfo(signup)}}</p>
                </div>
                <div class="col-sm-3 signup-element" v-if="nintendo">
                    <p>{{signup.comment}}</p>
                </div>
                <div class="col-sm-3 signup-element" v-if="nintendo">
                    <p>{{formatDate(signup.signupTime)}}</p>
                </div>
                <div class="col-sm-6 signup-element" v-if="!nintendo">
                    <p>{{signup.comment}}</p>
                </div>
            </div>
            <h2>Kommer inte</h2>
            <div class="row signup-row" v-for="signup in notComing">
                <div class="col-sm-3 signup-element">
                    <p>{{signup.personName}}</p>
                </div>
                <div class="col-sm-3 signup-element">
                    <p>{{signup.instrumentName}}</p>
                </div>
                <div class="col-sm-3 signup-element" v-if="nintendo">
                    <p>{{signup.comment}}</p>
                </div>
                <div class="col-sm-3 signup-element" v-if="nintendo">
                    <p>{{formatDate(signup.signupTime)}}</p>
                </div>
                <div class="col-sm-6 signup-element" v-if="!nintendo">
                    <p>{{signup.comment}}</p>
                </div>
            </div>
        </div>
    </div>   
</template>
<script>
    export default {
        props: ['signups', 'nintendo'],
        computed: {
            coming() {
                if (!this.signups) {
                    return [];
                }
                return this.signups.filter((signup) => {
                    return signup.where !== "Kan inte komma";
                });
            },
            notComing() {
                if (!this.signups) {
                    return [];
                }
                return this.signups.filter((signup) => {
                    return signup.where === "Kan inte komma";
                });
            },
        },
        methods: {
            formatDate(dateString) {
                const date = new Date(dateString);
                const dateStr = this.frmtNum(date.getMonth() + 1) + "-" + this.frmtNum(date.getDate());
                const strTime = this.frmtNum(date.getHours()) + ":" + this.frmtNum(date.getMinutes());
                return dateStr + " " + strTime;
            },
            frmtNum(num) {
                return ("0" + num).slice(-2);
            },
            otherInstrumentsList(signup) {
                if (signup.otherInstruments) {
                    return ", " + signup.otherInstruments.replace(",", ", ");
                }
                return "";
            },
            getInfo(signup) {
                let info = signup.where;
                if (signup.instrument) {
                    info += ", har instrument";
                }
                if (signup.car) {
                    info += ", har bil";
                }
                return info;
            }
        }
    }
</script>
<style lang="scss">
  
</style>