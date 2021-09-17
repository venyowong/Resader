- 若不配置 redis 则会使用内存作为缓存策略，该模式下仅支持启动一个服务实例
```
"Redis": {
    "ConnectionString": "localhost",
    "DefaultDb": 0
  }
```
- UseScheduler：是否启用定时任务服务，提示：同时只能存在一个定时任务服务
- AutoRecovery：是否每次服务启动都运行 AutoRecoveryJob，若缓存策略使用的是内存，则一定要为 true
- ArticleMonths：文章数据月数，若配置为3，则只展示3个月内的文章，若配置为小于等于0，则展示全部数据，该配置主要是为了减少缓存数据量，在资源不足的情况下可通过配置该字段来保证服务的正常运行