using WeatherAPI.WeatherDomain.Dtos;

namespace WeatherAPI.WeatherInfrastructure.WeatherRepository
{
    public interface IWeatherRepository
    {
        Task<ResponseDto<InformationDto>> GetCurrentWeatherAsync(string location);
    }
}
