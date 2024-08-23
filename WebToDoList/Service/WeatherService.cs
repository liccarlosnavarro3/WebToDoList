using Newtonsoft.Json;

namespace WebToDoList.Service
{
    public class WeatherService
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _city;

        public WeatherService(IConfiguration configuration)
        {
            _apiKey = configuration["WeatherService:ApiKey"];
            _apiUrl = configuration["WeatherService:ApiUrl"];
            _city = configuration["WeatherService:City"];
        }

        public async Task<WeatherResponse> GetWeatherAsync()
        {
            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync($"{_apiUrl}?q={_city}&appid={_apiKey}&units=metric");
                return JsonConvert.DeserializeObject<WeatherResponse>(response);
            }
            catch (HttpRequestException httpEx)
            {
                // Manejar errores de solicitud HTTP
                throw new Exception("Error al comunicarse con la API del clima. Inténtalo de nuevo más tarde.", httpEx);
            }
            catch (JsonException jsonEx)
            {
                // Manejar errores de deserialización JSON
                throw new Exception("Error al procesar la respuesta de la API del clima. Inténtalo de nuevo más tarde.", jsonEx);
            }
            catch (Exception ex)
            {
                // Manejar otras excepciones generales
                throw new Exception("Ocurrió un error inesperado al obtener el clima. Inténtalo de nuevo más tarde.", ex);
            }
        }

        public class WeatherResponse
        {
            public Main Main { get; set; }
            public string Name { get; set; }
        }

        public class Main
        {
            public float Temp { get; set; }
            public float Feels_Like { get; set; }
        }
    }
}
