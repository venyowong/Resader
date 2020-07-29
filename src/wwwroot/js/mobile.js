function toast(message) {
  app.$toast({
    message: message,
    position: 'bottom',
    duration: 2000
  });
}

function checkResponse(response, codeHandler) {
    if (!response) {
      toast('请求异常，响应数据为空');
      return false;
    }
  
    switch (response.code) {
      case 0:
        return true;
      default:
        if (response.message) {
          toast(response.message);
        }
        else {
          var msg = '';
          if (codeHandler) {
            msg = codeHandler(response.code);
          }
          if (msg) {
            toast(msg);
          }
          else {
            toast(defaultCodeHandler(response.code));
          }
        }
        return false;
    }
  }
  
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

function getQueryStringByName(name){
    var result = location.search.match(new RegExp("[\?\&]" + name+ "=([^\&]+)","i"));
    if(result == null || result.length < 1){
        return "";
    }

    return result[1];
}

