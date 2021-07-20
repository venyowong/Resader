namespace Resader.Api
{
    public class Configuration
    {
        public LoggingConfig Logging { get; set; }

        public RedisConfig Redis { get; set; }

        public MySqlConfig MySql { get; set; }

        /// <summary>
        /// �Ƿ�ÿ�� Scheduler ���������� AutoRecoveryJob
        /// </summary>
        public bool AutoRecovery { get; set; }
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
}