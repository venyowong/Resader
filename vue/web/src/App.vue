<template>
  <div id="app">
    <el-container>
    <el-aside style="width: 300px">
      <el-menu default-active="2" :default-openeds="defaultOpeneds">
        <el-menu-item index="1">
          <i class="el-icon-s-home"></i>
          <span slot="title" @click="toIndex">Resader</span>
        </el-menu-item>
        <el-submenu index="2">
          <template slot="title">
            <i class="el-icon-document"></i>
            <span>RSS</span>
          </template>
          <el-menu-item :key="feed.id" v-for="feed in feeds" @click="toArticles(feed.id, feed.title)">
            <el-badge value="new" class="item"  v-if="feed.active">
              {{feed.title}}
            </el-badge>
            <div v-else>{{feed.title}}</div>
          </el-menu-item>
        </el-submenu>
        <el-menu-item index="3" @click="subscribe">
            <i class="el-icon-download"></i>
            <span>Import</span>
        </el-menu-item>
        <el-menu-item index="4" @click="exportOpml">
            <i class="el-icon-upload2"></i>
            <span>Export</span>
        </el-menu-item>
        <el-menu-item index="5" @click="toSettings">
            <i class="el-icon-setting"></i>
            <span>Settings</span>
        </el-menu-item>
        <el-menu-item index="6" @click="logout">
          <i class="el-icon-switch-button"></i>
          <span>Logout</span>
        </el-menu-item>
      </el-menu>
    </el-aside>

    <el-main>
      <router-view/>
    </el-main>
  </el-container>
  </div>
</template>

<script>
  import common from "@/common.js"

  let user = common.getUser();
  if (!user) {
    window.location.href = "#/login";
  }

  export default {
    data() {
      return {
        feeds: [],
        defaultOpeneds: ["2"]
      };
    },
    methods: {
      getFeeds() {
        fetch(`${common.baseUrl}rss/feeds?userId=${user.id}`, {
          method: "GET",
          headers: {
            "Authorization": `Bearer ${user.token}`
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
            this.$message(result.msg);
          }
        }.bind(this));
      },
      getActiveFeeds() {
        if (this.feeds.length > 0) {
          fetch(`${common.baseUrl}rss/activefeeds?userId=${user.id}`, {
            method: "GET",
            headers: {
              "Authorization": `Bearer ${user.token}`
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
              this.$message(result.msg);
            }
          }.bind(this));
        }

        setTimeout(this.getActiveFeeds, 5000 * 60);
      },
      toArticles(id, name) {
        window.location.href = `#/articles?id=${id}&name=${name}`;
      },
      toIndex() {
        window.location.href = "#/";
      },
      subscribe() {
        this.$prompt('请输入 RSS 链接', 'Import', {
          confirmButtonText: '确定',
          cancelButtonText: '取消'
        }).then(({ value }) => {
          fetch(`${common.baseUrl}rss/subscribe`, {
              body: JSON.stringify({
                  UserId: user.id,
                  Feeds: [value]
              }),
            method: "POST",
            headers: {
                "Authorization": `Bearer ${user.token}`,
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
              this.feeds.push({
                id: response.data[0].id,
                title: response.data[0].title
              })
              window.location.href = `#/articles?id=${response.data[0].id}&name=${response.data[0].title}`;
            }
            else {
              this.$message(result.msg);
            }
          }.bind(this));
        });
      },
      exportOpml() {
        window.open(`${common.baseUrl}rss/opml.xml?userId=${user.id}`, '_blank');
      },
      toSettings() {
        window.location.href = "#/settings";
      },
      logout() {
        window.localStorage.removeItem("user");
        window.location.href = "./";
      }
    },
    created() {
      this.getFeeds();
    }
  }
</script>

<style scoped>
  .el-container {
    height: 98vh;
  }
</style>
