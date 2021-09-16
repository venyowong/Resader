// pages/recommend/recommend.js
let common = require('../../common')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'
import Dialog from '../../miniprogram_npm/@vant/weapp/dialog/dialog';

Page({

    /**
     * 页面的初始数据
     */
    data: {
        labels: ["动漫", "编程", "软件"],
        recommends: []
    },
    loadRecommends: function(label) {
        let $this = this

        wx.request({
            url: `${common.baseurl}/rss/recommendFeeds?label=${label}`,
            method: "GET",
            header: {token: $this.data.token},
            success: function(res) {
              if (res.statusCode != "200") {
                  Notify({type: "warning", message: "请求失败"})
                  return
              }

              $this.setData({recommends: res.data})
            }
          })
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

                $this.loadRecommends($this.data.labels[0])
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
    onTabChange: function(e) {
        this.loadRecommends(e.detail.title)
    },
    subscribe: function(e) {
        console.log(e)
        Dialog.confirm({
            title: '订阅',
            message: `确认订阅${e.currentTarget.dataset.title}?`,
          })
            .then(() => {
              // on confirm
              wx.request({
                  url: `${common.baseurl}/rss/subscribe`,
                  method: "POST",
                  data: [
                      e.currentTarget.dataset.url
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
        
                    wx.switchTab({
                        url: '../index/index',
                    })
                  }
                })
            })
            .catch(() => {
              // on cancel
            });
    }
})