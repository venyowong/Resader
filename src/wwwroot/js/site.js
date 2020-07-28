function checkResponse(response, codeHandler) {
  if (!response) {
    app.$message("请求异常，响应数据为空");
    return false;
  }

  switch (response.code) {
    case 0:
      return true;
    default:
      if (response.message) {
        app.$message(response.message);
      }
      else {
        var msg = '';
        if (codeHandler) {
          msg = codeHandler(response.code);
        }
        if (msg) {
          app.$message(msg);
        }
        else {
          app.$message(defaultCodeHandler(response.code));
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

if (screen.width <= 650) {
    window.location.href = "./mobile/index.html";
}