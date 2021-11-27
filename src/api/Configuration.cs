namespace Resader.Api;

public class Configuration
{
    public LoggingConfig Logging { get; set; }

    public RedisConfig Redis { get; set; }

    public MySqlConfig MySql { get; set; }

    /// <summary>
    /// 是否启用数据恢复任务
    /// </summary>
    public bool AutoRecovery { get; set; }

    /// <summary>
    /// 文章数据月数，若配置为3，则只展示3个月内的文章，若配置为小于等于0，则展示全部数据
    /// </summary>
    public int ArticleMonths { get; set; }
}

public class LoggingConfig
{
    public SerilogConfig Serilog { get; set; }
}

public class SerilogConfig
{
    public string File { get; set; }
}

public class RedisConfig
{
    public string ConnectionString { get; set; }

    public int DefaultDb { get; set; }
}

public class MySqlConfig
{
    public string ConnectionString { get; set; }
}
