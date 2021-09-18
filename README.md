# Resader

Resader 是一个集成了后台定时数据抓取任务、API服务以及阅读器的RSS服务，本服务旨在实现一个轻量、可扩展的单体服务。

1. 轻量，Resader 最基本的部署要求为一个 mysql 数据库以及服务本身，两者可部署在1C1G的基础机型上，并且保证服务可正常运行
2. 可扩展，若访问量达到一定量级，Resader 可通过部署多个实例来分担访问压力，但需要注意的是，Resader 将后台定时任务与API接口放在一起开发，应该将后台定时任务配置为只有一个实例(参考 api 服务的 UseScheduler 配置，该配置表示是否启用定时任务服务)；当存在多个服务实例的时候，还需要部署 Redis 作为缓存方案，保证缓存数据的一致性，当 api 服务未配置 Redis 时会使用内存作为缓存方案

## 编译部署 api 服务

由于代码暂时还未稳定，因此暂不提供 release 包，可自行克隆源码本地进行编译。

1. 安装 [.net core](https://dotnet.microsoft.com/download)，请安装 .net 5.0 以上版本的 sdk
2. 安装 [mysql](https://dev.mysql.com/downloads/)
3. git clone https://gitee.com/venyowong/Resader & cd Resader/src/api
4. 复制 appsettings.Staging.json 改名为 appsettings.Production.json，并修改 mysql 连接字符串
5. dotnet run 当你看到以下日志输出，说明服务已成功运行
```
[2021-09-18 11:02:37 INF] [] Default Quartz.NET properties loaded from embedded resource file
[2021-09-18 11:02:37 INF] [] Initialized Scheduler Signaller of type: Quartz.Core.SchedulerSignalerImpl
[2021-09-18 11:02:37 INF] [] Quartz Scheduler v.3.2.4.0 created.
[2021-09-18 11:02:37 INF] [] RAMJobStore initialized.
[2021-09-18 11:02:37 INF] [] Scheduler meta-data: Quartz Scheduler (v3.2.4.0) 'DefaultQuartzScheduler' with instanceId 'NON_CLUSTERED'
  Scheduler class: 'Quartz.Core.QuartzScheduler' - running locally.
  NOT STARTED.
  Currently in standby mode.
  Number of jobs executed: 0
  Using thread pool 'Quartz.Simpl.DefaultThreadPool' - with 10 threads.
  Using job-store 'Quartz.Simpl.RAMJobStore' - which does not support persistence. and is not clustered.

[2021-09-18 11:02:37 INF] [] Quartz scheduler 'DefaultQuartzScheduler' initialized
[2021-09-18 11:02:37 INF] [] Quartz scheduler version: 3.2.4.0
[2021-09-18 11:02:37 INF] [] JobFactory set to: Resader.Api.Quartz.CustomJobFactory
[2021-09-18 11:02:37 INF] [] Scheduler DefaultQuartzScheduler_$_NON_CLUSTERED started.
[2021-09-18 11:02:37 INF] [] Trigger: Resader.AutoRecoveryJob_RightNow Job: Resader.AutoRecoveryJob TriggerFired
[2021-09-18 11:02:37 INF] [] Trigger: Resader.FetchJob_Trigger2 Job: Resader.FetchJob TriggerFired
[2021-09-18 11:02:37 INF] [] Trigger: Resader.SaveReadRecordJob_RightNow Job: Resader.SaveReadRecordJob TriggerFired
[2021-09-18 11:02:37 INF] [] Trigger: Resader.SaveReadRecordJob_RightNow Job: Resader.SaveReadRecordJob TriggerComplete Runtime: 00:00:00.2277788
[2021-09-18 11:02:37 INF] [] Trigger: Resader.AutoRecoveryJob_RightNow Job: Resader.AutoRecoveryJob TriggerComplete Runtime: 00:00:00.4092778
[2021-09-18 11:02:41 INF] [] Trigger: Resader.FetchJob_Trigger2 Job: Resader.FetchJob TriggerComplete Runtime: 00:00:04.6044165
[2021-09-18 11:02:42 INF] [] Scheduler DefaultQuartzScheduler_$_NON_CLUSTERED shutting down.
[2021-09-18 11:02:42 INF] [] Scheduler DefaultQuartzScheduler_$_NON_CLUSTERED paused.
[2021-09-18 11:02:42 INF] [] Scheduler DefaultQuartzScheduler_$_NON_CLUSTERED Shutdown complete.
```
6. 若想启用 redis 则在 mysql 的同层级下配置
```
"Redis": {
    "ConnectionString": "localhost",
    "DefaultDb": 0
}
```
7. 若想禁用定时任务，则在 appsettings.json 中将 UseScheduler 配置为 false
8. 若启用了 redis，并且有多个实例，想禁用其中部分实例的数据恢复任务，则在 appsettings.json 中将 AutoRecovery 配置为 false，若缓存策略使用的是内存，则一定要为 true
9. 若想要减少缓存空间，可以修改 appsettings.json 中的 ArticleMonths 配置，该配置表示文章数据月数，若配置为3，则只展示3个月内的文章，若配置为小于等于0，则展示全部数据

## 打包 PWA 页面

1. cd Resader/src/pwa
2. npm install
3. npm run build
4. 将 dist 放在 api 服务的 wwwroot 下或者挂载在 nginx 服务下

## 线上 PWA 链接

https://venyo.cn/resader/pwa/

注：由于该页面只适配了手机端，因此请在手机上打开或者使用浏览器的设备仿真功能进行浏览

## PWA 页面截图

![首页](/screenshots/首页.png "首页")

![精选](/screenshots/精选.png "精选")

![我](/screenshots/我.png "我")