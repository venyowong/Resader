(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-2d2086b7"],{a55b:function(a,e,t){"use strict";t.r(e);var s=function(){var a=this,e=a.$createElement,t=a._self._c||e;return t("div",[t("van-nav-bar",{attrs:{title:"Login"}}),t("van-form",[t("van-field",{attrs:{name:"邮箱",label:"邮箱",placeholder:"邮箱"},model:{value:a.mail,callback:function(e){a.mail=e},expression:"mail"}}),t("van-field",{attrs:{type:"password",name:"密码",label:"密码",placeholder:"密码"},model:{value:a.password,callback:function(e){a.password=e},expression:"password"}}),t("div",{staticStyle:{margin:"16px"}},[t("van-button",{attrs:{round:"",block:"",type:"info"},on:{click:a.login}},[a._v(" 登录 ")])],1),t("div",{staticStyle:{margin:"16px"}},[t("van-button",{attrs:{round:"",block:"",type:"info"},on:{click:a.signup}},[a._v(" 注册 ")])],1)],1)],1)},n=[],i=(t("d3b7"),t("69d9")),o=t("8237"),r=t.n(o),l=t("2b0e"),c=t("6b41"),u=t("b650"),d=t("2241"),p=t("565f"),f=t("772a");l["a"].use(c["a"]),l["a"].use(u["a"]),l["a"].use(d["a"]),l["a"].use(p["a"]),l["a"].use(f["a"]);var h={data:function(){return{mail:null,password:null}},methods:{login:function(){this.mail?this.password?fetch("".concat(i["a"].baseUrl,"user/login"),{body:JSON.stringify({Mail:this.mail,Password:r()(this.password)}),method:"POST",headers:{"Content-Type":"application/json"}}).then((function(a){if(200==a.status)return a.json()})).then(function(a){var e=i["a"].checkResponse(a,(function(a){switch(a){case 101:return"该邮箱尚未注册";case 102:return"密码错误"}}));e.result?(window.localStorage.setItem("user",JSON.stringify(a.data)),window.location.href="./"):Object(d["a"])({message:e.msg})}.bind(this)):Object(d["a"])({message:"请输入密码"}):Object(d["a"])({message:"请输入邮箱"})},signup:function(){this.mail?this.password?fetch("".concat(i["a"].baseUrl,"user/signup"),{body:JSON.stringify({Mail:this.mail,Password:r()(this.password)}),method:"POST",headers:{"Content-Type":"application/json"}}).then((function(a){if(200==a.status)return a.json()})).then((function(a){var e=i["a"].checkResponse(a,(function(a){switch(a){case 101:return"该邮箱已被注册"}}));e.result?(window.localStorage.setItem("user",JSON.stringify(a.data)),window.location.href="./"):Object(d["a"])({message:e.msg})})):Object(d["a"])({message:"请输入密码"}):Object(d["a"])({message:"请输入邮箱"})}}},b=h,m=t("2877"),w=Object(m["a"])(b,s,n,!1,null,null,null);e["default"]=w.exports}}]);
//# sourceMappingURL=chunk-2d2086b7.6b80e1c1.js.map