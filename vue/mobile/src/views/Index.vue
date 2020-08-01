<template>
  <div>
    <img class="center" v-lazy="logo">

    <van-list class="index-list">
      <van-swipe-cell v-for="feed in feeds" :key="feed.id">
        <van-cell :border="false" :title="feed.title" is-link 
          :value="feed.active ? '有未读文章' : ''" 
          :to="'/articles?id=' + feed.id + '&name=' + feed.title" />
        <template #right>
          <van-button square type="danger" text="取消订阅" @click="unsubscribe(feed)" />
        </template>
      </van-swipe-cell>
    </van-list>
  </div>
</template>

<script>
  import common from "@/common.js";
  import Vue from 'vue';
  import { Lazyload, List, SwipeCell, Cell, Button,Dialog } from 'vant';

  Vue.use(Lazyload);
  Vue.use(List);
  Vue.use(SwipeCell);
  Vue.use(Cell);
  Vue.use(Button);
  Vue.use(Dialog);

  export default {
    data() {
      return {
        logo: "./img/logo.png",
        feeds: [],
        user: common.getUser()
      }
    },
    methods: {
      getActiveFeeds() {
        if (this.feeds.length > 0) {
          fetch(`${common.baseUrl}rss/activefeeds?userId=${this.user.id}`, {
              method: "GET",
              headers: {
                  "Authorization": `Bearer ${this.user.token}`
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
                this.feeds.forEach(feed => {
                    feed.active = response.data.find(item => item == feed.id) != null;
                });
                this.$forceUpdate();
            }
            else {
              Dialog({message: result.msg});
            }
          }.bind(this));
        }

        setTimeout(this.getActiveFeeds, 5000 * 60);
      },
      unsubscribe(feed) {
        fetch(`${common.baseUrl}rss/unsubscribe`, {
          body: JSON.stringify({
              UserId: this.user.id,
              Feeds: [feed.id]
          }),
          method: "POST",
          headers: {
              "Authorization": `Bearer ${this.user.token}`,
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
            this.feeds = this.feeds.filter(f => f.id != feed.id);
          }
          else {
            Dialog({message: result.msg});
          }
        }.bind(this));
      }
    },
    created() {
      fetch(`${common.baseUrl}rss/feeds?userId=${this.user.id}`, {
          method: "GET",
          headers: {
              "Authorization": `Bearer ${this.user.token}`
          }
      })
      .then(function(response) {
        if (response.status == 200) {
            return response.json();
        }
        if (response.status == 401) {
            window.localStorage.removeItem("user");
            window.location.href = "./login.html";
        }
      })
      .then(function(response) {
        let result = common.checkResponse(response);
          if (result.result) {
              this.feeds = response.data;
              this.getActiveFeeds();
          }
          else {
            Dialog({message: result.msg});
          }
      }.bind(this));
    }
  }
</script>

<style scoped>
  img.center {
    margin: auto;
    display: block;
    padding-top: 80px;
    padding-bottom: 30px;
  }

  .index-list {
    margin-bottom: 50px;
  }
</style>