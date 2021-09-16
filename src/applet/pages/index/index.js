// index.js
// 获取应用实例
let common = require('../../common')
let md5 = require('js-md5')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'
const app = getApp()

Page({
  data: {
  },
  // 事件处理函数
  bindViewTap() {
    wx.navigateTo({
      url: '../logs/logs'
    })
  },
  onLoad() {
    let $this = this
    wx.getStorage({
        key: "user",
        success(res) {
            $this.setData(JSON.parse(res.data))

            wx.request({
              url: `${common.baseurl}/rss/feeds`,
              method: "GET",
              header: {token: $this.data.token},
              success: function(res) {
                if (res.statusCode != "200") {
                  Notify({type: "warning", message: "请求失败"})
                  return
                }
  
                if (res.data.code != 0) {
                  Notify({type: "warning", message: res.data.message})
                  return
                }

                $this.setData({feeds: res.data.data})
              }
            })
        }
    })
  },
  getUserProfile(e) {
    // 推荐使用wx.getUserProfile获取用户信息，开发者每次通过该接口获取用户个人信息均需用户确认，开发者妥善保管用户快速填写的头像昵称，避免重复弹窗
    wx.getUserProfile({
      desc: '展示用户信息', // 声明获取用户个人信息后的用途，后续会展示在弹窗中，请谨慎填写
      success: (res) => {
        console.log(res)
        this.setData({
          userInfo: res.userInfo,
          hasUserInfo: true
        })
      }
    })
  },
  getUserInfo(e) {
    // 不推荐使用getUserInfo获取用户信息，预计自2021年4月13日起，getUserInfo将不再弹出弹窗，并直接返回匿名的用户个人信息
    console.log(e)
    this.setData({
      userInfo: e.detail.userInfo,
      hasUserInfo: true
    })
  }
})
