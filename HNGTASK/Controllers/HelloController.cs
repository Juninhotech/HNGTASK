﻿using HNGTASK.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Text.Json;

namespace HNGTASK.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _openWeatherApiKey;

        public HelloController(IOptions<OpenWeatherOption> openWeatherOptions, IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
            _openWeatherApiKey = openWeatherOptions.Value.ApiKey;
        }

        [HttpGet]
        public async Task<DataResponse> Get([FromQuery] string visitorsName)
        {
            DataResponse response = new DataResponse();
            try
            {
                var temperature = "Unknown";
                var location = "unknown";
                var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";

                using (var httClient = _httpClientFactory.CreateClient())
                {
                    var locationUrl = $"http://api.openweathermap.org/geo/1.0/direct?q={clientIp}&appid={_openWeatherApiKey}";
                    var locationResponse = await httClient.GetStringAsync(locationUrl);
                    var locationData = JArray.Parse(locationResponse).FirstOrDefault();

                    if (locationData != null)
                    {
                        location = locationData["name"].ToString();
                        var lat = locationData["lat"].ToString();
                        var lon = locationData["lon"].ToString();


                        var weatherUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={_openWeatherApiKey}";
                        var weatherResponse = await httClient.GetStringAsync(weatherUrl);
                        var weatherData = JObject.Parse(weatherResponse);
                        temperature = weatherData["main"]["temp"].ToString();
                    }

                }


                response.client_ip = clientIp;
                response.location = location;
                response.greeting = $"Hello {visitorsName}, your temperature is {temperature} in {response.location}";
            }
            catch (Exception)
            {


            }
            return response;
        }

        //[HttpGet]
        //public async Task<DataResponse> Get([FromQuery] string visitors_Name)
        //{
        //    DataResponse response = new DataResponse();
        //    try
        //    {
        //        var temperature = "Unknown";
        //        var location = "unknown";
        //        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "8.8.8.8";


        //        using (var httClient = _httpClientFactory.CreateClient())
        //        {
        //            var query = $"http://ip-api.com/json/{clientIp}";
        //            var locationResponse = await httClient.GetStringAsync(query);
        //            var locationData = JsonDocument.Parse(locationResponse).RootElement;

        //            if (locationData.TryGetProperty("lat", out var latElement) && locationData.TryGetProperty("lon", out var lonElement))
        //            {
        //                var lat = latElement.GetDouble();
        //                var lon = lonElement.GetDouble();
        //                location = locationData.GetProperty("city").GetString() ?? "Unknown";

        //                var weatherUrl = $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&appid={_openWeatherApiKey}";
        //                var weatherResponse = await httClient.GetStringAsync(weatherUrl);
        //                var weatherData = JObject.Parse(weatherResponse);
        //                temperature = weatherData["main"]["temp"].ToString();
        //            }

        //        }


        //        response.client_ip = clientIp;
        //        response.location = location;
        //        response.greeting = $"Hello, {visitors_Name}!, the temperature is {temperature} degree Celcius in {response.location}";
        //    }
        //    catch (Exception)
        //    {


        //    }
        //    return response;
        //}
    }
}