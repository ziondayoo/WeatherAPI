namespace WeatherAPI.WeatherDomain.Models
{
        public class User
        {
            public Guid Id { get; set; }
            public string Email { get; set; } = string.Empty;
            public byte[] PasswordHash { get; set; }
            public byte[] PasswordSalt { get; set; }
            public string RefreshToken { get; set; } = string.Empty;
            public DateTime TokenCreated { get; set; }
            public DateTime TokenExpires { get; set; }
        }
    }



