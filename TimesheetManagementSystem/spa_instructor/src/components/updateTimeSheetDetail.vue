<template id="timesheetdata-update-template">
  <v-container>
  <form>
    <v-layout row wrap justify-left  >
  <v-flex xs12>
    <v-alert info dismissible v-model="messageAlertStatus" >
         The timesheet data has been signed. It is not changeable anymore.
    </v-alert>

  </v-flex>
        

        <v-flex xs12>
        <v-card width=400>
          <v-card-row xs12 class="grey darken-1">
            <v-card-title>
            <span class="white--text">{{oneTimeSheetDetailData.customerAccountName}}</span>
            </v-card-title>
          </v-card-row>     
            
          <v-card-row xs12 class="text-xs-left">
          <v-card-text>{{moment(oneTimeSheetDetailData.dateOfLesson).format("DD/MM/YYYY dddd")}}</v-card-text>
          </v-card-row xs12>
          
          <v-card-row xs12 class="text-xs-left">
          <v-card-text> From {{oneTimeSheetDetailData.officialTimeInHHMM}} to {{oneTimeSheetDetailData.officialTimeOutHHMM}}</v-card-text>
          </v-card-row>
        </v-card> 
        </v-flex>    
    <div style="height:10px;"></div>
    <v-layout row wrap justify-left v-if="oneTimeSheetDetailData.signedStatus==false" >
      <v-flex xs12 sm6 class="text-xs-left">
        <v-dialog
          persistent
          v-model="modal1"
          lazy
        >
          <v-text-field
            v-validate.initial="required" 
            name="timeInInput"
            slot="activator"
            label="Actual time-in"
            v-model="oneTimeSheetDetailData.actualTimeInHHMM"

            prepend-icon="access_time"
            readonly
          ></v-text-field>
          
          <v-time-picker v-model="oneTimeSheetDetailData.actualTimeInHHMM"  actions>
            <template scope="{ save, cancel }">
              <v-card-row actions>
                <v-btn flat primary @click.native="cancel()">Cancel</v-btn>
                <v-btn flat primary @click.native="save()">Save</v-btn>
              </v-card-row>
            </template>
          </v-time-picker>
        </v-dialog>
      </v-flex>
      <v-flex xs12 sm6 class="text-xs-left">
        <v-dialog
          persistent
          v-model="modal2"
          lazy
        >
          <v-text-field
            name="timeOutInput"
            v-validate.initial="required"
            slot="activator"
            label="Actual time-out"
            v-model="oneTimeSheetDetailData.actualTimeOutHHMM"

            prepend-icon="access_time"
            readonly
          ></v-text-field>
          
          <v-time-picker v-model="oneTimeSheetDetailData.actualTimeOutHHMM"  actions>
            <template scope="{ save, cancel }">
              <v-card-row actions>
                <v-btn flat primary @click.native="cancel()">Cancel</v-btn>
                <v-btn flat primary @click.native="save()">Save</v-btn>
              </v-card-row>
            </template>
          </v-time-picker>
        </v-dialog>
      </v-flex>
      
    </v-layout>
    <v-flex xs12 sm12 class="text-xs-left" v-if="oneTimeSheetDetailData.signedStatus==false" >
       <v-layout row wrap justify-left>
          <v-flex xs12 sm12>
            <v-select
              label="Select session synopsis(s)"
              v-bind:items="sessionSynopsisListForControls"
              v-model="oneTimeSheetDetailData.sessionSynopsisNamesInArray"
              multiple
              chips
              dark
              hint="You can speficify more than one"
              persistent-hint
            ></v-select>
          </v-flex>
       </v-layout>
    </v-flex>
    <v-flex xs12 sm12 class="text-xs-left" v-if="oneTimeSheetDetailData.signedStatus==false" >
    <v-layout row wrap justify-left>
      <v-flex xs12 sm12 class="text-xs-left">
      <v-text-field
              name="commentInput"
              label="Comments"
              multi-line
              v-model="oneTimeSheetDetailData.comments"
              hint="Provide comments if late or exceeded lesson duration"
      ></v-text-field>
      </v-flex>
     </v-layout>
     </v-flex>
  </v-layout>    

 <v-layout row wrap justify-left v-if="oneTimeSheetDetailData.signedStatus==true" >
   <v-flex xs12 sm6>
    <v-card>
      <v-flex xs12 sm6 class="text-xs-left">
         <div class="card-title blue--text">Time-in</div>
        &nbsp;<h6>{{oneTimeSheetDetailData.actualTimeInHHMM}}</h6>
      </v-flex> 
      <v-flex xs12 sm6 class="text-xs-left">
         <div class="card-title blue--text">Time-out</div>
          &nbsp;<h6>{{oneTimeSheetDetailData.actualTimeOutHHMM}}</h6>
      </v-flex> 
      <v-flex xs12 sm6 class="text-xs-left">
         <div class="card-title blue--text">Session synopses</div>
          &nbsp;<h6>{{oneTimeSheetDetailData.sessionSynopsisNamesInArray.join()}}</h6>
      </v-flex>
      <v-flex xs12 sm6 class="text-xs-left">
         <div class="card-title blue--text">Comments</div>
         &nbsp;<h6>{{oneTimeSheetDetailData.comments}}</h6>
      </v-flex>
     </v-card> 
   </v-flex>   
   <v-flex xs12 sm6>  
    <v-card>      
      <v-flex  xs12 sm6 class="text-xs-left">
          <div class="card-title blue--text">Client signature</div>
          <img :src="imageUrl" style="width:150px;height:150px;border:2px solid #546e7a" />
      </v-flex>
    </v-card> 
    </v-flex> 
  </v-layout>
  

  </form>
  <v-btn block primary v-if="oneTimeSheetDetailData.signedStatus==false" @click.native="submitForm" light>Save</v-btn>
  <v-btn block v-if="saveStatus" secondary @click.native="goBack" light>Back</v-btn>
  <v-btn block v-if="oneTimeSheetDetailData.signedStatus==true" secondary @click.native="goBack" light>Back</v-btn>
   <v-snackbar
      :timeout="timeOut"
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
import moment from 'moment';



