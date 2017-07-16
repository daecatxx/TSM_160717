<template id="timesheetdata-update-signature-template">
<v-container>
    <v-layout row wrap>
    <v-card>
      <v-card-row class="blue-grey darken-1">
        <v-card-title>
        <span class="white--text">{{oneTimeSheetDetailData.customerAccountName}}</span>
      </v-card-row>     
        <divider></divider>
        <v-card-row actions class="blue-blue darken-1 mt-0">  
          <v-card-text>{{moment(oneTimeSheetDetailData.dateOfLesson).format("DD/MM/YYYY dddd")}}</v-card-text>
           <v-spacer></v-spacer>               
           <v-card-text> From {{oneTimeSheetDetailData.actualTimeInHHMM}} to {{oneTimeSheetDetailData.actualTimeOutHHMM}}</v-card-text>
      </v-card-row>      
    </v-card>     

    </v-layout>


<v-layout row wrap>


   <div id="signature_pad" class="m-signature-pad">

    <div class="m-signature-pad--body">
      <canvas id="canvas" style="width:150px;height:150px;border:solid"></canvas>
      
    </div>
    <div class="m-signature-pad--footer">
      
      <div class="left">
        <button type="button" class="button clear" data-action="clear">Clear</button>
      </div>
    </div>
  </div>

</v-layout>
 <v-layout row wrap> 
  <v-btn block primary @click.native="submitForm" light>Save</v-btn>
 </v-layout> 
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
    <canvas id="canvas_blank" style="position:absolute;width:150px;height:150px;display:none;"></canvas>
   </v-container> 
</template>
<script>
import moment from 'moment';
import 'expose-loader?$!expose-loader?jQuery!jquery'
import SignaturePad from 'signature_pad';
/* The following import for SignaturePad (trial and error) did not work.
   It gave SignaturePad is not a constructor when signaturePad = new SignaturePad(canvas);
   is evaluated */
//import 'expose-loader?$!expose-loader?SignaturePad!signature_pad'
export default {
  name: 'timesheetdata-update-signature-template',
  data () {
    //Checking whether there are parameter values entering the component
    console.log(this.$route.params);
    //By using the above console.log, I have some idea how to initialize
    //the necessary variables here.
     
    return {
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
       oneTimeSheetDetailData : this.$route.params,
       moment:moment
    }},
props: ['comments','customerAccountName','dateOfLesson','actualTimeInHHMM','actualTimeOutHHMM','officialTimeInHHMM','officialTimeOutHHMM','actualTimeIn', 'actualTimeOut','status','timeSheetDetailId'],

methods:{
    validateSignatureCanvas: function(){
      //Return false if the canvas_blank <canvas> element (which is hidden inside the template) is the
      //same as the canvas element used by the user. False means empty signature. True means, signature is not empty.
      //console.log('comparing signature : ' + (this.$el.querySelector('#canvas').toDataURL() == this.$el.querySelector('#canvas_blank').toDataURL()) )
      //console.log(this.$el.querySelector('#canvas').toDataURL());
      //console.log(this.$el.querySelector('#canvas_blank').toDataURL());
      //Return true if the canvas has digital data. Return false otherwise
      //http://jsfiddle.net/amaan/rX572/
      if(this.$('#canvas').toDataURL() == this.$('#canvas_blank').toDataURL())
        return false;
      else
        return true;
    },
    submitForm:function(){
  
        if (this.validateSignatureCanvas()==false){
            this.context='warning';
            this.message='The signature is empty. Try again.';
            this.snackbar=true;
            return false;
        }
        var parent = this;
        var parentComponentContainer = this.$el;
         
        var canvasElement = $('#canvas');
        var image =canvasElement.toDataURL("image/png");
        image = image.replace('data:image/png;base64,', '');
        //Using the following code to check if I can read the CSRF value.
        //console.log('csrf value: ' + $('input[name=__RequestVerificationToken]').val());
        $.ajax({
         type: 'POST',
         url: '/API/TimeInTimeOutData/CreateTimeInTimeOutDataSignature',
         data:  {signatureImage:image,timeSheetDetailId:parent.oneTimeSheetDetailData.timeSheetDetailId},
         contentType: 'application/x-www-form-urlencoded; charset=UTF-8',
                                 beforeSend: function (xhr) {
                            xhr.setRequestHeader('RequestVerificationToken', $('input[name=__RequestVerificationToken]').val());
                        },
         dataType: 'json'}).done(function(data){
            parent.context='success';
            parent.message='Signature is saved.';
            parent.snackbar=true;
         }).fail(function(data){
            //console.log('error occurred');//Spent a long time debugging this. Due to using this object.
            parent.context='error';
            parent.message='Something went wrong. Cannot save signature.';
            parent.snackbar=true;
            
         });
      
    }/*end of submitForm*/,
    
    prepareSignaturePad:function(){
       var that = this;
       // Adjust canvas coordinate space taking into account pixel ratio,
       // to make it look crisp on mobile devices.
       // This also causes canvas to be cleared.
       function resizeCanvas() {
          // When zoomed out to less than 100%, for some very strange reason,
          // some browsers report devicePixelRatio as less than 1
          // and only part of the canvas is cleared then.
          var ratio =  Math.max(window.devicePixelRatio || 1, 1);
          canvas.width = canvas.offsetWidth * ratio;
          canvas.height = canvas.offsetHeight * ratio;
          canvas.getContext("2d").scale(ratio, ratio);
          //The following five lines of code are additional.
          //When the logic resizes the visible canvas, I need the hidden
          //canvas to resize too. Or else cannot do comparison to check for
          //empty signature.
          var blank_canvas;
          blank_canvas = that.$el.querySelector('#canvas_blank');
          blank_canvas.width = canvas.offsetWidth * ratio;
          blank_canvas.height = canvas.offsetHeight * ratio;
          blank_canvas.getContext("2d").scale(ratio, ratio);
        
        }/*resizeCanvas*/

        //Don't remove the <v-container>. Removing it will give problems on finding the signature_pad
    var wrapper = document.getElementById("signature_pad"),
    clearButton = wrapper.querySelector("[data-action=clear]"),
    savePNGButton = wrapper.querySelector("[data-action=save-png]"),
    saveSVGButton = wrapper.querySelector("[data-action=save-svg]"),
    canvas = wrapper.querySelector("canvas"),
    signaturePad;

    /*Double checking if I can reference these elements using the above statement*/
    console.dir(canvas);
    console.dir(wrapper);


    window.onresize = resizeCanvas;
    resizeCanvas();
     
    signaturePad = new SignaturePad(canvas);
    clearButton.addEventListener("click", function (event) {
        signaturePad.clear();
    });
    

    }

   }/*methods*/,
   mounted:function(){
      this.prepareSignaturePad();

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
</style>
