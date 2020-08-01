import Vue from 'vue'
import VueRouter from 'vue-router'

Vue.use(VueRouter)

  const routes = [
    {
      path: '/',
      name: 'Index',
      component: () => import('../views/Index.vue')
    },
    {
      path: '/articles',
      name: 'Articles',
      component: () => import('../views/Articles.vue')
    },
    {
      path: '/settings',
      name: 'Settings',
      component: () => import('../views/Settings.vue')
    },
    {
      path: '/login',
      name: 'Login',
      component: () => import('../views/Login.vue')
    },
    {
      path: '/resetpwd',
      name: 'ResetPassword',
      component: () => import('../views/ResetPassword.vue')
    }
  ]

const router = new VueRouter({
  routes
})

export default router