export default {
  name: 'timesheetdata-update-template',
  data () {
    //Checking whether there are parameter values entering the component
    console.log(this.$route.params);
    //By using the above console.log, I have some idea how to initialize
    //the necessary variables here.
    
    return {
       /*Used to make the back button appear 
       so that user can navigate back after saving*/      
       saveStatus:false, 
       /*******************************************/
       //The message, context is used to set the snackbar appearance and content
       message:'',
       context:'',
       snackbar:false, /*This property make the snackbar appear */
       timeOut:3000, /*You need this property to ensure the snackbar can reappear.
        The logic inside this Vue component will reset the timeOut. You must have this
        property declared here for that to behave correctly. Or else the snackbar will
        only appear once*/
       /*******************************************/        
       messageAlertStatus:this.$route.params.signedStatus, 
       /*messageAlertStatus is used to control when the message "you cannot edit anymore" appear*/
       oneTimeSheetDetailData : { 
            timeSheetDetailId : this.$route.params.timeSheetDetailId,
            comments : this.$route.params.comments,
            dateOfLesson : this.$route.params.dateOfLesson,
            actualTimeInHHMM : this.$route.params.actualTimeInHHMM,
            actualTimeOutHHMM : this.$route.params.actualTimeOutHHMM,
            officialTimeInHHMM : this.$route.params.officialTimeInHHMM,
            officialTimeOutHHMM : this.$route.params.officialTimeOutHHMM,
            customerAccountName : this.$route.params.customerAccountName,
            signedStatus : this.$route.params.signedStatus,
            sessionSynopsisNamesInArray : this.$route.params.sessionSynopsisNamesInArray
       },
       moment:moment,
       sessionSynopsisListForControls : this.sessionSynopsisArrayForSelectComponent(this.$route.params.sessionSynopsisList)
    }},
props: ['sessionSynopsisList','sessionSynopsisNamesInArray','comments','customerAccountName','dateOfLesson','actualTimeInHHMM','actualTimeOutHHMM','officialTimeInHHMM','officialTimeOutHHMM','actualTimeIn', 'actualTimeOut','signedStatus','timeSheetDetailId'],
computed: {
  imageUrl: function() {
    let id = this.oneTimeSheetDetailData.timeSheetDetailId;
    let url = '/API/TimeInTimeOutData/GetSignatureImageByTimeSheetDetailId/';
    return (url+id);
  }
},
methods:{
    goBack:function(){
       this.$router.push({ name:'VIEW_CURRENT_MONTH_TIMESHEET'});
    },
    submitForm:function(){
      console.dir(this.$data.oneTimeSheetDetailData.sessionSynopsisNamesInArray);
      console.dir(this.oneTimeSheetDetailData.sessionSynopsisNamesInArray);
      console.log('Is this.$route.params.sessionSynopsisNamesInArray an array : ' +  Array.isArray(this.$route.params.sessionSynopsisNamesInArray));
      console.log('Is this.$data.oneTimeSheetDetailData.sessionSynopsisNamesInArray an array : ' +  Array.isArray(this.$data.oneTimeSheetDetailData.sessionSynopsisNamesInArray));
      var vueComponentData = this;
      console.dir(vueComponentData.oneTimeSheetDetailData.timeSheetDetailId);

        $.ajax({
         type: 'PUT',
         url: '/API/TimeInTimeOutData/UpdateTimeInTimeOutData',
         data:  
         {
            timeSheetDetailId : vueComponentData.oneTimeSheetDetailData.timeSheetDetailId,
            actualTimeInHHMM: vueComponentData.oneTimeSheetDetailData.actualTimeInHHMM,
            actualTimeOutHHMM: vueComponentData.oneTimeSheetDetailData.actualTimeOutHHMM,
            comments : vueComponentData.oneTimeSheetDetailData.comments,
            sessionSynopsisNames : vueComponentData.oneTimeSheetDetailData.sessionSynopsisNamesInArray.join()}
         ,
         contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
         beforeSend: function (xhr) {
           xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
          },
         dataType: 'json'}).done(function(data){
            vueComponentData.context='success';
            vueComponentData.message= data.message;
            vueComponentData.snackbar=true;
            vueComponentData.saveStatus=true;
         });

        
    },
    sessionSynopsisArrayForSelectComponent : function(inSessionSynopsisList){
      //The Vuetify Select component needs an array for binding.
      var index =0;
      var sessionSynopsisArray = [];
      for(index=0;index<inSessionSynopsisList.length;index++){
           sessionSynopsisArray.push(inSessionSynopsisList[index].sessionSynopsisName);
      }
      console.dir(sessionSynopsisArray);
      return sessionSynopsisArray;
    }
}

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
.card-title{
  margin:3px 0px 3px 3px;
}
</style>
