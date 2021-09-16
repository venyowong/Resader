// pages/articles/articles.js
let common = require('../../common')
import Notify from '../../miniprogram_npm/@vant/weapp/notify/notify'

Page({

    /**
     * 页面的初始数据
     */
    data: {
        articles: null,
        page: 0,
        showMore: true
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
                Notify({type: "success", message: `加载了${res.data.data.length}篇文章`, duration: 1000})
                $this.setData({page: $this.data.page + 1})
              }
              if (res.data.data.length < 30) {
                  $this.setData({showMore: false})
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
        this.loadArticles()
    },

    /**
     * 用户点击右上角分享
     */
    onShareAppMessage: function () {

    },
    read: function(e) {
        console.log(e)
        let id = e.currentTarget.dataset.id
        wx.setClipboardData({
            data: this.data.articles.find(a => a.id == id).url,
            success (res) {
                Notify({type: "success", message: "文章链接已复制到剪切板，请使用浏览器打开"})
            }
          })
        wx.request({
          url: `${common.baseurl}/rss/read`,
          method: "POST",
          data: [id],
          header: {token: this.data.token}
        })
    }
})