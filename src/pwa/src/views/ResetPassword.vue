<template>
  <div>
    <van-nav-bar title="重置密码" left-text="返回" left-arrow @click-left="back" />

    <van-form>
      <van-field
        v-model="password"
        type="password"
        name="原始密码"
        label="原始密码"
        placeholder="原始密码"
      />
      <van-field
        v-model="newPassword"
        type="password"
        name="新密码"
        label="新密码"
        placeholder="新密码"
      />
      <van-field
        v-model="cfmPassword"
        type="password"
        name="确认密码"
        label="确认密码"
        placeholder="确认密码"
      />
      <div style="margin: 16px;">
        <van-button round block type="info" @click="reset">
          确定
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
        password: null,
        newPassword: null,
        cfmPassword: null,
        user: common.getUser()
      }
    },
    methods: {
      reset() {
        if (!this.password) {
          Dialog({message: "请输入原密码"});
          return;
        }
        if (!this.newPassword) {
            Dialog({message: "请输入新密码"});
            return;
        }
        if (!this.cfmPassword) {
            Dialog({message: "请确认新密码"});
            return;
        }
        if (this.newPassword == this.password) {
            Dialog({message: "新密码不能与原密码一致"});
            return;
        }
        if (this.newPassword != this.cfmPassword) {
            Dialog({message: "两次新密码输入不一致"});
            return;
        }

        fetch(`${common.baseUrl}user/resetpassword`, {
            method: "POST",
            headers: {
                "Authorization": `Bearer ${this.user.token}`,
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                OldPassword: md5(this.password),
                Password: md5(this.newPassword),
                UserId: this.user.id
            })
        })
        .then(function (response) {
            if (response.status == 200) {
                return response.json();
            }
            if (response.status == 401) {
                this.logout();
            }
        }.bind(this))
        .then(function (response) {
          let result = common.checkResponse(response);
          if (result.result) {
            Dialog({message: "密码重置成功"});
            window.localStorage.removeItem("user");
            window.location.href = "./";
          }
          else {
            Dialog({message: result.msg});
          }
        }.bind(this));
      },
      back() {
        window.location.href = "#/settings";
      }
    }
  }
</script>