(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-1c776d38"],{"93a7":function(t,s,e){"use strict";var n=e("d341"),a=e.n(n);a.a},a55b:function(t,s,e){"use strict";e.r(s);var n=function(){var t=this,s=t.$createElement,e=t._self._c||s;return e("div",[e("h3",[t._v("登录")]),e("el-form",{attrs:{"label-position":"right","label-width":"80px"}},[e("el-form-item",{attrs:{label:"邮箱"}},[e("el-input",{staticClass:"input",model:{value:t.mail,callback:function(s){t.mail=s},expression:"mail"}})],1),e("el-form-item",{attrs:{label:"密码"}},[e("el-input",{staticClass:"input",attrs:{type:"password"},model:{value:t.password,callback:function(s){t.password=s},expression:"password"}})],1),e("el-form-item",[e("el-button",{attrs:{type:"primary"},on:{click:t.login}},[t._v("登录")]),e("el-button",{on:{click:t.signup}},[t._v("注册")])],1)],1)],1)},a=[],i=(e("d3b7"),e("69d9")),o=e("8237"),r=e.n(o),l=localStorage.getItem("user");l&&(window.location.href="./");var c={data:function(){return{mail:null,password:null}},methods:{login:function(){this.mail?this.password?fetch("".concat(i["a"].baseUrl,"user/login"),{body:JSON.stringify({Mail:this.mail,Password:r()(this.password)}),method:"POST",headers:{"Content-Type":"application/json"}}).then((function(t){if(200==t.status)return t.json()})).then(function(t){var s=i["a"].checkResponse(t,(function(t){switch(t){case 101:return"该邮箱尚未注册";case 102:return"密码错误"}}));s.result?(window.localStorage.setItem("user",JSON.stringify(t.data)),window.location.href="./"):this.$message(s.msg)}.bind(this)):this.$message("请输入密码"):this.$message("请输入邮箱")},signup:function(){this.mail?this.password?fetch("".concat(i["a"].baseUrl,"user/signup"),{body:JSON.stringify({Mail:this.mail,Password:r()(this.password)}),method:"POST",headers:{"Content-Type":"application/json"}}).then((function(t){if(200==t.status)return t.json()})).then((function(t){var s=i["a"].checkResponse(t,(function(t){switch(t){case 101:return"该邮箱已被注册"}}));s.result?(window.localStorage.setItem("user",JSON.stringify(t.data)),window.location.href="./"):this.$message(s.msg)})):this.$message("请输入密码"):this.$message("请输入邮箱")}}},u=c,d=(e("93a7"),e("2877")),h=Object(d["a"])(u,n,a,!1,null,"1d23c39e",null);s["default"]=h.exports},d341:function(t,s,e){}}]);
//# sourceMappingURL=chunk-1c776d38.651ebf2b.js.map