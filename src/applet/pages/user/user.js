// pages/user/user.js
let common = require('../../common')
let md5 = require('js-md5')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'

Page({

    /**
     * 页面的初始数据
     */
    data: {
        mail: null,
        mail_input: null,
        password_input: null,
        showImportDialog: false,
        rss: null,
        opml: null
    },

    /**
     * 生命周期函数--监听页面加载
     */
    onLoad: function (options) {
        let $this = this
        wx.getStorage({
            key: 'mail_input',
            success (res) {
                $this.setData({mail_input: res.data})
            }
          })
        wx.getStorage({
            key: "user",
            success(res) {
                $this.setData(JSON.parse(res.data))
                $this.setData({opml: `${common.baseurl}/rss/opml.xml?userId=${$this.data.id}`})
            }
        })
        wx.getStorage({
            key: "showUnread",
            success(res) {
                $this.setData({showUnread: res.data == "true"})
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
    login: function() {
        let $this = this
        wx.request({
          url: `${common.baseurl}/user/login`,
          data: {
              mail: this.data.mail_input,
              password: md5(this.data.password_input)
          },
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

              wx.setStorage({key: "user", data: JSON.stringify(res.data.data)})
              $this.setData(res.data.data)
          }
        })
    },
    onMailChange: function(e) {
        wx.setStorage({key: "mail_input", data: e.detail})
    },
    showUnreadChange: function(e) {
        wx.setStorage({key: "showUnread", data: e.detail.value ? "true" : "false"})
    },
    onCloseImportDialog: function(e) {
        if (this.data.rss) {
            wx.request({
              url: `${common.baseurl}/rss/subscribe`,
              method: "POST",
              data: [
                  this.data.rss
              ],
              header: {token: this.data.token},
              success: function(res) {
                if (res.statusCode != "200") {
                    Notify({type: "warning", message: "请求失败"})
                    return
                }
    
                if (res.data.code != 0) {
                    Notify({type: "warning", message: res.data.message})
                    return
                }
    
                if (res.data.data.length > 0) {
                    Notify({type: "success", message: "订阅成功"})
                }
                else {
                    Notify({type: "warning", message: "订阅失败"})
                }
              }
            })
        }
        this.setData({showImportDialog: false})
    },
    onShowImportDialog: function(e) {
        this.setData({showImportDialog: true})
    },
    export: function(e) {
        wx.setClipboardData({
            data: this.data.opml,
            success (res) {
                Notify({type: "success", message: "opml 链接已复制到剪切板，请使用浏览器打开"})
            }
          })
    }
})