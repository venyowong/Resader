import Vue from 'vue'
import App from './App.vue'
import './registerServiceWorker'
import router from './router'
import { Tabbar, TabbarItem } from 'vant'
import 'vant/lib/index.css';

Vue.config.productionTip = false
Vue.use(Tabbar)
Vue.use(TabbarItem)

new Vue({
  router,
  render: h => h(App)
}).$mount('#app')
