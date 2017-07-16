import Vue from 'vue'
import Router from 'vue-router'
import VueViewCurrentTimeSheet from '@/components/viewCurrentTimeSheet'
import VueViewTimeSheetHistory from '@/components/viewTimeSheetHistory'
import VueGenerateThisMonthTimeSheet from '@/components/generateThisMonthTimeSheet'
Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'VueViewCurrentTimeSheet',
      component: VueViewCurrentTimeSheet
    }
  ]
})
