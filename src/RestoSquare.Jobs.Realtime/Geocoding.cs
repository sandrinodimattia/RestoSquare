using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestoSquare.Jobs.Realtime
{
    public static class Geocoding
    {
        public static Coordinates Execute(string country, string city, string street)
        {
            var client = new HttpClient();
            
            // Create query.
            var nameValue = new NameValueCollection();
            nameValue.Add("q", String.Join(", ", new[] { street, city, country }).Trim(',', ' '));
            nameValue.Add("format", "json");

            // Get content.
            var content = "";

            try
            {
                var result = client.GetAsync("http://open.mapquestapi.com/nominatim/v1/search.php" + ToQueryString(nameValue))
                    .Result;
                if (!result.IsSuccessStatusCode)
                    throw new Exception(result.Content.ReadAsStringAsync().Result);

                content = result
                    .Content
                    .ReadAsStringAsync()
                    .Result;
            }
            catch (AggregateException ex)
            {
                Console.WriteLine("Error: " + ex.InnerException.Message);
            }

            var array = (JArray)JsonConvert.DeserializeObject(content);
            if (array.Any())
            {
                var record = array.FirstOrDefault() as JObject;
                var latitude = Convert.ToDouble(record.Property("lat").Value.Value<string>(), CultureInfo.InvariantCulture);
                var longitude = Convert.ToDouble(record.Property("lon").Value.Value<string>(), CultureInfo.InvariantCulture);
                return new Coordinates { Latitude = latitude, Longitude = longitude };
            }

            return null;
        }
        private static string ToQueryString(NameValueCollection nvc)
        {
            var array = (from key in nvc.AllKeys
                from value in nvc.GetValues(key)
                select string.Format("{0}={1}", HttpUtility.UrlEncode(key), HttpUtility.UrlEncode(value))).ToArray();
            return "?" + string.Join("&", array);
        }
    }
}