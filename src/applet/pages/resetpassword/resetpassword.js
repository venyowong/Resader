// pages/resetpassword/resetpassword.js
let common = require('../../common')
let md5 = require('js-md5')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'

Page({

    /**
     * 页面的初始数据
     */
    data: {

    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad: function (options) {
        let $this = this
        wx.getStorage({
            key: "user",
            success(res) {
                $this.setData(JSON.parse(res.data))
            }
        })
    },

    /**
     * 生命周期函数--监听页面初次渲染完成
     */
    onReady: function () {

    },

    /**
     * 生命周期函数--监听页面显示
     */
    onShow: function () {

    },

    /**
     * 生命周期函数--监听页面隐藏
     */
    onHide: function () {

    },

    /**
     * 生命周期函数--监听页面卸载
     */
    onUnload: function () {

    },

    /**
     * 页面相关事件处理函数--监听用户下拉动作
     */
    onPullDownRefresh: function () {

    },

    /**
     * 页面上拉触底事件的处理函数
     */
    onReachBottom: function () {

    },

    /**
     * 用户点击右上角分享
     */
    onShareAppMessage: function () {

    },
    reset: function() {
        if (this.data.new_password != this.data.repeat_password) {
            Notify({type: "warning", message: "两次密码不一致"})
            return
        }
        if (this.data.new_password == this.data.old_password) {
            Notify({type: "warning", message: "新旧密码一致"})
            return
        }

        wx.request({
          url: `${common.baseurl}/user/resetpassword`,
          data: {
              oldPassword: md5(this.data.old_password),
              password: md5(this.data.new_password)
          },
          header: {token: this.data.token},
          method: "POST",
          success: function(res) {
              if (res.statusCode != "200") {
                Notify({type: "warning", message: "请求失败"})
                return
              }

              if (res.data.code != 0) {
                Notify({type: "warning", message: res.data.message})
                return
              }

              wx.navigateBack()
          }
        })
    }
})