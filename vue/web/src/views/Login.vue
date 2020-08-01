<template>
  <div>
    <h3>登录</h3>
    <el-form label-position="right" label-width="80px">
      <el-form-item label="邮箱">
        <el-input v-model="mail" class="input"></el-input>
      </el-form-item>
      <el-form-item label="密码">
          <el-input type="password" v-model="password" class="input"></el-input>
      </el-form-item>
      <el-form-item>
          <el-button type="primary" @click="login">登录</el-button>
          <el-button @click="signup">注册</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
  import common from "@/common.js"
  import md5 from "js-md5"

  var user = localStorage.getItem("user");
  if (user) {
      window.location.href = "./";
  }

  export default {
    data() {
      return {
        mail: null,
        password: null
      }
    },
    methods: {
      login: function () {
        if (!this.mail) {
            this.$message("请输入邮箱");
            return;
        }
        if (!this.password) {
            this.$message("请输入密码");
            return;
        }

        fetch("https://venyo.cn/resader/user/login", {
            body: JSON.stringify({
                Mail: this.mail,
                Password: md5(this.password)
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
          let result = common.checkResponse(response, function (code) {
            switch (code) {
              case 101:
                  return "该邮箱尚未注册";
              case 102:
                  return "密码错误";
            }
          });
          if (result.result) {
              window.localStorage.setItem("user", JSON.stringify(response.data));
              window.location.href = "./";
          }
          else {
            this.$message(result.msg);
          }
        }.bind(this));
      },
      signup: function () {
        if (!this.mail) {
            this.$message("请输入邮箱");
            return;
        }
        if (!this.password) {
            this.$message("请输入密码");
            return;
        }

        fetch("https://venyo.cn/resader/user/signup", {
            body: JSON.stringify({
                Mail: this.mail,
                Password: md5(this.password)
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
          let result = common.checkResponse(response, function (code) {
              switch (code) {
                  case 101:
                      return "该邮箱已被注册";
              }
          });
          if (result.result) {
              window.localStorage.setItem("user", JSON.stringify(response.data));
              window.location.href = "./";
          }
          else {
            this.$message(result.msg);
          }
        });
      }
  }
  };
</script>

<style scoped>
  .input {
    width: auto;
  }
</style>