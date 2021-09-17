<template>
  <div>
    <van-nav-bar title="我"/>

    <van-cell-group title="基础信息">
      <van-cell title="邮箱" :value="user.mail" />
    </van-cell-group>

    <van-cell-group title="设置">
      <van-cell title="只展示未读文章">
        <template #right-icon>
          <van-switch v-model="ignoreRead" @change="onIgnoreReadChange" />
        </template>
      </van-cell>
    </van-cell-group>

    <van-cell-group title="操作">
      <van-cell title="Import" value="导入 RSS" is-link @click="showImport = true" />
      <van-cell title="Export" value="导出 OPML" is-link :url="baseUrl + 'rss/opml.xml?userId=' + user.id" />
      <van-cell title="重置密码" is-link to="/resetpwd" />
      <van-button class="bottom-button" size="large" @click="logout">退出登录</van-button>
    </van-cell-group>

    <van-dialog v-model="showImport" title="请输入 RSS 链接" show-cancel-button @confirm="subscribe">
      <van-field v-model="rssUrl" placeholder="RSS" />
    </van-dialog>
  </div>
</template>

<script>
  import common from "@/common.js";
  import Vue from 'vue';
  import { Cell, NavBar, Button, Dialog, CellGroup, Switch, Field } from 'vant';

  Vue.use(Cell);
  Vue.use(NavBar);
  Vue.use(Button);
  Vue.use(Dialog);
  Vue.use(CellGroup);
  Vue.use(Switch);
  Vue.use(Field);

  export default {
    data() {
      return {
        user: common.getUser(),
        ignoreRead: window.localStorage.getItem("ignoreRead") == "true",
        showImport: false,
        rssUrl: null,
        baseUrl: common.baseUrl
      }
    },
    methods: {
      onIgnoreReadChange() {
        window.localStorage.setItem("ignoreRead", String(this.ignoreRead));
      },
      subscribe() {
        fetch(`${common.baseUrl}rss/subscribe`, {
          body: JSON.stringify([this.rssUrl]),
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
      },
      logout() {
        window.localStorage.removeItem("user");
        window.location.href = "./";
      }
    }
  }
</script>

<style>
  .bottom-button {
    margin-top: 30px;
  }
</style>