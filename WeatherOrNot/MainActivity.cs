using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Locations;
using Android.OS;
using Android.Util;
using Android.Widget;
using Android.Content;
using Android.Runtime;
using Android.Views;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace WeatherOrNot
{
    [Activity(Label = "Get Location", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity, ILocationListener
    {

        static readonly string TAG = "X:" + typeof(Activity1).Name;
        TextView _addressText;
        Location _currentLocation;
        NumberPicker _kmPicker;
        LocationManager _locationManager;

        string _locationProvider;
        //TextView _locationText;
        double currLatitude, currLongitude;

        public async void OnLocationChanged(Location location)
        {
            _currentLocation = location;
            if (_currentLocation == null)
            {
                _addressText.Text = "Unable to determine your location. Try again in a short while.";
            }
            else
            {
               // _locationText.Text = string.Format("{0:f6},{1:f6}", _currentLocation.Latitude, _currentLocation.Longitude);
                currLongitude = _currentLocation.Longitude;
                currLatitude = _currentLocation.Latitude;
                Address address = await ReverseGeocodeCurrentLocation();
                DisplayAddress(address);
            }
        }

        public void OnProviderDisabled(string provider) { }

        public void OnProviderEnabled(string provider) { }

        public void OnStatusChanged(string provider, Availability status, Bundle extras)
        {
            Log.Debug(TAG, "{0}, {1}", provider, status);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Main);

            InitializeLocationManager();

            _addressText = FindViewById<TextView>(Resource.Id.address_text);
            //_locationText = FindViewById<TextView>(Resource.Id.location_text);
            //FindViewById<TextView>(Resource.Id.get_address_button).Click += AddressButton_OnClick;

            _kmPicker = FindViewById<NumberPicker>(Resource.Id.kmPicker);
            _kmPicker.Value = 5;
            _kmPicker.MinValue = 1;
            _kmPicker.MaxValue = 20;

            Button button = FindViewById<Button>(Resource.Id.btnNext);
            button.Click += delegate
            {
                string key = getLocationKey(currLongitude, currLatitude);
                var listForecast = getForecast("10day", key);
                
                StartActivity(typeof(SecondScreen));
            };
        }

        public void InitializeLocationManager()
        {
            _locationManager = (LocationManager)GetSystemService(LocationService);
            Criteria criteriaForLocationService = new Criteria
            {
                Accuracy = Accuracy.Fine
            };
            IList<string> acceptableLocationProviders = _locationManager.GetProviders(criteriaForLocationService, true);

            if (acceptableLocationProviders.Any())
            {
                _locationProvider = acceptableLocationProviders.First();
            }
            else
            {
                _locationProvider = string.Empty;
            }
            Log.Debug(TAG, "Using " + _locationProvider + ".");
        }

        protected override void OnResume()
        {
            base.OnResume();
            _locationManager.RequestLocationUpdates(_locationProvider, 0, 0, this);
            Log.Debug(TAG, "Listening for location updates using " + _locationProvider + ".");
        }

        protected override void OnPause()
        {
            base.OnPause();
            _locationManager.RemoveUpdates(this);
            Log.Debug(TAG, "No longer listening for location updates.");
        }

        //async void AddressButton_OnClick(object sender, EventArgs eventArgs)
        //{
        //    if (_currentLocation == null)
        //    {
        //        _addressText.Text = "Can't determine the current address. Try again in a few minutes.";
        //        return;
        //    }

        //    Address address = await ReverseGeocodeCurrentLocation();
        //    DisplayAddress(address);
        //}

        async Task<Address> ReverseGeocodeCurrentLocation()
        {
            Geocoder geocoder = new Geocoder(this);
            IList<Address> addressList =
                await geocoder.GetFromLocationAsync(_currentLocation.Latitude, _currentLocation.Longitude, 10);

            Address address = addressList.FirstOrDefault();
            return address;
        }

        void DisplayAddress(Address address)
        {
            if (address != null)
            {
                string deviceAddress = "";
                //for (int i = 0; i < address.MaxAddressLineIndex; i++)
                //{
                deviceAddress = address.GetAddressLine(address.MaxAddressLineIndex - 1);
                //}
                // Remove the last comma from the end of the address.
                _addressText.Text = deviceAddress.ToString();
            }
            else
            {
                _addressText.Text = "Unable to determine the address. Try again in a few minutes.";
            }
        }

        public string getLocationKey(double longitude, double latitude)
        {
            string apiKey = "HackuWeather2016";
            string url = "http://apidev.accuweather.com/locations/v1/cities/geoposition/search.json?q=" + latitude.ToString() + "," + longitude.ToString() + "&apikey=" + apiKey;
            
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(url);
                var currLocation = Newtonsoft.Json.Linq.JObject.Parse(json);

                return (string)currLocation["Key"];//account for null case!?!??!
            }
        }

        public object getForecast(string timeString, string locationKey)
        {
            string apiKey = "HackuWeather2016";
            string url = "http://apidev.accuweather.com/forecasts/v1/daily/" + timeString + "/" + locationKey + "?apikey=" + apiKey;
            
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(url);
                var locForecast = Newtonsoft.Json.Linq.JObject.Parse(json);

                var i = 0;
                var listForecast = new List<Dictionary<string, string>>();

                foreach (var item in locForecast["DailyForecasts"])
                {
                    Dictionary<string, string> dict = new Dictionary<string, string>();

                    dict.Add("minVal", (string)locForecast["DailyForecasts"][i]["Temperature"]["Minimum"]["Value"]);
                    dict.Add("maxVal", (string)locForecast["DailyForecasts"][i]["Temperature"]["Maximum"]["Value"]);
                    dict.Add("iconVal", (string)locForecast["DailyForecasts"][i]["Day"]["Icon"]);
                    dict.Add("descriptionString", (string)locForecast["DailyForecasts"][i]["Day"]["IconPhrase"]);
                    listForecast.Add(dict);
                    i++;
                }
                return listForecast;
            }
        }

        //public void SendRequestAccuweather(string requestType, string locationKey)
        //{
        //    string apiKey = "HackuWeather2016";
        //    string url = "http://apidev.accuweather.com/forecasts/v1/daily/10day/" + locationKey + "?apikey=" + apiKey;
        //    //string url = "http://apidev.accuweather.com/" + requestType + "/v1/" + locationKey + ".json?apikey=" + apiKey;
        //    using (var webClient = new System.Net.WebClient())
        //    {
        //        var json = webClient.DownloadString(url);
        //        // Now parse with JSON.Net
        //        var currLocation = JsonConvert.DeserializeObject<CurrentLocation>(json);
                
        //    }
        //}

        //public void SendPOIRequestGoogle(double latitude, double longitude, int radius)
        //{
        //    string apiKey = "AIzaSyBEW5DD9mB8UWIZuPus3vaWGDnlUw0L6Eg";
        //    string typesString = "AIzaSyBEW5DD9mB8UWIZuPus3vaWGDnlUw0L6Eg";
        //    string url = "https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" +
        //        latitude.ToString() + "," + longitude.ToString() +
        //        "&radius =" + radius.ToString() +
        //        "&type =" + typesString +
        //        "&key =" + apiKey;

        //    using (var webClient = new System.Net.WebClient())
        //    {
        //        var json = webClient.DownloadString(url);
        //        // Now parse with JSON.Net
        //    }


        //}

        //// Gets data from the passed URL.
        //private async Task<JsonValue> FetchWeatherAsync(string url)
        //{
        //    // Create an HTTP web request using the URL:
        //    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(new Uri(url));
        //    request.ContentType = "application/json";
        //    request.Method = "GET";

        //    // Send the request to the server and wait for the response:
        //    using (WebResponse response = await request.GetResponseAsync())
        //    {
        //        // Get a stream representation of the HTTP web response:
        //        using (Stream stream = response.GetResponseStream())
        //        {
        //            // Use this stream to build a JSON document object:
        //            JsonValue jsonDoc = await Task.Run(() => JsonObject.Load(stream));
        //            Console.Out.WriteLine("Response: {0}", jsonDoc.ToString());

        //            // Return the JSON document:
        //            return jsonDoc;
        //        }
        //    }
        //}

    }
}