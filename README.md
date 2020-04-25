# Resader
Resader 是一个 RSS 阅读器，程序包含了 web 前端页面和服务端，提供了 RSS 抓取接口，可供自定义设置定时任务，比如使用 crontab 定时调用接口。目前功能比较简陋，甚至没有忘记密码功能，主要是针对个人用户，但是服务端是基于 Orleans 编写的，理论上支持无限水平扩展，因此也许能够支持大量用户访问。

## 快速启动
### 本地环境
1. 安装 MySql，并使用 src/init.sql 脚本初始化
2. 安装 [dotnet core SDK](https://dotnet.microsoft.com/)
3. 修改 src/Host/appsettings.json 的配置项以满足需求
4. `dotnet run`
5. 打开浏览器访问 http://localhost:9090 或第三步配置的链接

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
6. 添加 crontab 配置 `* */5 * * * curl -X post -H 'Accept: application/json' http://localhost:9090/grains/fetcher/fetch`