using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace API
{
    public class DailyData
    {
        [JsonPropertyName("time")]
        public List<DateTime> Time { get; set; }

        [JsonPropertyName("weather_code")]
        public List<int> WeatherCode { get; set; }

        [JsonPropertyName("temperature_2m_max")]
        public List<float> Temperature2mMax { get; set; }

        [JsonPropertyName("temperature_2m_min")]
        public List<float> Temperature2mMin { get; set; }

        [JsonPropertyName("sunshine_duration")]
        public List<float> SunshineDuration { get; set; }
    }
    public class Daily
    {
        [JsonPropertyName("daily")]
        public DailyData DailyData { get; set; }
    }

    public class WeatherResponse
    {

        
        public DateTime Time { get; set; }
        public int WeatherCode { get; set; }

        public float Temperature2mMax { get; set; }
 
        public float Temperature2mMin { get; set; }

        public float generatedEnergy { get; set; }
    }
}