<template id="current-month-timesheet-template">
<div style="text-align:center">
    <h2>View current timesheet</h2>
    <div><h3>Manage Timesheet for {{timeSheetMonth}} / {{timeSheetYear}}</h3></div>
     
  

  <v-flex v-if="progressState.show==true" xs12 md12>
    <div style="text-align:center" class="elevation-5">
    <v-progress-circular v-bind:size="70" v-bind:width="7" class="grey--text"
    v-bind:indeterminate="progressState.query"
    v-bind:query="progressState.query"
    v-model="progressState.value"
    v-bind:active="progressState.show"
    ></v-progress-circular>
    </div>
    </v-flex>
 
  <v-layout class="text-xs-center">  
    <v-flex v-if="progressState.show==false" xs12 md12>
      <v-layout row wrap> 
      <v-card is="timesheetdetail-item" vertical raised class="elevation-5"
      
      v-for="(oneTimeSheetDetail, index) in timeSheetDetailList"
      v-bind:key="oneTimeSheetDetail.timeSheetDetailId"
      v-bind:timeSheetDetailId = "oneTimeSheetDetail.timeSheetDetailId"
      v-bind:customerAccountName="oneTimeSheetDetail.customerAccountName"
      v-bind:dateOfLesson="oneTimeSheetDetail.dateOfLesson"
      v-bind:officialTimeInHHMM="oneTimeSheetDetail.officialTimeInHHMM"
      v-bind:officialTimeOutHHMM="oneTimeSheetDetail.officialTimeOutHHMM"
      v-bind:signedStatus= "oneTimeSheetDetail.signedStatus"
      v-bind:actualTimeIn= "oneTimeSheetDetail.actualTimeIn"
      v-bind:actualTimeOut= "oneTimeSheetDetail.actualTimeOut"
      v-bind:isReplacement="oneTimeSheetDetail.isReplacement"
      v-bind:updatedAt="oneTimeSheetDetail.updatedAt"
      v-on:remove="timeSheetDetailList.splice(index, 1)"
      @interface="handleUpdateTimeSheetDataDetail" >
   </v-card>
   </v-layout>
</v-flex>
</v-layout>



</div>
</template>

<script>
import moment from 'moment'
import Vue from 'vue'
//https://stackoverflow.com/questions/38367038/format-relative-time-in-momentjs
//The following code section moment.updateLocale(..) customizes how moment formats relative time
//The Vue component(s) are able reference it.
moment.updateLocale('en', {
    relativeTime : {
        future: "in %s",
        past:   "%s ago",
        s: function (number, withoutSuffix, key, isFuture){
            return '00:' + (number<10 ? '0':'') + number + ' minutes';
        },
        m:  "01:00 minutes",
        mm: function (number, withoutSuffix, key, isFuture){
            return (number<10 ? '0':'') + number + ':00' + ' minutes';
        },
        h:  "an hour",
        hh: "%d hours",
        d:  "a day",
        dd: "%d days",
        M:  "a month",
        MM: "%d months",
        y:  "a year",
        yy: "%d years"
    }
});

function findTimeSheetDetailData(inId){
 var  selectedOneTimeSheetDetail = this.timeSheetDetailList.find(d => d.timeSheetDetailId === inId);
 return selectedOneTimeSheetDetail;
}

