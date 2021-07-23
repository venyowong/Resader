# Resader
Resader 是一个 RSS 阅读器，程序包含了 web 前端页面和服务端，提供了 RSS 抓取接口，可供自定义设置定时任务，比如使用 crontab 定时调用接口。

## 快速启动
### 使用已打包好的程序包
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 根据系统环境下载对应 Release 包
3. 修改 appsettings.Production.json 中的数据库连接字符串
4. 运行 ./Resader 或 .\Resader.exe

### 本地环境
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 安装 [dotnet core SDK](https://dotnet.microsoft.com/)
3. 修改 appsettings.Production.json 中的数据库连接字符串
4. `dotnet run`
5. 打开浏览器访问 http://localhost:7854

注：src 目录下的前端页面文件不再更新，若要获取到最新的页面，请安装 npm 后，在 vue/mobile、vue/web 中分别执行 `npm install`，然后依次执行 build_web.bat、build_mobile.bat、build_wwwroot.bat

### 服务器环境
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 在本地环境打包

    `dotnet publish -r linux-x64 /p:PublishSingleFile=true -c Release -o publish`
3. 将打包出来的文件全部拷贝到服务器上，修改 appsettings.Production.json 中的数据库连接字符串
4. 
    ```
    sudo chomod 777 *(这一步非必需)
    sudo ./Resader
    ```
5. 访问 http://localhost:7854
6. 创建 ~/resader_fetch.sh 文件，加入以下命令 `curl -X post -H 'Accept: application/json' http://localhost:7854/fetcher/fetch`
7. 添加 crontab 配置 `* */5 * * * sh ~/resader_fetch.sh`

### 开发测试
1. 注册

curl -X POST "http://localhost:5303/User/SignUp" -H "accept: text/plain" -H "Content-Type: application/json" -d "{ \"mail\": \"test@test.com\", \"password\": \"123456\"}"
```
{
  "data": {
    "id": "b9742470cc7b4db19bc23f7d1ca987a3",
    "mail": "test@test.com",
    "token": "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyaWQiOiJiOTc0MjQ3MGNjN2I0ZGIxOWJjMjNmN2QxY2E5ODdhMyIsIm1haWwiOiJ0ZXN0QHRlc3QuY29tIn0.aJ8O98dT9-J0MZb2B1cmzfX-ER113uC11Wgpxmmg4VA"
  },
  "code": 0,
  "message": null
}
```
2. 订阅

curl -X POST "http://localhost:5303/RSS/Subscribe" -H "accept: text/plain" -H "Content-Type: application/json" -H "token: eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJ1c2VyaWQiOiJiOTc0MjQ3MGNjN2I0ZGIxOWJjMjNmN2QxY2E5ODdhMyIsIm1haWwiOiJ0ZXN0QHRlc3QuY29tIn0.aJ8O98dT9-J0MZb2B1cmzfX-ER113uC11Wgpxmmg4VA" -d "[ \"https://venyo.cn/rsshub/bilibili/bangumi/media/28231812\"]"