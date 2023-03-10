using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using WeatherAPI.WeatherDomain.Dtos;
using WeatherAPI.WeatherDomain.Models;

namespace WeatherAPI.WeatherDomain.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfiguration _configuration;
        private static List<User> _users = new List<User>();

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<ResponseDto<User>> Register(UserDto request)
        {
            var ToCheckIfEmailAlreadlyExist = _users.Where(x => x.Email == request.Email)
                .FirstOrDefault();
            if (ToCheckIfEmailAlreadlyExist is not null)
            {
                return ResponseDto<User>.Fail("This Email already exist", (int)HttpStatusCode.BadRequest);
            }
            ToCreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "User" 
            };
            _users.Add(user);
            return ResponseDto<User>.Success("You have successfully Registered", user, (int)HttpStatusCode.OK);
        }
        public async Task<ResponseDto<User>> RegisterAdmin(UserDto request)
        {
            var ToCheckIfEmailAlreadlyExist = _users.Where(x => x.Email == request.Email)
                .FirstOrDefault();
            if (ToCheckIfEmailAlreadlyExist is not null)
            {
                return ResponseDto<User>.Fail("This Email already exist", (int)HttpStatusCode.BadRequest);
            }
            ToCreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "Admin"
            };
            _users.Add(user);
            return ResponseDto<User>.Success("You have successfully Registered", user, (int)HttpStatusCode.OK);
        }
        public async Task<ResponseDto<string>> Login(UserDto request)
        {
            var user = _users.Where(z => z.Email == request.Email).FirstOrDefault();
            if(user == null)
            {
                return ResponseDto<string>.Fail("User does not exist", (int)HttpStatusCode.BadRequest);
            }
            if(!ToVerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            {
                return ResponseDto<string>.Fail("Password Incorrect", (int)HttpStatusCode.BadRequest);
            }
            string token = ToCreateToken(user, user.Role);
            return ResponseDto<string>.Success("You've successfully Logged in", token, (int)HttpStatusCode.OK);
        }

        private string ToCreateToken(User user, string role)
        {
            List<Claim> claims = new List<Claim>
           {
               new Claim(ClaimTypes.Name, user.Email),
               new Claim(ClaimTypes.Role, role)
           };
            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes
                (_configuration.GetSection("Jwt:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);
            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private bool ToVerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

        private void ToCreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