export default {
  name: 'current-month-timesheet',
  data: function() {
    return{
    progressState:{  
        value: 0,
        query: false,
        show: true,
        interval: {}
    },    
    currentView: 'current-month-timesheet',
    timeSheetMonth: getCurrentMonthName(),
    timeSheetYear: new Date().getFullYear(),
    timeSheetDetailList: [],
    timeSheet : {},
    sessionSynopsisList : [],
    moment:moment
    }
  },
        mounted: function () {
            this.queryAndIndeterminate();
            this.loadTimeSheetDetailData();
        },
       
        methods: {
        queryAndIndeterminate () {
          this.progressState.query = true
          this.progressState.show = true
          this.progressState.value = 0
          let int

        this.progressState.query = false

        this.progressState.interval = setInterval(() => {
          if (this.progressState.value === 100) {
            clearInterval(this.progressState.interval)
            this.progressState.show = false
            //To avoid infinite loop
            return; //Get rid this one which exist in the tutorial: setTimeout(this.queryAndIndeterminate, 2000)
          }
          this.progressState.value += 25
        }, 1000)
       },
       
       beforeDestroy () {
         console.log(this.progressState.interval)
          clearInterval(this.progressState.interval)
       },
       handleUpdateTimeSheetDataDetail (event) {
             console.log('The retrieved data after child handle is : ', event.id) // get the data after child dealing
            var selectedTimeSheetDetailId = event.id;
           
  const selectedOneTimeSheetDetail = this.timeSheetDetailList.find(d => d.timeSheetDetailId === selectedTimeSheetDetailId)
            console.dir(selectedOneTimeSheetDetail);
            //Had router not defined problem.
            //https://stackoverflow.com/questions/41860578/vue-route-is-not-defined
            console.dir({ timeSheetDetailId: selectedOneTimeSheetDetail.timeSheetDetailId,
 dateOfLesson : selectedOneTimeSheetDetail.dateOfLesson,
 actualTimeIn : selectedOneTimeSheetDetail.actualTimeIn,
 actualTimeOut : selectedOneTimeSheetDetail.actualTimeOut,
 officialTimeInHHMM : selectedOneTimeSheetDetail.officialTimeInHHMM,
 officialTimeOutHHMM : selectedOneTimeSheetDetail.officialTimeOutHHMM,
 signedStatus : selectedOneTimeSheetDetail.signedStatus,
 comments : selectedOneTimeSheetDetail.comments,
 customerAccountName : selectedOneTimeSheetDetail.customerAccountName
 });
 if (event.action=='update'){
 this.$router.push({ name:'UPDATE_TIMESHEET_DETAIL', 
 params: { timeSheetDetailId: selectedOneTimeSheetDetail.timeSheetDetailId,
 dateOfLesson : selectedOneTimeSheetDetail.dateOfLesson,
 comments : selectedOneTimeSheetDetail.comments,
 actualTimeIn : selectedOneTimeSheetDetail.actualTimeIn,
 actualTimeOut : selectedOneTimeSheetDetail.actualTimeOut,
 officialTimeInHHMM : selectedOneTimeSheetDetail.officialTimeInHHMM,
 officialTimeOutHHMM : selectedOneTimeSheetDetail.officialTimeOutHHMM,
 signedStatus : selectedOneTimeSheetDetail.signedStatus,
 actualTimeInHHMM : selectedOneTimeSheetDetail.actualTimeInHHMM,
 actualTimeOutHHMM : selectedOneTimeSheetDetail.actualTimeOutHHMM,
 customerAccountName : selectedOneTimeSheetDetail.customerAccountName,
 sessionSynopsisList : this.sessionSynopsisList,
 comments : selectedOneTimeSheetDetail.comments,
 sessionSynopsisNamesInArray : this.transformSessionSynopsisNamesToArrayOfNames(selectedOneTimeSheetDetail.sessionSynopsisNames)
 }});
 }else if (event.action=='signature'){
 this.$router.push({ name:'UPDATE_TIMESHEET_SIGNATURE', 
 params: { timeSheetDetailId: selectedOneTimeSheetDetail.timeSheetDetailId,
 dateOfLesson : selectedOneTimeSheetDetail.dateOfLesson,
 comments : selectedOneTimeSheetDetail.comments,
 actualTimeIn : selectedOneTimeSheetDetail.actualTimeIn,
 actualTimeOut : selectedOneTimeSheetDetail.actualTimeOut,
 officialTimeInHHMM : selectedOneTimeSheetDetail.officialTimeInHHMM,
 officialTimeOutHHMM : selectedOneTimeSheetDetail.officialTimeOutHHMM,
 signedStatus : selectedOneTimeSheetDetail.signedStatus,
 actualTimeInHHMM : selectedOneTimeSheetDetail.actualTimeInHHMM,
 actualTimeOutHHMM : selectedOneTimeSheetDetail.actualTimeOutHHMM,
 customerAccountName : selectedOneTimeSheetDetail.customerAccountName,
 comments : selectedOneTimeSheetDetail.comments,
 sessionSynopsisNamesInArray : this.transformSessionSynopsisNamesToArrayOfNames(selectedOneTimeSheetDetail.sessionSynopsisNames)
 }});
 }
        },    
        transformSessionSynopsisNamesToArrayOfNames(inSessionSynopsisNames){
             var index=0;
             var foundDelimiter = false;
             var sessionSynopsisNamesInArray = [];
             if (inSessionSynopsisNames.indexOf(',')!=-1){
               foundDelimiter = true;
             };
             if (foundDelimiter==true){
               sessionSynopsisNamesInArray = inSessionSynopsisNames.split(',').map(function(item) {
                       return item.trim();
                });
             };
             if ((foundDelimiter==false)&&(inSessionSynopsisNames.length!=0)){
               sessionSynopsisNamesInArray.push(inSessionSynopsisNames); 
             };
             //http://xahlee.info/js/js_convert_array-like.html
             // Convert to real prototype array
             // The Vuetify Select component nees a real prototype array.
             var sessionSynopsisNamesInArray = Array.prototype.slice.call(sessionSynopsisNamesInArray);
             return sessionSynopsisNamesInArray;
        },
            getWeekDayName : function(inWeedDayNumber) {
            var dayNameList = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday',
                'Saturday'];
            console.log(dayNameList[inWeedDayNumber]);
            return dayNameList[inWeedDayNumber];
            },
            getTimeFromMins : function (mins) {
                //https://stackoverflow.com/questions/36035598/how-to-convert-minutes-to-hours-using-moment-js
                // Do not include the first validation check if you want, for example,
                // getTimeFromMins(1530) to equal getTimeFromMins(90) (i.e. mins rollover)
                if (mins >= 24 * 60 || mins < 0) {
                    throw new RangeError("Valid input should be greater than or equal to 0 and less than 1440.");
                }
                var h = mins / 60 | 0,
                    m = mins % 60 | 0;
                return moment.utc().hours(h).minutes(m).format("hh:mm A");
            },
            loadTimeSheetDetailData: function () {
                var that = this;
                var $requestTracker = null;
                $requestTracker = jQuery.ajax({
                    type: 'GET',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                    },
                    url: '/API/TimeInTimeOutData/GetCurrentMonthTimeSheetDataAndConfiguration',
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded'
                });
                $requestTracker.done(function (data) {
                    console.dir(data);
                    that.timeSheetDetailList = data.timeSheetDetailList;
                    that.sessionSynopsisList = data.sessionSynopsisList;
                    that.timeSheet = data.timeSheet;
                    that.progressState.query=false;
                    that.progressState.show=false;
                    that.beforeDestroy();
                });
            
        }
}
}
/*
In VueJs you create and register a component using the Vue.component() constructor which takes in 2 parameters:
The name of your component
An object containing options for your component.
timesheetdetail-item is the naming convention which I have chosen. I usually use a "-" character
between words.
*/
Vue.component('timesheetdetail-item', {
    data: function() {
      console.log(this.customerAccountName);
      console.log(this.timeSheetDetailId);
    return{
    
    moment:moment
    }
  },
  template: `


  <v-card-text>
         <v-card-row xs12 class="grey darken-1">
            <v-card-title>
            <span class="white--text">{{customerAccountName}}</span>
            </v-card-title>
          </v-card-row>     
            
          <v-card-row xs12 class="text-xs-left">
          <v-card-text>{{moment(dateOfLesson).format("DD/MM/YYYY dddd")}}</v-card-text>
          </v-card-row xs12>
          
        
          <v-card-row xs12 class="text-xs-left">
          <v-card-text> From {{officialTimeInHHMM}} to {{officialTimeOutHHMM}}</v-card-text>
          </v-card-row>     

<v-card-row xs12 class="text-xs-left">
          <v-card-text> 
        <div v-if="signedStatus === true">
          <div style="color:green;">Signed</div>
          <div style="color:grey;"> Last changed 
        {{  moment(updatedAt).fromNow()}}
        </div>
        </div>
        <div v-else-if="(actualTimeIn === null) && (actualTimeOut === null) ">
            <div  style="color:brown;">Not updated</div>
        </div>
        <div v-else-if="!((actualTimeIn === null) && (actualTimeOut === null)) ">
            <div  style="color:brown;" >Partially completed</div>
            <div style="color:grey;"> Last changed 
              {{ moment(updatedAt).fromNow()}}
            </div>            
        </div>        
        <div v-if="isReplacement === true">
                      <div><span style="color:blue">Is replacement</span></div>
        </div>
 
</v-card-text>
          </v-card-row>     

    <v-divider></v-divider>


        <v-layout class="xs12 md12" >
        <v-spacer></v-spacer>
        <v-list-tile-action >
        <v-menu offset-y>
          <v-btn primary light slot="activator" >Manage</v-btn>
          <v-list>
            <v-list-item>
              <v-list-tile @click.native="gotoUpdate" >
                <v-list-tile-title >Update</v-list-tile-title>
              </v-list-tile>
              <v-list-tile @click.native="gotoSignature" >
                <v-list-tile-title>Get Signature</v-list-tile-title>
              </v-list-tile>          
            </v-list-item>
          </v-list>
        </v-menu>
      </v-list-tile-action>
      <v-spacer></v-spacer>
      </v-layout>

   <v-divider></v-divider>
   <v-btn error light text-xs-center @click.native="$emit('remove')">Delete</v-btn>
</v-card-text>


  `,
  props: ['updatedAt','customerAccountName','dateOfLesson','officialTimeInHHMM','officialTimeOutHHMM','signedStatus','timeSheetDetailId','isReplacement','actualTimeIn','actualTimeOut'],
  methods: {
     gotoUpdate : function (){
      console.log('The detected time sheet detail data record id is: ' + this.timeSheetDetailId);
      console.log('gotoUpdate method was called and emitting to the parent. At the same time pass the id value');
      // handle data and give it back to parent by interface
      this.$emit('interface', {id:this.timeSheetDetailId,action:'update'});
     },
     gotoSignature : function (){
      console.log('The detected time sheet detail data record id is: ' + this.timeSheetDetailId);
      console.log('gotoSignature method was called and emitting to the parent. At the same time pass the id value');
      // handle data and give it back to parent by interface
      this.$emit('interface', {id:this.timeSheetDetailId,action:'signature'});
     },
     
  }
});



    
   
    function getCurrentMonthName(){
        var monthNameList = ["January", "February", "March", "April", "May", "June",
            "July", "August", "September", "October", "November", "December"
        ];
        var currentDate = new Date();
       
        return monthNameList[currentDate.getMonth()];
    }



</script>
<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h1, h2 {
  font-weight: normal;
}

ul {
  list-style-type: none;
  padding: 0;
}

li {
  display: inline-block;
  margin: 0 10px;
}

a {
  color: #42b983;
}
.card-with-margin-fix .card__title .menu {
  margin: -9px
}
</style>
