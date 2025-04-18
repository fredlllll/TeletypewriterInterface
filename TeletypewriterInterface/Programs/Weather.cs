using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TeletypewriterInterface.Programs
{
    public static class Weather
    {
        const string weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=48.1374&longitude=11.5755&hourly=precipitation_probability&current=temperature_2m,cloud_cover,rain,showers,snowfall,relative_humidity_2m&forecast_days=1";

        class CurrentWeatherResponse
        {
            [JsonPropertyName("time")]
            public string Time { get; set; } = ""; // 2025-04-18T16:30
            [JsonPropertyName("temperature_2m")]
            public float Temperature2m { get; set; } = 0; //°C
            [JsonPropertyName("cloud_cover")]
            public int CloudCover { get; set; } = 0; // %
            [JsonPropertyName("rain")]
            public float Rain { get; set; } = 0; //mm
            [JsonPropertyName("shower")]
            public float Shower { get; set; } = 0; //mm
            [JsonPropertyName("snowfall")]
            public float Snowfall { get; set; } = 0; //cm (!!!)
            [JsonPropertyName("relative_humidity_2m")]
            public int RelativeHumidity2m { get; set; } = 0; //%

            public override string ToString()
            {
                return $"time: {Time}\ntemperature_2m: {Temperature2m}\ncloud_cover: {CloudCover}\nrain: {Rain}\nshower: {Shower}\n"+
                    $"snowfall: {Snowfall}\nrelative_humidity_2m: {RelativeHumidity2m}";
            }
        }

        class HourlyWeatherResponse
        {
            [JsonPropertyName("time")]
            public string[] Time { get; set; } = [];
            [JsonPropertyName("precipitation_probability")]
            public int[] PrecipitationProbability { get; set; } = [];
            public override string ToString()
            {
                return "time: [" + string.Join(", ", Time) + "]\n" +
                    "precipitation_probability: [" + string.Join(", ", PrecipitationProbability) + "]";
            }
        }

        class WeatherResponse
        {
            [JsonPropertyName("current")]
            public CurrentWeatherResponse Current { get; set; } = new();
            //for some stupid reason there is no current precipitation probability in this api, so have to extract it from hourly
            [JsonPropertyName("hourly")]
            public HourlyWeatherResponse Hourly { get; set; } = new();

            public override string ToString()
            {
                return Current.ToString() + "\n" + Hourly.ToString();
            }
        }

        public static void Run()
        {
            HttpClient client = new HttpClient();
            var task = client.GetFromJsonAsync<WeatherResponse>(weatherUrl);

            TeleIO.WriteOut("---wetter---\r\n");
            if (!task.IsCompletedSuccessfully)
            {
                TeleIO.WriteOut("wetter abfrage fehlgeschlagen\r\n\n");
            }
            else
            {
                if (task.Result != null)
                {
                    var r = task.Result;
                    var prec_prob = r.Hourly.PrecipitationProbability[DateTime.Now.Hour];
                    TeleIO.WriteOut($"temperatur: {r.Current.Temperature2m:0.0}C    luftfeuchtigkeit: {r.Current.RelativeHumidity2m}pc\r\n");
                    TeleIO.WriteOut($"niederschlagswarscheinlichkeit: {prec_prob}pc    wolkendecke: {r.Current.CloudCover}pc\r\n");
                    TeleIO.WriteOut($"regen: {(r.Current.Rain + r.Current.Shower):0.00}mm    schnee: {r.Current.Snowfall:0.00}cm\r\n\n");
                }
                else
                {
                    TeleIO.WriteOut("antwort war leer\r\n\n");
                }
            }
        }
    }
}
