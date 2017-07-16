<template id="current-month-timesheet-template">
<v-container>
<div style="text-align:center">
    <h2>Generate current month timesheet</h2>
     <div><h3>Generate Timesheet for {{timeSheetMonth}} / {{timeSheetYear}}</h3></div>
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
  <!-- uncomment the following to inspect data changes when selecting account detail data -->  
  <!-- <div style="font-weight:bold">Items array of objects: {{items}}</div>
  <div style="font-weight:bold">Selected Id {{selected}}</div> -->

<div v-if="canGenerateTimeSheetData">
<v-flex v-if="progressState.show==false" xs12 md12>
      
  <v-layout class="text-xs-center">  
    <v-data-table
    
    v-bind:headers="headers"
    v-bind:items="items"
    v-bind:search="search"
    v-model="selected"
    selected-key="accountDetailId"
    select-all
    class="elevation-1"
  >
    <template slot="items" scope="props">
      <td>
        <v-checkbox
          primary
          hide-details
          v-model="props.selected"

        ></v-checkbox>
      </td>
      <td class="text-xs-left">{{ props.item.accountName }}</td>
      <td  class="text-xs-left">{{ getWeekDayName(props.item.dayOfWeekNumber-1)}}</td>
      <td  class="text-xs-left">{{ props.item.startTimeInHHMM }} to {{ props.item.endTimeInHHMM }}</td>
    </template>
  </v-data-table>
  </v-layout>
  </v-flex>


</div><!-- for wrapping the v-flex which contains the datatable -->
 <v-alert info v-bind:value="!canGenerateTimeSheetData">
    You already have timesheet data for month {{timeSheetMonth}} / {{timeSheetYear}} <br />
    <v-btn v-if="!canGenerateTimeSheetData"  block secondary @click.native="gotoCurrentTimeSheet" light>Check my current month timesheet</v-btn>
</v-alert>

<v-btn v-if="canGenerateTimeSheetData" block primary @click.native="generateTimeSheetRecords" light>Generate timesheet for this month</v-btn>




</div>
 <v-snackbar
      :timeout="3000"
      :success="context === 'success'"
      :info="context === 'info'"
      :warning="context === 'warning'"
      :error="context === 'error'"
      :primary="context === 'primary'"
      :secondary="context === 'secondary'"
      :multi-line="mode === 'multi-line'"
      :vertical="mode === 'vertical'"
      v-model="snackbar"
    >
      {{ message }}
      <v-btn light flat @click.native="snackbar = false">Close</v-btn>
    </v-snackbar>
</v-container>
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



