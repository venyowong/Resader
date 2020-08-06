# Resader
Resader 是一个 RSS 阅读器，程序包含了 web 前端页面和服务端，提供了 RSS 抓取接口，可供自定义设置定时任务，比如使用 crontab 定时调用接口。

## 快速启动
### 本地环境
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 安装 [dotnet core SDK](https://dotnet.microsoft.com/)
3. 修改 src/Host/appsettings.json 的配置项以满足需求
4. `dotnet run`
5. 打开浏览器访问 http://localhost:9090 或第三步配置的链接

注：src 目录下的前端页面文件不再更新，若要获取到最新的页面，请安装 npm 后，依次执行 build_web.bat、build_mobile.bat、build_wwwroot.bat

### 服务器环境
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 在本地环境打包

    `dotnet publish -r linux-x64 /p:PublishSingleFile=true -c Release -o publish`
3. 将打包出来的文件全部拷贝到服务器上，修改 appsettings.json 的配置项以满足需求
4. 
    ```
    sudo chomod 777 *(这一步非必需)
    sudo ./Host
    ```
5. 打开浏览器访问 http://localhost:9090 或第三步配置的链接
6. 创建 ~/resader_fetch.sh 文件，加入以下命令 `curl -X post -H 'Accept: application/json' http://localhost:9090/grains/fetcher/fetch`
7. 添加 crontab 配置 `* */5 * * * sh ~/resader_fetch.sh`
