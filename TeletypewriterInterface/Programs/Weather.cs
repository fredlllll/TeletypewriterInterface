using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace TeletypewriterInterface.Programs
{
    public static class Weather
    {
        const string weatherUrl = "https://api.open-meteo.com/v1/forecast?latitude=48.1374&longitude=11.5755&hourly=precipitation_probability&current=temperature_2m,cloud_cover,rain,showers,snowfall,relative_humidity_2m&forecast_days=1";

        class CurrentWeatherResponse
        {
            public string time = ""; // 2025-04-18T16:30
            public float temperature_2m = 0; //°C
            public int cloud_cover = 0; // %
            public float rain = 0; //mm
            public float shower = 0; //mm
            public float snowfall = 0; //cm (!!!)
            public int relative_humidity_2m = 0; //%
        }

        class HourlyWeatherResponse
        {
            public string[] time = [];
            public int[] precipitation_probability = [];
        }

        class WeatherResponse
        {
            public CurrentWeatherResponse current = new();
            //for some stupid reason there is no current precipitation probability in this api, so have to extract it from hourly
            public HourlyWeatherResponse hourly = new();
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
                    var prec_prob = r.hourly.precipitation_probability[DateTime.Now.Hour];
                    TeleIO.WriteOut($"temperatur: {r.current.temperature_2m}C    luftfeuchtigkeit: {r.current.relative_humidity_2m}%\r\n");
                    TeleIO.WriteOut($"niederschlagswarscheinlichkeit: {prec_prob}%    wolkendecke: {r.current.cloud_cover}%\r\n");
                    TeleIO.WriteOut($"regen: {r.current.rain + r.current.shower}mm    schnee: {r.current.snowfall}cm\r\n\n");
                }
                else
                {
                    TeleIO.WriteOut("antwort war leer\r\n\n");
                }
            }
        }
    }
}