export default {
  name: 'generate-current-month-timesheet',
  data: function() {
    return{
      progressState:{  
        value: 0,
        query: false,
        show: true,
        interval: {}
    },          
        search: '',
        selected: [],
        headers: [
          {
            text: 'Account',
            left: true,
            sortable: false,
            value: 'accountName'
          },
          { text: 'Day', left: true,value: 'day' },
          { text: 'Start time/End Time',left: true, value: '' }
          
        
        ],
        items: []
      ,
       //Used for controlling the Create Timesheet Data button disabled property
       canGenerateTimeSheetData: false,
       accountDetailData: null,
       selectedAccountDetails: [],
       canGenerateTimeSheetData : false,
       timeSheetMonth: getCurrentMonthName(),
       timeSheetYear: new Date().getFullYear(),       
       moment:moment,
       /* for snackbar control */
       message:'',
       context:'',
    }
  },
    computed: {
    // a computed getter
    startTimeAndEndTimeInHHMM: function () {
      // `this` points to the vm instance
      return this.message.split('').reverse().join('')
    }
  },
  /*
        async mounted () {
           //this.loadAccountDetailData();
           console.log('-- before await call');
           this.items = await this.loadAccountDetailData2();
           console.log('checking this.items -- after await call');
           console.dir(this.items);
        }
  */
  mounted: function(){
    /* Start the progress bar animation */
    this.queryAndIndeterminate();
    /* Then call the loadAccountDetailData() methods 
       to begin making ajax call fetching data */
    /* The logic inside loadAccountDetailData method will stop the animation */ 
    this.checkCanGenerateTimeSheetRecords();  
    this.loadAccountDetailData();
  }      
        ,
       
        methods: {
          /*Start of methods for progress bar controlling*/    
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
       /*End of methods for progress bar controlling*/


      gotoCurrentTimeSheet:function(){
       this.$router.push({ name:'VIEW_CURRENT_MONTH_TIMESHEET'});
      },      

          loadAccountDetailData_usingPromise: function () {
            return new Promise(function(resolve, reject) {
                var accountDetailList = [];
                var $requestTracker = jQuery.ajax({
                    type: 'GET',
                    beforeSend: function (xhr) {
                       xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                    },
                    url: '/API/AccountDetails/GetCurrentAccountDetailsByInstructorId',
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded'
                });
                $requestTracker.done(function (data) {
                    console.log('Obtained server side data');
                    console.dir(data);
                    resolve(data);
                });
                $requestTracker.fail(function (data) {
                    //
                });
                return accountDetailList;

            });
            }/*loadAccountDetailData_usingPromise*/
            ,
             loadAccountDetailData: function () {
                var that = this;
                var $requestTracker = jQuery.ajax({
                    type: 'GET',
                    beforeSend: function (xhr) {
                       xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                    },
                    url: '/API/AccountDetails/GetCurrentAccountDetailsByInstructorId',
                    dataType: 'json',
                    contentType: 'application/x-www-form-urlencoded'
                });
                $requestTracker.done(function (data) {
                    console.dir(data);
                    that.items = data;
                    /* I trial and error to come up with the following
                       code to make the progress bar stop after fetching
                       server-side data */
                    that.progressState.query=false;
                    that.progressState.show=false;
                    that.beforeDestroy();               
                });

            }/*loadAccountDetailData*/,
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
            checkCanGenerateTimeSheetRecords: function () {
                var that = this;
                var $requestTracker = jQuery.ajax({
                    type: 'POST',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                    },
                    url: '/API/TimeInTimeOutData/CheckAvailableTimeSheetData',
                    dataType: 'json',
                    data: {mode:'mock',year:'2017',month:'6'},
                   /* data: {mode:'actual'},*/
                   /* Note: mode can be either actual, or mock. 
                      If actual, the server side api will use 
                      the actual system date*/
                    contentType: 'application/x-www-form-urlencoded',
                });
                $requestTracker.done(function (data, textStatus, jqXHR) {
                    that.canGenerateTimeSheetData = !(data.message.isTimeSheetDataFound);
                });//end of $requestTracker.done()
                $requestTracker.fail(function (data, textStatus, jqXHR) {

                });//end of $requestTracker.fail()
            },
           generateTimeSheetRecords: function () {
               var vueComponent = this;
               //console.dir({ accountDetailIds: that.$root._data.selectedAccountDetailValues });
               var index = 0;
               var selectedAccountDetailValues = [];
               for(index=0;index<this.selected.length;index++){
                   selectedAccountDetailValues[index]=this.selected[index].accountDetailId;
               }
               console.dir('generateTimeSheetRecords -- executing -- inspecting selectedAccountDetailValues ')
               console.dir(selectedAccountDetailValues);
                var $requestTracker = jQuery.ajax({
                    type: 'POST',
                    beforeSend: function (xhr) {
                        xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                    },
                    url: '/API/TimeInTimeOutData/CreateTimeSheetData',
                    dataType: 'json',
                    data: { accountDetailIds: selectedAccountDetailValues, mode:'mock',year:'2017', month:'6' },
                    contentType: 'application/x-www-form-urlencoded',
                })
                $requestTracker.done(function (data, textStatus, jqXHR) {
                    vueComponent.context='success';
                    vueComponent.message= data.message;
                    vueComponent.snackbar=true;
                    
                    
                });//end of $requestTracker.done()
                $requestTracker.fail(function (data, textStatus, jqXHR) {
                    vueComponent.context='error';
                    vueComponent.message= data.message;
                    vueComponent.snackbar=true;
                    
                });//end of $requestTracker.fail()
              
            }/*generateTimeSheetRecords*/
        }/*methods*/

}/*export default*/

    
   
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
