(window["webpackJsonp"]=window["webpackJsonp"]||[]).push([["chunk-3290b8e0"],{"1dde":function(t,e,i){var n=i("d039"),r=i("b622"),s=i("2d00"),a=r("species");t.exports=function(t){return s>=51||!n((function(){var e=[],i=e.constructor={};return i[a]=function(){return{foo:1}},1!==e[t](Boolean).foo}))}},"26d3":function(t,e,i){"use strict";i.r(e);var n=function(){var t=this,e=t.$createElement,i=t._self._c||e;return i("div",[i("van-nav-bar",{attrs:{title:"Settings"}}),i("van-cell-group",{attrs:{title:"基础信息"}},[i("van-cell",{attrs:{title:"邮箱",value:t.user.mail}})],1),i("van-cell-group",{attrs:{title:"设置"}},[i("van-cell",{attrs:{title:"只展示未读文章"},scopedSlots:t._u([{key:"right-icon",fn:function(){return[i("van-switch",{on:{change:t.onIgnoreReadChange},model:{value:t.ignoreRead,callback:function(e){t.ignoreRead=e},expression:"ignoreRead"}})]},proxy:!0}])})],1),i("van-cell-group",{attrs:{title:"操作"}},[i("van-cell",{attrs:{title:"Import",value:"导入 RSS","is-link":""},on:{click:function(e){t.showImport=!0}}}),i("van-cell",{attrs:{title:"Export",value:"导出 OPML","is-link":"",url:t.baseUrl+"rss/opml.xml?userId="+t.user.id}}),i("van-cell",{attrs:{title:"重置密码","is-link":"",to:"/resetpwd"}}),i("van-button",{staticClass:"bottom-button",attrs:{size:"large"},on:{click:t.logout}},[t._v("退出登录")])],1),i("van-dialog",{attrs:{title:"请输入 RSS 链接","show-cancel-button":""},on:{confirm:t.subscribe},model:{value:t.showImport,callback:function(e){t.showImport=e},expression:"showImport"}},[i("van-field",{attrs:{placeholder:"RSS"},model:{value:t.rssUrl,callback:function(e){t.rssUrl=e},expression:"rssUrl"}})],1)],1)},r=[],s=(i("99af"),i("d3b7"),i("69d9")),a=i("2b0e"),o=i("7744"),l=i("6b41"),c=i("b650"),u=i("2241"),h=i("2638"),d=i.n(h),f=i("d282"),g=i("ba31"),v=i("b1d2"),p=Object(f["a"])("cell-group"),m=p[0],b=p[1];function y(t,e,i,n){var r,s=t("div",d()([{class:[b(),(r={},r[v["e"]]=e.border,r)]},Object(g["b"])(n,!0)]),[null==i.default?void 0:i.default()]);return e.title||i.title?t("div",[t("div",{class:b("title")},[i.title?i.title():e.title]),s]):s}y.props={title:String,border:{type:Boolean,default:!0}};var k=m(y),x=i("ea8e"),w={size:[Number,String],value:null,loading:Boolean,disabled:Boolean,activeColor:String,inactiveColor:String,activeValue:{type:null,default:!0},inactiveValue:{type:null,default:!1}},S={inject:{vanField:{default:null}},watch:{value:function(){var t=this.vanField;t&&(t.resetValidation(),t.validateWithTrigger("onChange"))}},created:function(){var t=this.vanField;t&&!t.children&&(t.children=this)}},C=i("543e"),$=Object(f["a"])("switch"),I=$[0],j=$[1],F=I({mixins:[S],props:w,computed:{checked:function(){return this.value===this.activeValue},style:function(){return{fontSize:Object(x["a"])(this.size),backgroundColor:this.checked?this.activeColor:this.inactiveColor}}},methods:{onClick:function(t){if(this.$emit("click",t),!this.disabled&&!this.loading){var e=this.checked?this.inactiveValue:this.activeValue;this.$emit("input",e),this.$emit("change",e)}},genLoading:function(){var t=this.$createElement;if(this.loading){var e=this.checked?this.activeColor:this.inactiveColor;return t(C["a"],{class:j("loading"),attrs:{color:e}})}}},render:function(){var t=arguments[0],e=this.checked,i=this.loading,n=this.disabled;return t("div",{class:j({on:e,loading:i,disabled:n}),attrs:{role:"switch","aria-checked":String(e)},style:this.style,on:{click:this.onClick}},[t("div",{class:j("node")},[this.genLoading()])])}}),O=i("565f");a["a"].use(o["a"]),a["a"].use(l["a"]),a["a"].use(c["a"]),a["a"].use(u["a"]),a["a"].use(k),a["a"].use(F),a["a"].use(O["a"]);var R={data:function(){return{user:s["a"].getUser(),ignoreRead:"true"==window.localStorage.getItem("ignoreRead"),showImport:!1,rssUrl:null,baseUrl:s["a"].baseUrl}},methods:{onIgnoreReadChange:function(){window.localStorage.setItem("ignoreRead",String(this.ignoreRead))},subscribe:function(){fetch("".concat(s["a"].baseUrl,"rss/subscribe"),{body:JSON.stringify({UserId:this.user.id,Feeds:[this.rssUrl]}),method:"POST",headers:{Authorization:"Bearer ".concat(this.user.token),"Content-Type":"application/json"}}).then(function(t){if(200==t.status)return t.json();401==t.status&&this.logout()}.bind(this)).then(function(t){var e=s["a"].checkResponse(t);e.result?window.location.href="#/articles?id=".concat(t.data[0].id,"&name=").concat(t.data[0].title):Object(u["a"])({message:e.msg})}.bind(this))},logout:function(){window.localStorage.removeItem("user"),window.location.href="./"}}},B=R,T=(i("3e4b"),i("2877")),E=Object(T["a"])(B,n,r,!1,null,null,null);e["default"]=E.exports},"3e4b":function(t,e,i){"use strict";var n=i("bf50"),r=i.n(n);r.a},"565f":function(t,e,i){"use strict";var n=i("2638"),r=i.n(n),s=i("c31d"),a=i("a142");function o(){return!a["g"]&&/ios|iphone|ipad|ipod/.test(navigator.userAgent.toLowerCase())}var l=i("a8c1"),c=o();function u(){c&&Object(l["c"])(Object(l["a"])())}var h=i("482d"),d=i("1325"),f=i("d282"),g=i("ea8e"),v=i("ad06"),p=i("7744"),m=i("dfaf"),b=Object(f["a"])("field"),y=b[0],k=b[1];e["a"]=y({inheritAttrs:!1,provide:function(){return{vanField:this}},inject:{vanForm:{default:null}},props:Object(s["a"])({},m["a"],{name:String,rules:Array,disabled:Boolean,readonly:Boolean,autosize:[Boolean,Object],leftIcon:String,rightIcon:String,clearable:Boolean,formatter:Function,maxlength:[Number,String],labelWidth:[Number,String],labelClass:null,labelAlign:String,inputAlign:String,placeholder:String,errorMessage:String,errorMessageAlign:String,showWordLimit:Boolean,value:{type:[String,Number],default:""},type:{type:String,default:"text"},error:{type:Boolean,default:null},colon:{type:Boolean,default:null},clearTrigger:{type:String,default:"focus"},formatTrigger:{type:String,default:"onChange"}}),data:function(){return{focused:!1,validateFailed:!1,validateMessage:""}},watch:{value:function(){this.updateValue(this.value),this.resetValidation(),this.validateWithTrigger("onChange"),this.$nextTick(this.adjustSize)}},mounted:function(){this.updateValue(this.value,this.formatTrigger),this.$nextTick(this.adjustSize),this.vanForm&&this.vanForm.addField(this)},beforeDestroy:function(){this.vanForm&&this.vanForm.removeField(this)},computed:{showClear:function(){if(this.clearable&&!this.readonly){var t=Object(a["c"])(this.value)&&""!==this.value,e="always"===this.clearTrigger||"focus"===this.clearTrigger&&this.focused;return t&&e}},showError:function(){return null!==this.error?this.error:!!(this.vanForm&&this.vanForm.showError&&this.validateFailed)||void 0},listeners:function(){return Object(s["a"])({},this.$listeners,{blur:this.onBlur,focus:this.onFocus,input:this.onInput,click:this.onClickInput,keypress:this.onKeypress})},labelStyle:function(){var t=this.getProp("labelWidth");if(t)return{width:Object(g["a"])(t)}},formValue:function(){return this.children&&(this.$scopedSlots.input||this.$slots.input)?this.children.value:this.value}},methods:{focus:function(){this.$refs.input&&this.$refs.input.focus()},blur:function(){this.$refs.input&&this.$refs.input.blur()},runValidator:function(t,e){return new Promise((function(i){var n=e.validator(t,e);if(Object(a["f"])(n))return n.then(i);i(n)}))},isEmptyValue:function(t){return Array.isArray(t)?!t.length:!t},runSyncRule:function(t,e){return(!e.required||!this.isEmptyValue(t))&&!(e.pattern&&!e.pattern.test(t))},getRuleMessage:function(t,e){var i=e.message;return Object(a["d"])(i)?i(t,e):i},runRules:function(t){var e=this;return t.reduce((function(t,i){return t.then((function(){if(!e.validateFailed){var t=e.formValue;return i.formatter&&(t=i.formatter(t,i)),e.runSyncRule(t,i)?i.validator?e.runValidator(t,i).then((function(n){!1===n&&(e.validateFailed=!0,e.validateMessage=e.getRuleMessage(t,i))})):void 0:(e.validateFailed=!0,void(e.validateMessage=e.getRuleMessage(t,i)))}}))}),Promise.resolve())},validate:function(t){var e=this;return void 0===t&&(t=this.rules),new Promise((function(i){t||i(),e.runRules(t).then((function(){e.validateFailed?i({name:e.name,message:e.validateMessage}):i()}))}))},validateWithTrigger:function(t){if(this.vanForm&&this.rules){var e=this.vanForm.validateTrigger===t,i=this.rules.filter((function(i){return i.trigger?i.trigger===t:e}));this.validate(i)}},resetValidation:function(){this.validateMessage&&(this.validateFailed=!1,this.validateMessage="")},updateValue:function(t,e){void 0===e&&(e="onChange"),t=Object(a["c"])(t)?String(t):"";var i=this.maxlength;if(Object(a["c"])(i)&&t.length>i&&(t=t.slice(0,i)),"number"===this.type||"digit"===this.type){var n="number"===this.type;t=Object(h["a"])(t,n)}this.formatter&&e===this.formatTrigger&&(t=this.formatter(t));var r=this.$refs.input;r&&t!==r.value&&(r.value=t),t!==this.value&&this.$emit("input",t),this.currentValue=t},onInput:function(t){t.target.composing||this.updateValue(t.target.value)},onFocus:function(t){this.focused=!0,this.$emit("focus",t),this.readonly&&this.blur()},onBlur:function(t){this.focused=!1,this.updateValue(this.value,"onBlur"),this.$emit("blur",t),this.validateWithTrigger("onBlur"),u()},onClick:function(t){this.$emit("click",t)},onClickInput:function(t){this.$emit("click-input",t)},onClickLeftIcon:function(t){this.$emit("click-left-icon",t)},onClickRightIcon:function(t){this.$emit("click-right-icon",t)},onClear:function(t){Object(d["c"])(t),this.$emit("input",""),this.$emit("clear",t)},onKeypress:function(t){var e=13;if(t.keyCode===e){var i=this.getProp("submitOnEnter");i||"textarea"===this.type||Object(d["c"])(t),"search"===this.type&&this.blur()}this.$emit("keypress",t)},adjustSize:function(){var t=this.$refs.input;if("textarea"===this.type&&this.autosize&&t){t.style.height="auto";var e=t.scrollHeight;if(Object(a["e"])(this.autosize)){var i=this.autosize,n=i.maxHeight,r=i.minHeight;n&&(e=Math.min(e,n)),r&&(e=Math.max(e,r))}e&&(t.style.height=e+"px")}},genInput:function(){var t=this.$createElement,e=this.type,i=this.slots("input"),n=this.getProp("inputAlign");if(i)return t("div",{class:k("control",[n,"custom"]),on:{click:this.onClickInput}},[i]);var a={ref:"input",class:k("control",n),domProps:{value:this.value},attrs:Object(s["a"])({},this.$attrs,{name:this.name,disabled:this.disabled,readonly:this.readonly,placeholder:this.placeholder}),on:this.listeners,directives:[{name:"model",value:this.value}]};if("textarea"===e)return t("textarea",r()([{},a]));var o,l=e;return"number"===e&&(l="text",o="decimal"),"digit"===e&&(l="tel",o="numeric"),t("input",r()([{attrs:{type:l,inputmode:o}},a]))},genLeftIcon:function(){var t=this.$createElement,e=this.slots("left-icon")||this.leftIcon;if(e)return t("div",{class:k("left-icon"),on:{click:this.onClickLeftIcon}},[this.slots("left-icon")||t(v["a"],{attrs:{name:this.leftIcon,classPrefix:this.iconPrefix}})])},genRightIcon:function(){var t=this.$createElement,e=this.slots,i=e("right-icon")||this.rightIcon;if(i)return t("div",{class:k("right-icon"),on:{click:this.onClickRightIcon}},[e("right-icon")||t(v["a"],{attrs:{name:this.rightIcon,classPrefix:this.iconPrefix}})])},genWordLimit:function(){var t=this.$createElement;if(this.showWordLimit&&this.maxlength){var e=(this.value||"").length;return t("div",{class:k("word-limit")},[t("span",{class:k("word-num")},[e]),"/",this.maxlength])}},genMessage:function(){var t=this.$createElement;if(!this.vanForm||!1!==this.vanForm.showErrorMessage){var e=this.errorMessage||this.validateMessage;if(e){var i=this.getProp("errorMessageAlign");return t("div",{class:k("error-message",i)},[e])}}},getProp:function(t){return Object(a["c"])(this[t])?this[t]:this.vanForm&&Object(a["c"])(this.vanForm[t])?this.vanForm[t]:void 0},genLabel:function(){var t=this.$createElement,e=this.getProp("colon")?":":"";return this.slots("label")?[this.slots("label"),e]:this.label?t("span",[this.label+e]):void 0}},render:function(){var t,e=arguments[0],i=this.slots,n=this.getProp("labelAlign"),r={icon:this.genLeftIcon},s=this.genLabel();s&&(r.title=function(){return s});var a=this.slots("extra");return a&&(r.extra=function(){return a}),e(p["a"],{attrs:{icon:this.leftIcon,size:this.size,center:this.center,border:this.border,isLink:this.isLink,required:this.required,clickable:this.clickable,titleStyle:this.labelStyle,valueClass:k("value"),titleClass:[k("label",n),this.labelClass],arrowDirection:this.arrowDirection},scopedSlots:r,class:k((t={error:this.showError,disabled:this.disabled},t["label-"+n]=n,t["min-height"]="textarea"===this.type&&!this.autosize,t)),on:{click:this.onClick}},[e("div",{class:k("body")},[this.genInput(),this.showClear&&e(v["a"],{attrs:{name:"clear"},class:k("clear"),on:{touchstart:this.onClear}}),this.genRightIcon(),i("button")&&e("div",{class:k("button")},[i("button")])]),this.genWordLimit(),this.genMessage()])}})},"65f0":function(t,e,i){var n=i("861d"),r=i("e8b5"),s=i("b622"),a=s("species");t.exports=function(t,e){var i;return r(t)&&(i=t.constructor,"function"!=typeof i||i!==Array&&!r(i.prototype)?n(i)&&(i=i[a],null===i&&(i=void 0)):i=void 0),new(void 0===i?Array:i)(0===e?0:e)}},"6b41":function(t,e,i){"use strict";var n=i("d282"),r=i("b1d2"),s=i("ad06"),a=Object(n["a"])("nav-bar"),o=a[0],l=a[1];e["a"]=o({props:{title:String,fixed:Boolean,zIndex:[Number,String],leftText:String,rightText:String,leftArrow:Boolean,placeholder:Boolean,border:{type:Boolean,default:!0}},data:function(){return{height:null}},mounted:function(){this.placeholder&&this.fixed&&(this.height=this.$refs.navBar.getBoundingClientRect().height)},methods:{genLeft:function(){var t=this.$createElement,e=this.slots("left");return e||[this.leftArrow&&t(s["a"],{class:l("arrow"),attrs:{name:"arrow-left"}}),this.leftText&&t("span",{class:l("text")},[this.leftText])]},genRight:function(){var t=this.$createElement,e=this.slots("right");return e||(this.rightText?t("span",{class:l("text")},[this.rightText]):void 0)},genNavBar:function(){var t,e=this.$createElement;return e("div",{ref:"navBar",style:{zIndex:this.zIndex},class:[l({fixed:this.fixed}),(t={},t[r["a"]]=this.border,t)]},[e("div",{class:l("left"),on:{click:this.onClickLeft}},[this.genLeft()]),e("div",{class:[l("title"),"van-ellipsis"]},[this.slots("title")||this.title]),e("div",{class:l("right"),on:{click:this.onClickRight}},[this.genRight()])])},onClickLeft:function(t){this.$emit("click-left",t)},onClickRight:function(t){this.$emit("click-right",t)}},render:function(){var t=arguments[0];return this.placeholder&&this.fixed?t("div",{class:l("placeholder"),style:{height:this.height+"px"}},[this.genNavBar()]):this.genNavBar()}})},8418:function(t,e,i){"use strict";var n=i("c04e"),r=i("9bf2"),s=i("5c6c");t.exports=function(t,e,i){var a=n(e);a in t?r.f(t,a,s(0,i)):t[a]=i}},"99af":function(t,e,i){"use strict";var n=i("23e7"),r=i("d039"),s=i("e8b5"),a=i("861d"),o=i("7b0b"),l=i("50c4"),c=i("8418"),u=i("65f0"),h=i("1dde"),d=i("b622"),f=i("2d00"),g=d("isConcatSpreadable"),v=9007199254740991,p="Maximum allowed index exceeded",m=f>=51||!r((function(){var t=[];return t[g]=!1,t.concat()[0]!==t})),b=h("concat"),y=function(t){if(!a(t))return!1;var e=t[g];return void 0!==e?!!e:s(t)},k=!m||!b;n({target:"Array",proto:!0,forced:k},{concat:function(t){var e,i,n,r,s,a=o(this),h=u(a,0),d=0;for(e=-1,n=arguments.length;e<n;e++)if(s=-1===e?a:arguments[e],y(s)){if(r=l(s.length),d+r>v)throw TypeError(p);for(i=0;i<r;i++,d++)i in s&&c(h,d,s[i])}else{if(d>=v)throw TypeError(p);c(h,d++,s)}return h.length=d,h}})},bf50:function(t,e,i){},e8b5:function(t,e,i){var n=i("c6b6");t.exports=Array.isArray||function(t){return"Array"==n(t)}}}]);
//# sourceMappingURL=chunk-3290b8e0.1e616ce3.js.map