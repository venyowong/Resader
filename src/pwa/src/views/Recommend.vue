<template>
  <div style="margin-bottom: 50px;">
    <van-tabs @change="onTabChange" sticky animated>
      <van-tab  v-for="label in labels" :title="label" :key="label"></van-tab>
    </van-tabs>
    <van-card v-for="item in recommends" :key="item.feed.id" :tag="item.feed.label" :thumb="item.feed.image"
      :title="item.feed.title" :desc="item.feed.description" @click="subscribe(item.feed.url)">
      <van-cell slot="bottom" :title="item.article.title" :label="item.article.summary">
      </van-cell>
    </van-card>
  </div>
</template>

<script>
import common from "@/common.js";
  import Vue from 'vue';
  import { List, Cell, NavBar, SwipeCell, Button, Dialog, Notify, Tab, Tabs, Card } from 'vant';

  Vue.use(List);
  Vue.use(Cell);
  Vue.use(NavBar);
  Vue.use(SwipeCell);
  Vue.use(Button);
  Vue.use(Dialog);
  Vue.use(Notify);
  Vue.use(Tab);
  Vue.use(Tabs);
  Vue.use(Card);

  export default {
    data() {
      return {
        user: common.getUser(),
        labels: ["资讯", "新闻", "动漫", "电影", "阅读", "科技", "金融", "软件", "游戏", "编程", "体育", "天文"],
        recommends: []
      }
    },
    methods: {
      loadRecommends(index) {
        fetch(`${common.baseUrl}rss/recommendFeeds?label=${this.labels[index]}`)
          .then(function(response) {
            if (response.status == 200) {
              return response.json();
            }
            if (response.status == 401) {
              this.logout();
            }
          }.bind(this))
          .then(function(response) {
            this.recommends = response
          }.bind(this))
      },
      onTabChange(index) {
        this.loadRecommends(index)
      },
      subscribe(url) {
        if (!this.user) {
          Dialog({message: "请登录后再订阅"});
          return
        }

        let feed = this.feeds.find(f => f.url == url)
        if (feed) {
          window.location.href = `#/articles?id=${feed.id}&name=${feed.title}`;
        }
        else {
          Dialog.confirm({
            title: '订阅',
            message: '确认订阅?',
          })
            .then(() => {
              fetch(`${common.baseUrl}rss/subscribe`, {
                body: JSON.stringify([url]),
                method: "POST",
                headers: {
                    "token": this.user.token,
                    "Content-Type": "application/json"
                }
              })
              .then(function(response) {
                if (response.status == 200) {
                  return response.json();
                }
                if (response.status == 401) {
                  this.logout();
                }
              }.bind(this))
              .then(function(response) {
                let result = common.checkResponse(response);
                if (result.result) {
                  window.location.href = `#/articles?id=${response.data[0].id}&name=${response.data[0].title}`;
                }
                else {
                  Dialog({message: "订阅失败"});
                }
              }.bind(this));
            })
            .catch(() => {
              // on cancel
            });
        }

        
      }
    },
    created() {
      this.loadRecommends(0)

      if (this.user) {
        fetch(`${common.baseUrl}rss/feeds?userId=${this.user.id}`, {
          method: "GET",
          headers: {
              "token": this.user.token
          }
      })
      .then(function(response) {
        if (response.status == 200) {
            return response.json();
        }
        if (response.status == 401) {
            window.localStorage.removeItem("user");
            window.location.href = "#/login";
        }
      })
      .then(function(response) {
        let result = common.checkResponse(response);
        if (result.result) {
            this.feeds = response.data;
        }
        else {
          Dialog({message: result.msg});
        }
      }.bind(this));
      }
    }
  }
</script>
