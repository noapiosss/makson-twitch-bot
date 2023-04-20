namespace Web.Models
{
    public class AccessTokenResponse
    {
        public string AccessToken { get; set; }
        public long ExpiresIn { get; set; }
    }
}