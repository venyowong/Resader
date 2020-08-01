<template>
  <div>
    <div style="float: right;">
      <el-button type="danger" round style="margin-right: 10px;" @click="unsubscribe">
        取消订阅
      </el-button>
    </div>
    <div>
      <h3>
        {{title}}
      </h3>
    </div>
    <div class="article" :key="article.id" v-for="article in articles">
      <div v-if="!ignoreRead || !article.read">
        <el-link :underline="false" @click="readArticle(article)">
          <h3>
            <font v-if="!article.read">{{article.title}}</font>
            <font color="#808080" v-else>{{article.title}}</font>
          </h3>
        </el-link>
        <p><font color="#808080">{{article.summary}}</font></p> 
        <div><font color="#808080">{{article.published}}</font></div>
      </div>
    </div>
    <el-row v-if="articles != null && articles.length > 0">
      <el-button @click="readAndLoad">以上标记为已读，并加载更多</el-button>
      <el-button @click="loadMore">加载更多</el-button>
    </el-row>
  </div>
</template>

<script>
  import common from "@/common.js"

  export default {
    data() {
      return {
        articles: [],
        id: common.getQueryStringByName("id"),
        title: decodeURIComponent(common.getQueryStringByName("name")),
        user: common.getUser(),
        endTime: null,
        ignoreRead: window.localStorage.getItem("ignoreRead") == "true"
      }
    },
    methods: {
      unsubscribe() {
        if (!this.id) {
          return;
        }

        fetch(`${common.baseUrl}rss/unsubscribe`, {
            body: JSON.stringify({
                UserId: this.user.id,
                Feeds: [this.id]
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
            this.$message("取消订阅成功");
            window.location.href = "./";
          }
          else {
            this.$message(result.msg);
          }
        }.bind(this));
      },
      loadArticles: function(append) {
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
            if (response.data.length > 0) {
              this.endTime = response.data[response.data.length - 1].published;
            }
            if (append) {
              this.articles = this.articles.concat(response.data);
            }
            else {
              this.articles = response.data;
            }
            var unreadCount = response.data.filter(a => !a.read).length;
            var msg = '';
            if (unreadCount > 0) {
              msg = `已加载${unreadCount}篇未读文章`;
            }
            else {
              msg = `已加载${response.data.length}篇已读文章`;
            }
            this.$message(msg);
          }
          else {
            this.$message(result.msg);
          }
        }.bind(this));
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
        })
        .then(function(response) {
          let result = common.checkResponse(response);
          if (result.result) {
            for (var i = 0; i < ids.length; i++) {
              var article = this.articles.find(a => a.id == ids[i]);
              article.read = true;
            }
          }
          else {
            this.$message(result.msg);
          }
        }.bind(this));
      },
      readAndLoad() {
        this.readArticles(this.articles.filter(item => !item.read).map(a => a.id));
        this.loadArticles()
      },
      loadMore() {
        this.loadArticles(true);
      },
      readArticle(article) {
        this.readArticles([article.id]);
        window.open(article.url, '_blank');
      }
    },
    created() {
      this.loadArticles();
    },
    watch: {
      $route() {
        this.id = common.getQueryStringByName("id");
        this.title = decodeURIComponent(common.getQueryStringByName("name"));
        this.endTime = null;
        this.articles = [];
        this.loadArticles();
      }
    }
  }
</script>

<style scoped>
  .article {
    padding-bottom: 26px;
  }
</style>