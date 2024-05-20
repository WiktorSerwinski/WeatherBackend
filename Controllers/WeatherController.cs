using API;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Globalization;

namespace WeatherApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {

        private readonly HttpClient _httpClient;

        float installationPower = 2.5f; // Moc instalacji fotowoltaicznej w kW
                    
        float panelEfficiency = 0.2f; // Efektywność paneli

        public WeatherController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetWeatherForecast(float latitude, float longitude)
        {
            
            try
            {
                // Sprawdzenie czy dane wejściowe to liczby
                if (float.IsNaN(latitude) || float.IsNaN(longitude))
                {
                    return BadRequest("Współrzędne geograficzne muszą być liczbami.");
                }

                // Sprawdzenie zakresu szerokości geograficznej
                if (latitude < -90 || latitude > 90)
                {
                    return BadRequest("Szerokość geograficzna musi być pomiędzy -90 a 90 stopni.");
                }

                // Sprawdzenie zakresu długości geograficznej
                if (longitude < -180 || longitude > 180)
                {
                    return BadRequest("Długość geograficzna musi być pomiędzy -180 a 180 stopni.");
                }
                
                string latitudeFormatted = latitude.ToString("G", CultureInfo.InvariantCulture);
                string longitudeFormatted = longitude.ToString("G", CultureInfo.InvariantCulture);
                string apiUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latitudeFormatted}&longitude={longitudeFormatted}&daily=weather_code,temperature_2m_max,temperature_2m_min,sunshine_duration&timezone=Europe%2FBerlin";
                string apiString = apiUrl.ToString();
                HttpResponseMessage response = await _httpClient.GetAsync(apiString);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var mateoResponse = JsonSerializer.Deserialize<Daily>(jsonResponse);

                    var weatherResponses = new List<WeatherResponse>();

                    if(mateoResponse != null)
                    for(int i = 0;i<7;i++)
                    {
                        float sunshineDurationInSeconds = mateoResponse.DailyData.SunshineDuration[i];
                        
                        float generatedEnergy = installationPower * sunshineDurationInSeconds / 3600 * panelEfficiency;
                        
                        var weatherResponse = new WeatherResponse{
                        
                        Time = mateoResponse.DailyData.Time[i],
                        WeatherCode = mateoResponse.DailyData.WeatherCode[i],
                        Temperature2mMax = mateoResponse.DailyData.Temperature2mMax[i],
                        Temperature2mMin = mateoResponse.DailyData.Temperature2mMin[i],
                        generatedEnergy = generatedEnergy
                        };
                        weatherResponses.Add(weatherResponse);
                    }
                    

                    
                    

                    

                    



                    if (mateoResponse != null)
                    {
                        return Ok(weatherResponses);
                    }
                    else
                    {
                        return StatusCode(500, "Błąd deserializacji danych pogodowych.");
                    }

                }

                else
                {
                    return StatusCode((int)response.StatusCode, "Błąd podczas pobierania danych z zewnętrznego API.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Wystąpił błąd: {ex.Message}");
            }
        }


    }
}
