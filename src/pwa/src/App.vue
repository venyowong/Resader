<template>
  <div id="app">
    <router-view/>
    <van-tabbar v-model="active" route>
      <van-tabbar-item replace name="home" to="/" icon="home-o">首页</van-tabbar-item>
      <van-tabbar-item replace name="recommend" to="/recommend" icon="gem-o">精选</van-tabbar-item>
      <van-tabbar-item replace name="settings" to="/settings" icon="user-o">我</van-tabbar-item>
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
        active: 'home',
      };
    }
  };
</script>

<style>

</style>
