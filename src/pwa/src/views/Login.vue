<template>
  <div>
    <van-nav-bar title="Login" />

    <van-form>
      <van-field
        v-model="mail"
        name="邮箱"
        label="邮箱"
        placeholder="邮箱"
      />
      <van-field
        v-model="password"
        type="password"
        name="密码"
        label="密码"
        placeholder="密码"
      />
      <div style="margin: 16px;">
        <van-button round block type="info" @click="login">
          登录
        </van-button>
      </div>
      <div style="margin: 16px;">
        <van-button round block type="info" @click="signup">
          注册
        </van-button>
      </div>
      <div style="margin: 16px;">
        <van-button round block type="info" url="https://venyo.cn/resader/opensecurity/oauth?service=github">
          使用 Github 登录
        </van-button>
      </div>
      <div style="margin: 16px;">
        <van-button round block type="info" url="https://venyo.cn/resader/opensecurity/oauth?service=gitee">
          使用 Gitee 登录
        </van-button>
      </div>
    </van-form>
  </div>
</template>

<script>
  import common from "@/common.js"
  import md5 from "js-md5"
  import Vue from 'vue';
  import { NavBar, Button, Dialog, Field, Form } from 'vant';

  Vue.use(NavBar);
  Vue.use(Button);
  Vue.use(Dialog);
  Vue.use(Field);
  Vue.use(Form);

  export default {
    data() {
      return {
        mail: null,
        password: null
      }
    },
    methods: {
      login() {
        if (!this.mail) {
            Dialog({message: "请输入邮箱"});
            return;
        }
        if (!this.password) {
            Dialog({message: "请输入密码"});
            return;
        }

        fetch(`${common.baseUrl}user/login`, {
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
            Dialog({message: result.msg});
          }
        }.bind(this));
      },
      signup() {
        if (!this.mail) {
            Dialog({message: "请输入邮箱"});
            return;
        }
        if (!this.password) {
            Dialog({message: "请输入密码"});
            return;
        }

        fetch(`${common.baseUrl}user/signup`, {
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
            Dialog({message: result.msg});
          }
        });
      }
    }
  }
</script>