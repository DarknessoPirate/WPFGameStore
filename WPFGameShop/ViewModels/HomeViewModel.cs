using WPFGameShop.Models;
using System.Net.Http;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;
using System.IO;
using WPFGameShop.Tools;
using System.Windows.Input;
using WPFGameShop.Repositories;
using Microsoft.Extensions.Configuration;

namespace WPFGameShop.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        private string _geolocationApiKey;
        public HomeViewModel(User user)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            IConfiguration config = builder.Build();
            _geolocationApiKey = config["ApiKey"]!;

            CurrentUser = user;
            CurrentDate = DateTime.Now.ToString("dd.MM.yyyy");
            CurrentTime = DateTime.Now.ToString("HH:mm");
            _geolocationService = new GeolocationService();

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMinutes(1)
            };
            _timer.Tick += TimerTick;
            _timer.Start();

            InitializeAsync();
        }

        #region properties
        private User _currentUser;
        private decimal _currentMoney;
        private string _currentDate;
        private string _currentTime;
        private string _city;
        private string _weatherCondition;
        private string _weatherImage;
        private double _temperature;
        private GeolocationService _geolocationService;
        private DispatcherTimer _timer;
        #endregion

        #region accessors
        public User CurrentUser {
            get { return _currentUser; }
            set 
            {
                _currentUser = value;
                CurrentMoney = _currentUser.Money;
                onPropertyChanged(nameof(CurrentUser)); 
            }
        }

        public decimal CurrentMoney
        {
            get { return _currentMoney; }
            set
            {
                _currentMoney = value;
                CurrentUser.Money = value;
                onPropertyChanged(nameof(CurrentMoney));
            }
        }

        public string CurrentDate
        {
            get
            {
                return _currentDate;
            }
            set
            {
                _currentDate = value;
                onPropertyChanged(nameof(CurrentDate));
            }
        }

        public string CurrentTime
        {
            get
            {
                return _currentTime;
            }
            set
            {
                _currentTime = value;
                onPropertyChanged(nameof(CurrentTime));
            }
        }

        public string WeatherCondition
        {
            get => _weatherCondition;
            set
            {
                _weatherCondition = value;
                onPropertyChanged(nameof(WeatherCondition));
            }
        }
        public string WeatherImage
        {
            get => _weatherImage;
            set
            {
                _weatherImage = value;
                onPropertyChanged(nameof(WeatherImage));
            }
        }

        public double Temperature
        {
            get => _temperature;
            set
            {
                _temperature = value;
                onPropertyChanged(nameof(Temperature));
            }
        }

        public string City
        {
            get => _city;
            set
            {
                _city = value;
                onPropertyChanged(nameof(City));
            }
        }


        #endregion

        #region functions
       
        private void TimerTick(object sender, EventArgs e)
        {
            CurrentTime = DateTime.Now.ToString("HH:mm");
        }

        private async void InitializeAsync()
        {
            await GetWeatherDataAsync();
        }

        public async Task GetWeatherDataAsync()
        {
            City = await _geolocationService.GetGeolocationAsync();
            

            string url = $"http://api.openweathermap.org/data/2.5/weather?q={City}&appid={_geolocationApiKey}&units=metric";



            using (HttpClient client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject weatherData = JObject.Parse(result);
                    WeatherCondition = weatherData["weather"][0]["main"].ToString();
                    Temperature = Convert.ToDouble(weatherData["main"]["temp"]);
                    UpdateWeatherImage(WeatherCondition);
                }
            }
        }


        private void UpdateWeatherImage(string condition)
        {
            string imagePath = condition.ToLower() switch
            {
                "clear" => "/Images/clear.png",
                "clouds" => "/Images/cloudy.png",
                "rain" => "/Images/rainy.png",
                "snow" => "/Images/snowing.png",
                "thundering" => "/Images/stormy.png",
                _ => "Images/default.png"
            };

            WeatherImage = Directory.GetCurrentDirectory() + imagePath;
        }

        public void RefreshBalance(decimal balance)
        {
            CurrentMoney = balance;
        }

        #endregion






    }
}
