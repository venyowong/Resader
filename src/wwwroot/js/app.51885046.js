(function(e){function t(t){for(var o,r,a=t[0],s=t[1],u=t[2],l=0,d=[];l<a.length;l++)r=a[l],Object.prototype.hasOwnProperty.call(i,r)&&i[r]&&d.push(i[r][0]),i[r]=0;for(o in s)Object.prototype.hasOwnProperty.call(s,o)&&(e[o]=s[o]);f&&f(t);while(d.length)d.shift()();return c.push.apply(c,u||[]),n()}function n(){for(var e,t=0;t<c.length;t++){for(var n=c[t],o=!0,r=1;r<n.length;r++){var a=n[r];0!==i[a]&&(o=!1)}o&&(c.splice(t--,1),e=s(s.s=n[0]))}return e}var o={},r={app:0},i={app:0},c=[];function a(e){return s.p+"js/"+({}[e]||e)+"."+{"chunk-389a998a":"b9c4c651","chunk-2b5e42c4":"a03c4457","chunk-72b47c6d":"6e420a84","chunk-57f617f6":"4e4d44ab","chunk-64e32240":"9e4ea0b9"}[e]+".js"}function s(t){if(o[t])return o[t].exports;var n=o[t]={i:t,l:!1,exports:{}};return e[t].call(n.exports,n,n.exports,s),n.l=!0,n.exports}s.e=function(e){var t=[],n={"chunk-2b5e42c4":1,"chunk-72b47c6d":1,"chunk-64e32240":1};r[e]?t.push(r[e]):0!==r[e]&&n[e]&&t.push(r[e]=new Promise((function(t,n){for(var o="css/"+({}[e]||e)+"."+{"chunk-389a998a":"31d6cfe0","chunk-2b5e42c4":"c942c5a0","chunk-72b47c6d":"3af1296e","chunk-57f617f6":"31d6cfe0","chunk-64e32240":"c2db3dc6"}[e]+".css",i=s.p+o,c=document.getElementsByTagName("link"),a=0;a<c.length;a++){var u=c[a],l=u.getAttribute("data-href")||u.getAttribute("href");if("stylesheet"===u.rel&&(l===o||l===i))return t()}var d=document.getElementsByTagName("style");for(a=0;a<d.length;a++){u=d[a],l=u.getAttribute("data-href");if(l===o||l===i)return t()}var f=document.createElement("link");f.rel="stylesheet",f.type="text/css",f.onload=t,f.onerror=function(t){var o=t&&t.target&&t.target.src||i,c=new Error("Loading CSS chunk "+e+" failed.\n("+o+")");c.code="CSS_CHUNK_LOAD_FAILED",c.request=o,delete r[e],f.parentNode.removeChild(f),n(c)},f.href=i;var h=document.getElementsByTagName("head")[0];h.appendChild(f)})).then((function(){r[e]=0})));var o=i[e];if(0!==o)if(o)t.push(o[2]);else{var c=new Promise((function(t,n){o=i[e]=[t,n]}));t.push(o[2]=c);var u,l=document.createElement("script");l.charset="utf-8",l.timeout=120,s.nc&&l.setAttribute("nonce",s.nc),l.src=a(e);var d=new Error;u=function(t){l.onerror=l.onload=null,clearTimeout(f);var n=i[e];if(0!==n){if(n){var o=t&&("load"===t.type?"missing":t.type),r=t&&t.target&&t.target.src;d.message="Loading chunk "+e+" failed.\n("+o+": "+r+")",d.name="ChunkLoadError",d.type=o,d.request=r,n[1](d)}i[e]=void 0}};var f=setTimeout((function(){u({type:"timeout",target:l})}),12e4);l.onerror=l.onload=u,document.head.appendChild(l)}return Promise.all(t)},s.m=e,s.c=o,s.d=function(e,t,n){s.o(e,t)||Object.defineProperty(e,t,{enumerable:!0,get:n})},s.r=function(e){"undefined"!==typeof Symbol&&Symbol.toStringTag&&Object.defineProperty(e,Symbol.toStringTag,{value:"Module"}),Object.defineProperty(e,"__esModule",{value:!0})},s.t=function(e,t){if(1&t&&(e=s(e)),8&t)return e;if(4&t&&"object"===typeof e&&e&&e.__esModule)return e;var n=Object.create(null);if(s.r(n),Object.defineProperty(n,"default",{enumerable:!0,value:e}),2&t&&"string"!=typeof e)for(var o in e)s.d(n,o,function(t){return e[t]}.bind(null,o));return n},s.n=function(e){var t=e&&e.__esModule?function(){return e["default"]}:function(){return e};return s.d(t,"a",t),t},s.o=function(e,t){return Object.prototype.hasOwnProperty.call(e,t)},s.p="",s.oe=function(e){throw console.error(e),e};var u=window["webpackJsonp"]=window["webpackJsonp"]||[],l=u.push.bind(u);u.push=t,u=u.slice();for(var d=0;d<u.length;d++)t(u[d]);var f=l;c.push([0,"chunk-vendors"]),n()})({0:function(e,t,n){e.exports=n("56d7")},"238f":function(e,t,n){"use strict";var o=n("4cc4"),r=n.n(o);r.a},"4cc4":function(e,t,n){},"56d7":function(e,t,n){"use strict";n.r(t);n("e260"),n("e6cf"),n("cca6"),n("a79d");var o=n("2b0e"),r=function(){var e=this,t=e.$createElement,n=e._self._c||t;return n("div",{attrs:{id:"app"}},[n("el-container",[n("el-aside",{staticStyle:{width:"300px"}},[n("el-menu",{attrs:{"default-active":"2","default-openeds":e.defaultOpeneds}},[n("el-menu-item",{attrs:{index:"1"}},[n("i",{staticClass:"el-icon-s-home"}),n("span",{attrs:{slot:"title"},on:{click:e.toIndex},slot:"title"},[e._v("Resader")])]),n("el-submenu",{attrs:{index:"2"}},[n("template",{slot:"title"},[n("i",{staticClass:"el-icon-document"}),n("span",[e._v("RSS")])]),e._l(e.feeds,(function(t){return n("el-menu-item",{key:t.id,on:{click:function(n){return e.toArticles(t.id,t.title)}}},[t.active?n("el-badge",{staticClass:"item",attrs:{value:"new"}},[e._v(" "+e._s(t.title)+" ")]):n("div",[e._v(e._s(t.title))])],1)}))],2),n("el-menu-item",{attrs:{index:"3"},on:{click:e.subscribe}},[n("i",{staticClass:"el-icon-download"}),n("span",[e._v("Import")])]),n("el-menu-item",{attrs:{index:"4"},on:{click:e.exportOpml}},[n("i",{staticClass:"el-icon-upload2"}),n("span",[e._v("Export")])]),n("el-menu-item",{attrs:{index:"5"},on:{click:e.toSettings}},[n("i",{staticClass:"el-icon-setting"}),n("span",[e._v("Settings")])]),n("el-menu-item",{attrs:{index:"6"},on:{click:e.logout}},[n("i",{staticClass:"el-icon-switch-button"}),n("span",[e._v("Logout")])])],1)],1),n("el-main",[n("router-view")],1)],1)],1)},i=[],c=(n("99af"),n("7db0"),n("4160"),n("d3b7"),n("159b"),n("69d9")),a=c["a"].getUser();a||(window.location.href="#/login");var s={data:function(){return{feeds:[],defaultOpeneds:["2"]}},methods:{getFeeds:function(){fetch("https://venyo.cn/resader/rss/feeds?userId=".concat(a.id),{method:"GET",headers:{Authorization:"Bearer ".concat(a.token)}}).then((function(e){if(200==e.status)return e.json();401==e.status&&(window.localStorage.removeItem("user"),window.location.href="./login.html")})).then(function(e){var t=c["a"].checkResponse(e);t.result?(this.feeds=e.data,this.getActiveFeeds()):this.$message(t.msg)}.bind(this))},getActiveFeeds:function(){this.feeds.length>0&&fetch("https://venyo.cn/resader/rss/activefeeds?userId=".concat(a.id),{method:"GET",headers:{Authorization:"Bearer ".concat(a.token)}}).then(function(e){if(200==e.status)return e.json();401==e.status&&this.logout()}.bind(this)).then(function(e){var t=c["a"].checkResponse(e);t.result?(this.feeds.forEach((function(t){t.active=null!=e.data.find((function(e){return e==t.id}))})),this.$forceUpdate()):this.$message(t.msg)}.bind(this)),setTimeout(this.getActiveFeeds,3e5)},toArticles:function(e,t){window.location.href="#/articles?id=".concat(e,"&name=").concat(t)},toIndex:function(){window.location.href="#/"},subscribe:function(){var e=this;this.$prompt("请输入 RSS 链接","Import",{confirmButtonText:"确定",cancelButtonText:"取消"}).then((function(t){var n=t.value;fetch("https://venyo.cn/resader/rss/subscribe",{body:JSON.stringify({UserId:a.id,Feeds:[n]}),method:"POST",headers:{Authorization:"Bearer ".concat(a.token),"Content-Type":"application/json"}}).then(function(e){if(200==e.status)return e.json();401==e.status&&this.logout()}.bind(e)).then(function(e){var t=c["a"].checkResponse(e);t.result?(this.feeds.push({id:e.data[0].id,title:e.data[0].title}),window.location.href="#/articles?id=".concat(e.data[0].id,"&name=").concat(e.data[0].title)):this.$message(t.msg)}.bind(e))}))},exportOpml:function(){window.open("https://venyo.cn/resader/rss/opml.xml?userId=".concat(a.id),"_blank")},toSettings:function(){window.location.href="#/settings"},logout:function(){window.localStorage.removeItem("user"),window.location.href="./"}},created:function(){this.getFeeds()}},u=s,l=(n("238f"),n("2877")),d=Object(l["a"])(u,r,i,!1,null,"60adfcc6",null),f=d.exports,h=n("9483");Object(h["a"])("".concat("","service-worker.js"),{ready:function(){console.log("App is being served from cache by a service worker.\nFor more details, visit https://goo.gl/AFskqB")},registered:function(){console.log("Service worker has been registered.")},cached:function(){console.log("Content has been cached for offline use.")},updatefound:function(){console.log("New content is downloading.")},updated:function(){console.log("New content is available; please refresh.")},offline:function(){console.log("No internet connection found. App is running in offline mode.")},error:function(e){console.error("Error during service worker registration:",e)}});var p=n("8c4f");o["default"].use(p["a"]);var m=[{path:"/",name:"Index",component:function(){return n.e("chunk-57f617f6").then(n.bind(null,"d504"))}},{path:"/articles",name:"Articles",component:function(){return n.e("chunk-64e32240").then(n.bind(null,"291b"))}},{path:"/settings",name:"Settings",component:function(){return Promise.all([n.e("chunk-389a998a"),n.e("chunk-2b5e42c4")]).then(n.bind(null,"26d3"))}},{path:"/login",name:"Login",component:function(){return Promise.all([n.e("chunk-389a998a"),n.e("chunk-72b47c6d")]).then(n.bind(null,"a55b"))}}],g=new p["a"]({routes:m}),v=g,b=n("5c96"),w=n.n(b);n("0fae");o["default"].config.productionTip=!1,o["default"].use(w.a),new o["default"]({router:v,render:function(e){return e(f)}}).$mount("#app")},"69d9":function(e,t,n){"use strict";n("4d63"),n("ac1f"),n("25f0"),n("466d");function o(e){switch(e){case 1:return"参数异常";case 2:return"系统异常";default:return"请求失败(".concat(e,")")}}function r(e,t){if(!e)return{result:!1,msg:"请求异常，响应数据为空"};switch(e.code){case 0:return{result:!0,msg:""};default:if(e.message)return{result:!1,msg:e.message};var n="";return t&&(n=t(e.code)),n?{result:!1,msg:n}:{result:!1,msg:o(e.code)}}}function i(){var e=localStorage.getItem("user");return e?JSON.parse(e):null}function c(e){var t=location.href.match(new RegExp("[?&]"+e+"=([^&]+)","i"));return null==t||t.length<1?"":t[1]}t["a"]={checkResponse:r,getUser:i,getQueryStringByName:c}}});
//# sourceMappingURL=app.51885046.js.map