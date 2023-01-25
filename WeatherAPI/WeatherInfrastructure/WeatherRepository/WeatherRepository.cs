using Newtonsoft.Json;
using System.Net;
using WeatherAPI.WeatherDomain.Dtos;
using WeatherAPI.WeatherDomain.Models;
using WeatherAPI.WeatherInfrastructure.WeatherRepository;
using static WeatherAPI.WeatherDomain.Models.Information;

namespace WeatherAPI.WeatherInfrastructure.WeatherRepo
{
    public class WeatherRepository: IWeatherRepository
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public WeatherRepository(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<ResponseDto<InformationDto>> GetCurrentWeatherAsync(string location)

        {
            if (string.IsNullOrEmpty(location))
            {
                return ResponseDto<InformationDto>.Fail("Please insert Location", (int)HttpStatusCode.BadRequest);
            }
            var response = await _httpClient.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={_configuration.GetConnectionString("ApiKey")}");
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var weather = JsonConvert.DeserializeObject<root>(json);
                var weatherDto = new InformationDto()
                {
                    Temperature = weather.main.temp,
                    Summary = weather.weather[0].main,
                    Details = weather.weather[0].description,
                    Pressure = weather.main.pressure.ToString(),
                    Humidity = weather.main.humidity.ToString(),
                    Sunrise = ConvertDateTime(weather.sys.sunrise).ToString(),
                    Sunset = ConvertDateTime(weather.sys.sunset).ToString(),
                    Icon = $"https://api.openweathermap.org/img/w/{weather.weather[0].icon}.png",
                };
                return ResponseDto<InformationDto>.Success("Successful", weatherDto, (int)HttpStatusCode.OK);
            }
            else
            {
                var json = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<User>(json);
                return ResponseDto<InformationDto>.Fail("Location does not exist", (int)HttpStatusCode.NotFound);
            }
        }

        private DateTime ConvertDateTime(long millisec)
        {
            var day = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
               .AddSeconds(millisec).ToLocalTime();
            return day;
        }
    }
}
