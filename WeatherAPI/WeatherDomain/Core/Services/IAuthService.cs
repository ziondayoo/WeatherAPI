using WeatherAPI.WeatherDomain.Dtos;
using WeatherAPI.WeatherDomain.Models;

namespace WeatherAPI.WeatherDomain.Core.Services
{
    public class IAuthService
    {
        Task<ResponseDto<string>> Login(UserDto request);
        Task<ResponseDto<User>> Register(UserDto request);
    }
}
