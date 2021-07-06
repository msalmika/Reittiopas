
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
        //public List<Juna> LiikkuvatJunat()
        //{
        //    //string pvm = DateTime.Today.ToString("yyyy-MM-dd");
        //    string json = "";
        //    string url = $"{APIURL}/live-trains?version=0";
        //    json = UrlAvaaminen(url);
        //    List<Juna> haku = JsonConvert.DeserializeObject<List<Juna>>(json);
        //    List<Juna> res = new List<Juna>();
        //    var query = haku.Where(h => h.timeTableRows[0].cancelled == false);
        //    foreach (var q in query)
        //    {
        //        res.Add(q);
        //    }
        //    return res;
        //}

        public List<Juna> EtsiJuna(string type, int nro)
        {
            string json = "";
            string url = $"{APIURL}/live-trains?version=0";
            json = UrlAvaaminen(url);
            List<Juna> haku = JsonConvert.DeserializeObject<List<Juna>>(json);
            List<Juna> res = new List<Juna>();
            var query = haku.Where(h => h.trainNumber == nro && h.trainType == type).Where(h => h.runningCurrently);
            foreach ( var j in query)
            {
                res.Add(j);
            }
            return res;
        }
        public List<Rajoitus> RadanRajoitukset()
        {
            string json = "";
            string url = $"{APIURL}/trafficrestriction-notifications.json?schema=false&state=";
            json = UrlAvaaminen(url);
            List<Rajoitus> res = JsonConvert.DeserializeObject<List<Rajoitus>>(json);
            return res;
        }
        
    }





}
