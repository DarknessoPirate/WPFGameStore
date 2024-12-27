using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

namespace WPFGameShop.Tools
{
    public class Geolocation
    {
        public string ip { get; set; }
        public string city { get; set; }

    }

    public class GeolocationService
    {
        private readonly HttpClient _httpClient;

        public GeolocationService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<string> GetGeolocationAsync()
        {
            // Using ipinfo.io API to get geolocation data
            var response = await _httpClient.GetAsync("https://ipinfo.io/json");
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var geolocation = JsonSerializer.Deserialize<Geolocation>(json);

            // Parsing location from 'loc' field if available
            if (!string.IsNullOrEmpty(geolocation.city))
            {
                return geolocation.city.ToString();
            }

            return "Szczytno";
        }
    }

}