namespace ERP.GC.Presentation.Configuration
{
    public class JwtConfigSettings
    {
        public string SecretKey { get; set; }
        public string Audience { get; set; }
        public string Issuer { get; set; }
    }
}
