// pages/articles/articles.js
let common = require('../../common')
let md5 = require('js-md5')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'

Page({

    /**
     * 页面的初始数据
     */
    data: {
        articles: null,
        page: 0
    },
    loadArticles: function() {
        let $this = this
        wx.request({
            url: `${common.baseurl}/rss/articles?feedId=${$this.data.feed}&page=${$this.data.page}&pageSize=30&userId=${$this.data.id}`,
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

              if ($this.data.articles) {
                  $this.setData({articles: $this.data.articles.concat(res.data.data)})
              }
              else {
                  $this.setData({articles: res.data.data})
              }
              if (res.data.data.length > 0) {
                  $this.setData({page: $this.data.page + 1})
              }
            }
          })
    },
    /**
     * 生命周期函数--监听页面加载
     */
    onLoad: function (options) {
        this.setData(options)

        let $this = this
        wx.getStorage({
            key: "user",
            success(res) {
                $this.setData(JSON.parse(res.data))

                $this.loadArticles()
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
        this.loadArticles()
    },

    /**
     * 用户点击右上角分享
     */
    onShareAppMessage: function () {

    },
    read: function(e) {
        console.log(this)
        console.log(e)
    }
})