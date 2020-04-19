namespace Resader.Host
{
    public class Configuration
    {
        public MySqlConfig MySql{get;set;}

        public NiologConfig Niolog{get;set;}

        public JwtConfig Jwt{get;set;}
    }

    public class MySqlConfig
    {
        public string ConnectionString{get;set;}
    }

    public class NiologConfig
    {
        public string File{get;set;}
    }

    public class JwtConfig
    {
        public string Secret{get;set;}

        public double Expiration{get;set;}
    }
}