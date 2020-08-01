<template>
  <div>
    <van-nav-bar :title="title" left-text="返回" left-arrow
      @click-left="back" right-text="全标记为已读" @click-right="readAll"/>

    <van-list
      v-model="loading"
      :finished="finished"
      finished-text="没有更多了 ヽ(ー_ー)ノ"
      @load="onLoad"
    >
      <van-swipe-cell v-for="article in articles" :key="article.id">
        <van-cell :border="false" :title="article.title" is-link :class="article.read ? 'read' : ''"
          :label="article.summary ? article.summary : article.published"
          @click="read(article, true)"  v-if="!ignoreRead || !article.read"/>
        <template #right>
          <van-button class="swipe-button" square text="标记为已读" @click="read(article)" />
        </template>
      </van-swipe-cell>
    </van-list>
  </div>
</template>

<script>
  import common from "@/common.js";
  import Vue from 'vue';
  import { List, Cell, NavBar, SwipeCell, Button, Dialog } from 'vant';

  Vue.use(List);
  Vue.use(Cell);
  Vue.use(NavBar);
  Vue.use(SwipeCell);
  Vue.use(Button);
  Vue.use(Dialog);

  export default {
    data() {
      return {
        id: common.getQueryStringByName("id"),
        title: decodeURIComponent(common.getQueryStringByName("name")),
        loading: false,
        finished: false,
        articles: [],
        endTime: null,
        user: common.getUser(),
        ignoreRead: window.localStorage.getItem("ignoreRead") == "true"
      }
    },
    methods: {
      back() {
        window.location.href = "#/";
      },
      onLoad() {
        fetch(`${common.baseUrl}rss/articles?feedId=${this.id}&page=0&pageCount=30&endTime=${this.endTime}&userId=${this.user.id}`, {
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
              this.loading = false;
              if (response.data.length > 0) {
                this.endTime = response.data[response.data.length - 1].published;
              }
              if (response.data.length < 30) {
                  this.finished = true;
              }
              var unreadCount = response.data.filter(a => !a.read).length;
              if (unreadCount <= 0) {
                this.finished = true;
              }
              this.articles = this.articles.concat(response.data);
            }
            else {
              Dialog({message: result.msg});
            }
          }.bind(this));
      },
      read(article, needNav) {
        if (!article.read) {
          this.readArticles([article.id]);
        }
        if (needNav) {
          window.open(article.url, '_blank');
        }
      },
      readArticles(ids) {
        fetch(`${common.baseUrl}rss/read`, {
          method: "POST",
            body: JSON.stringify({
                UserId: this.user.id,
                Articles: ids
            }),
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
            for (var i = 0; i < ids.length; i++) {
              var article = this.articles.find(a => a.id == ids[i]);
              article.read = true;
            }
          }
          else {
            Dialog({message: result.msg});
          }
        }.bind(this));
      },
      readAll() {
        Dialog.confirm({
          title: "提示",
          message: "确定将当前所有文章标记为已读?"
        })
        .then(() => {
          let ids = this.articles.filter(item => !item.read).map(item => item.id);
          this.readArticles(ids);
        })
      }
    }
  }
</script>

<style>
  .swipe-button {
    color: white;
    background-color: gray;
  }

  .read {
    color: #808080;
  }
</style>