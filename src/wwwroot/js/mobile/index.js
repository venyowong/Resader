var user = localStorage.getItem("user");
if (!user) {
    window.location.href = "./login.html";
}
user = JSON.parse(user);

var app = new Vue({
    el: '#app',
    data: function () {
        return {
            active: "home-container",
            selected: "home",
            tabbarFixed: true,
            swipeable: true,
            feeds: [],
            user: user,
            onlyShowUnread: false,
            logo: "../logo.png"
        };
    },
    methods: {
        unsubscribe: function(feed) {
            fetch(`../rss/unsubscribe`, {
                body: JSON.stringify({
                    UserId: user.id,
                    Feeds: [feed.id]
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
                app.logout();
              }
            })
            .then(function(response) {
              if (checkResponse(response)) {
                app.feeds = app.feeds.filter(f => f.id != feed.id);
              }
            });
        },
        getActiveFeeds: function() {
            if (app.feeds.length > 0) {
                fetch(`../rss/activefeeds?userId=${user.id}`, {
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
                        app.feeds.forEach(feed => {
                            feed.active = response.data.find(item => item == feed.id) != null;
                        });
                        app.reload();
                    }
                });
            }

            setTimeout(app.getActiveFeeds, 5000 * 60);
        },
        logout: function() {
          window.localStorage.removeItem("user");
          window.location.href = "./login.html";
        },
        reload: function() {
          this.$forceUpdate();
        },
        onUnsubscribe(feed) {
            this.$messagebox.confirm("确定取消订阅该 RSS 源?")
            .then(action => {
                if (action == "confirm") {
                    app.unsubscribe(feed);
                }
            });
        },
        importRss() {
            this.$messagebox.prompt("请输入 RSS 源链接")
            .then(({value, action}) => {
                if (action != "confirm") {
                    return;
                }

                fetch(`../rss/subscribe`, {
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
                    app.logout();
                  }
                })
                .then(function(response) {
                  if (checkResponse(response)) {
                      if (app.feeds.findIndex(item => item.id == response.data[0].id) < 0) {
                          app.feeds.push({
                            id: response.data[0].id,
                            title: response.data[0].title,
                            active: true
                          });
                      }
                  }
                });
            });
        },
        exportOpml() {
            window.open(`../rss/opml.xml?userId=${user.id}`, '_blank');
        },
        onReadSwitchChange() {
            if (app.onlyShowUnread) {
                window.localStorage.setItem("onlyShowUnread", "true");
            }
            else {
                window.localStorage.setItem("onlyShowUnread", "false");
            }
        }
    },
    watch: {
        selected(val, oldVal) {
            app.active = `${val}-container`;
        }
    },
    created() {
        this.onlyShowUnread = window.localStorage.getItem("onlyShowUnread") == "true";

        fetch(`../rss/feeds?userId=${user.id}`, {
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
            if (checkResponse(response)) {
                app.feeds = response.data;
                app.getActiveFeeds();
            }
        });
    }
});