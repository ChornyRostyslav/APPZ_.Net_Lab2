using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace APPZ_Lab_2
{
    public class WeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(string apiKey)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
        }

        public async Task<string> GetWeatherAsync(string city)
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={city}&appid={_apiKey}&units=metric";
            var response = await _httpClient.GetFromJsonAsync<WeatherResponse>(url);

            if (response != null)
            {
                return $"Weather in {city}: {response.Main.Temp}°C, {response.Weather[0].Description}";
            }
            return "Could not retrieve weather data.";
        }
    }

    public class WeatherResponse
    {
        public WeatherInfo[] Weather { get; set; }
        public MainInfo Main { get; set; }
    }

    public class WeatherInfo
    {
        public string Description { get; set; }
    }

    public class MainInfo
    {
        public float Temp { get; set; }
    }
}
