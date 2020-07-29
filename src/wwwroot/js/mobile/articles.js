var user = localStorage.getItem("user");
if (!user) {
    window.location.href = "./login.html";
}
user = JSON.parse(user);

var id = getQueryStringByName("id");
var name = decodeURIComponent(getQueryStringByName("name"));

var app = new Vue({
    el: '#app',
    data: function () {
        return {
            articles: [],
            id: id,
            title: name,
            endTime: "",
            allLoaded: false,
            onlyShowUnread: false
        };
    },
    methods: {
        logout: function() {
          window.localStorage.removeItem("user");
          window.location.href = "./login.html";
        },
        reload: function() {
          this.$forceUpdate();
        },
        loadBottom: function() {
            if (!app.allLoaded) {
                this.getArticles();
            }
        },
        back: function() {
            window.location.href = "./index.html";
        },
        getArticles: function() {
          fetch(`../rss/articles?feedId=${this.id}&page=0&pageCount=30&endTime=${this.endTime}&userId=${user.id}`, {
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
              app.logout();
            }
          })
          .then(function(response) {
            if (checkResponse(response)) {
              if (response.data.length > 0) {
                app.endTime = response.data[response.data.length - 1].published;
              }
              if (response.data.length < 30) {
                  app.allLoaded = true;
              }
              if (app.onlyShowUnread && app.articles.length > 0) {
                response.data = response.data.filter(item => !item.read);
              }
              else if (app.onlyShowUnread) {
                var unreadCount = response.data.filter(a => !a.read).length;
                if (unreadCount > 0) {
                  response.data = response.data.filter(item => !item.read);
                }
              }
              app.articles = app.articles.concat(response.data);
            }
          });
        },
        read(article, needNav) {
          if (!article.read) {
            app.readArticles([article.id]);
          }
          if (needNav) {
            window.open(article.url, '_blank');
          }
        },
        readArticles(ids) {
          fetch(`../rss/read`, {
            method: "POST",
              body: JSON.stringify({
                  UserId: user.id,
                  Articles: ids
              }),
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
              app.logout();
            }
          })
          .then(function(response) {
            if (checkResponse(response)) {
              for (var i = 0; i < ids.length; i++) {
                var article = app.articles.find(a => a.id == ids[i]);
                article.read = true;
              }
            }
          });
        }
    },
    created() {
      this.onlyShowUnread = window.localStorage.getItem("onlyShowUnread") == "true";
    }
});