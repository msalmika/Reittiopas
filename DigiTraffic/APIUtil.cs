
using Newtonsoft.Json;
using RataDigiTraffic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RataDigiTraffic
{
    public class APIUtil
    {

        private const string APIURL = "https://rata.digitraffic.fi/api/v1";
        public List<Liikennepaikka> Liikennepaikat()
        {
            string json = "";
            string url = $"{APIURL}/metadata/stations";
            json = UrlAvaaminen(url);
            //using HttpClient client = new HttpClient(GetZipHandler());
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //client.DefaultRequestHeaders.Add("accept-encoding", "gzip");
            //var response = client.GetAsync($"{APIURL}/metadata/stations").Result;
            //string json = response.Content.ReadAsStringAsync().Result;
            List<Liikennepaikka> res = JsonConvert.DeserializeObject<List<Liikennepaikka>>(json); //NewtonSoftin serialisointi
            //List<Liikennepaikka> res = JsonSerializer.Deserialize<List<Liikennepaikka>>(json);  // Core:n oma
            return res;


        }

        public List<Juna> JunatVälillä(string mistä, string minne)
        {
            string json = "";
            string url = $"{APIURL}/schedules?departure_station={mistä}&arrival_station={minne}";
            json = UrlAvaaminen(url);
            //using (var client = new HttpClient(GetZipHandler()))
            //{
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = client.GetAsync(url).Result;
            //    var responseString = response.Content.ReadAsStringAsync().Result;
            //    json = responseString;
            //}
            List<Juna> res = JsonConvert.DeserializeObject<List<Juna>>(json);
            return res;
        }

        public List<Kulkutietoviesti> LiikennepaikanJunat(string paikka)
        {
            string json = "";
            string url = $"{APIURL}/train-tracking?station={paikka}&departure_date={DateTime.Today.ToString("yyyy-MM-dd")}";
            json = UrlAvaaminen(url);
            //using (var client = new HttpClient(GetZipHandler()))
            //{
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //    var response = client.GetAsync(url).Result;
            //    var responseString = response.Content.ReadAsStringAsync().Result;
            //    json = responseString;
            //}
            List<Kulkutietoviesti> res = JsonConvert.DeserializeObject<List<Kulkutietoviesti>>(json);
            return res;
        }
        public List<Juna> SaapuvatJaLahtevat(string pvm)
        {
            string json;
            string url = $"{APIURL}/trains/{pvm}?include_deleted=false";
            json = UrlAvaaminen(url);
            List<Juna> res = JsonConvert.DeserializeObject<List<Juna>>(json);
            return res;
            }

        private static HttpClientHandler GetZipHandler()
        {
            return new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
        }
        private static string UrlAvaaminen(string url)
        {
            string json;
            using (var client = new HttpClient(GetZipHandler()))
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("accept-encoding", "gzip");
                var response = client.GetAsync(url).Result;
                var responseString = response.Content.ReadAsStringAsync().Result;
                json = responseString;
            }
            return json;
        }
    }





}
