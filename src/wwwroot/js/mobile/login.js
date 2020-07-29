var user = localStorage.getItem("user");
if (user) {
    window.location.href = "./index.html";
}

var app = new Vue({
    el: '#app',
    data: function () {
        return {
            mail: null,
            password: null
        };
    },
    methods: {
        login: function () {
            if (!app.loginForm.mail) {
                this.$message("请输入邮箱");
                return;
            }
            if (!app.loginForm.password) {
                this.$message("请输入密码");
                return;
            }

            fetch("../user/login", {
                body: JSON.stringify({
                    Mail: app.loginForm.mail,
                    Password: md5(app.loginForm.password)
                }),
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(function (response) {
                    if (response.status == 200) {
                        return response.json();
                    }
                })
                .then(function (response) {
                    if (checkResponse(response, function (code) {
                        switch (code) {
                            case 101:
                                return "该邮箱尚未注册";
                            case 102:
                                return "密码错误";
                        }
                    })) {
                        window.localStorage.setItem("user", JSON.stringify(response.data));
                        window.location.href = "./index.html";
                    }
                });
        },
        signup: function () {
            if (!app.mail) {
                this.$message("请输入邮箱");
                return;
            }
            if (!app.password) {
                this.$message("请输入密码");
                return;
            }

            fetch("../user/signup", {
                body: JSON.stringify({
                    Mail: app.mail,
                    Password: md5(app.password)
                }),
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                }
            })
                .then(function (response) {
                    if (response.status == 200) {
                        return response.json();
                    }
                })
                .then(function (response) {
                    if (checkResponse(response, function (code) {
                        switch (code) {
                            case 101:
                                return "该邮箱已被注册";
                        }
                    })) {
                        window.localStorage.setItem("user", JSON.stringify(response.data));
                        window.location.href = "./index.html";
                    }
                });
        }
    }
});