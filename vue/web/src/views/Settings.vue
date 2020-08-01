<template>
  <div>
    <h3>设置</h3>
    <el-switch v-model="ignoreRead" active-color="#13ce66" inactive-color="#dcdfe6" active-text="只展示未读" 
      inactive-text="展示全部" @change="setIgnoreRead">
    </el-switch>

    <hr/>

    <h3>重置密码</h3>
    <el-form label-position="right" label-width="80px">
      <el-form-item label="原密码">
        <el-input class="password" type="password" v-model="password"></el-input>
      </el-form-item>
      <el-form-item label="新密码">
        <el-input class="password" type="password" v-model="newPwd"></el-input>
      </el-form-item>
      <el-form-item label="确认密码">
        <el-input class="password" type="password" v-model="cfmNewPwd"></el-input>
      </el-form-item>
      <el-form-item>
        <el-button type="primary" @click="resetPassword">重置密码</el-button>
      </el-form-item>
    </el-form>
  </div>
</template>

<script>
  import common from "@/common.js"
  import md5 from "js-md5"

  export default {
   data() {
      return {
        newPwd: null,
        password: null,
        cfmNewPwd: null,
        ignoreRead: window.localStorage.getItem("ignoreRead") == "true",
        user: common.getUser()
      };
   },
   methods: {
     setIgnoreRead() {
       window.localStorage.setItem("ignoreRead", String(this.ignoreRead));
     },
     resetPassword() {
       if (!this.password) {
          this.$message("请输入原密码");
          return;
      }
      if (!this.newPwd) {
          this.$message("请输入新密码");
          return;
      }
      if (!this.cfmNewPwd) {
          this.$message("请确认新密码");
          return;
      }
      if (this.newPwd == this.password) {
          this.$message("新密码不能与原密码一致");
          return;
      }
      if (this.newPwd != this.cfmNewPwd) {
          this.$message("两次新密码输入不一致");
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
              Password: md5(this.newPwd),
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
          this.$message("密码重置成功");
          setTimeout(function () {
            window.localStorage.removeItem("user");
            window.location.href = "./";
          }, 1000)
        }
        else {
          this.$message(result.msg);
        }
      }.bind(this));
     }
   }
 }
</script>

<style scoped>
  .password {
    width: auto;
  }
</style>