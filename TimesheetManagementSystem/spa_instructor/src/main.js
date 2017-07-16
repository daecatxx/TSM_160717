// The Vue build version to load with the `import` command
// (runtime-only or standalone) has been set in webpack.base.conf with an alias.
import Vue from 'vue'

import App from './App'
import Router from './router'
import VueRouter from 'vue-router'

import Vuetify from 'vuetify'
import moment from 'moment'



import VueViewCurrentTimeSheet from './components/viewCurrentTimeSheet'
import VueViewTimeSheetHistory from './components/viewTimeSheetHistory'
import VueGenerateThisMonthTimeSheet from './components/generateThisMonthTimeSheet'
import VueUpdateTimeSheetDetail from './components/updateTimeSheetDetail'
import VueUpdateTimeSheetSignature from './components/updateTimeSheetSignature'
Vue.use(VueRouter);
//Reference: https://github.com/vuetifyjs/vuetify
Vue.use(Vuetify);




//Define your routes
const routes = [
  { path: '/', component: VueViewCurrentTimeSheet },
  { path: '/view_current_month_timesheet', name: 'VIEW_CURRENT_MONTH_TIMESHEET', component: VueViewCurrentTimeSheet },
  { path: '/view_past_month_timesheets',  component: VueViewTimeSheetHistory },
  { path: '/generate_this_month_timesheet',  component: VueGenerateThisMonthTimeSheet },
  { path: '/update_timesheet_detail', name:'UPDATE_TIMESHEET_DETAIL', component: VueUpdateTimeSheetDetail },
  { path: '/update_timesheet_signature', name:'UPDATE_TIMESHEET_SIGNATURE', component: VueUpdateTimeSheetSignature }
];

// Create the router instance and pass the `routes` option
// You can pass in additional options here, but let's
// keep it simple for now.
const router = new VueRouter({
  routes, // short for routes: routes
  mode: 'hash'
})
Vue.config.productionTip = false

/* eslint-disable no-new */
new Vue({
  el: '#app',
  router,
  template: '<App/>',
  components: { App },
  render: h => h(App)
})

Vue.filter('timeDisplay', {
  // model -> view
  // formats the value when updating the input element.
  read: function(mins) {
    if (mins!=null){
                //https://stackoverflow.com/questions/36035598/how-to-convert-minutes-to-hours-using-moment-js
                // Do not include the first validation check if you want, for example,
                // getTimeFromMins(1530) to equal getTimeFromMins(90) (i.e. mins rollover)
                if (mins >= 24 * 60 || mins < 0) {
                    throw new RangeError("Valid input should be greater than or equal to 0 and less than 1440.");
                }
                var h = mins / 60 | 0,
                    m = mins % 60 | 0;
                return moment.utc().hours(h).minutes(m).format("hh:mm A");
    }else{
      return null;
    }
  },
  // view -> model
  // formats the value when writing to the data.
  write: function(val, oldVal) {
    var m = moment(new Date(val));
    var minutes = (m.hour()*60) + m.minute();
    return minutes;
  }
})


