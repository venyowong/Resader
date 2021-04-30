namespace Resader
{
    public class Configuration
    {
        public LoggingConfig Logging{get;set;}

        public RedisConfig Redis{get;set;}

        public JwtConfig Jwt{get;set;}
    }

    public class LoggingConfig
    {
        public SerilogConfig Serilog{get;set;}
    }

    public class SerilogConfig
    {
        public string File{get;set;}
    }

    public class RedisConfig
    {
        public string ConnectionString{get;set;}

        public int DefaultDb{get;set;}
    }

    public class JwtConfig
    {
        public string Secret{get;set;}

        public double Expiration{get;set;}
    }
}