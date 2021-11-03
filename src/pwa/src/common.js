function defaultCodeHandler(code) {
  switch(code) {
    case 1:
      return "参数异常";
    case 2:
      return "系统异常";
    default:
      return `请求失败(${code})`;
  }
}

function checkResponse(response, codeHandler) {
  if (!response) {
    return {result: false, msg: "请求异常，响应数据为空"};
  }

  switch (response.code) {
    case 0:
      return {result: true, msg: ""};
    default:
      if (response.message) {
        return {result: false, msg: response.message};
      }
      else {
        var msg = '';
        if (codeHandler) {
          msg = codeHandler(response.code);
        }
        if (msg) {
          return {result: false, msg: msg};
        }
        else {
          return {result: false, msg: defaultCodeHandler(response.code)};
        }
      }
  }
}

function getUser() {
  var user = localStorage.getItem("user");
  if (!user) {
    return null;
  }
  
  return JSON.parse(user);
}

function getQueryStringByName(name){
  var result = location.href.match(new RegExp("[?&]" + name+ "=([^&]+)","i"));
  if(result == null || result.length < 1){
      return "";
  }

  return result[1];
}

var baseUrl = "https://venyo.cn/resader/";

export default { checkResponse, getUser, getQueryStringByName, baseUrl }