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

        /// <summary>
        /// ��������������������Ϊ3����ֻչʾ3�����ڵ�����
        /// <para>������ΪС�ڵ���0����չʾȫ������</para>
        /// <para>��������Ҫ��Ϊ�˼��ٻ���������������Դ���������¿�ͨ�����ø��ֶ�����֤�������������</para>
        /// </summary>
        public int ArticleMonths { get; set; }

        /// <summary>
        /// oauth ��Ȩ��ĵ�¼ҳ���ַ
        /// </summary>
        public string OauthLoginUrl { get; set; }
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