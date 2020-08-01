<template>
  <div id="app">
    <router-view/>
    <van-tabbar v-model="active">
      <van-tabbar-item replace to="/" icon="home-o">Home</van-tabbar-item>
      <van-tabbar-item replace to="/settings" icon="setting-o">Settings</van-tabbar-item>
    </van-tabbar>
  </div>
</template>

<script>
  import common from "@/common.js";

  if (!common.getUser()) {
    window.location.href = "#/login";
  }
  
  export default {
    data() {
      return {
        active: 0,
      };
    },
  };
</script>

<style>

</style>
