
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


        /// <summary>
        /// Palauttaa seuraavan suoran junan kahden aseman välillä.
        /// </summary>
        /// <param name="mistä">string, lähtöaseman koodi</param>
        /// <param name="minne">string, määränpää-aseman koodi</param>
        /// <returns>Juna, junaa kuvaava olio, jos seuraava juna löytyy.
        /// Null, jos välille ei löydetä junaa.</returns>
        public Juna SeuraavaSuoraJunaVälillä(string mistä, string minne)
        {
            try
            {
                string url = $"{APIURL}/live-trains/station/{mistä}/{minne}";
                string json = UrlAvaaminen(url);
                List<Juna> res = JsonConvert.DeserializeObject<List<Juna>>(json);

                foreach (Juna juna in res)
                {
                    if (juna.cancelled == false && (juna.trainCategory == "Commuter" | juna.trainCategory == "Long-distance"))
                    {
                        return juna;
                    }
                }
                return null;
            }
            catch (JsonSerializationException)
            {
                throw new ArgumentException("Ei suoraa junaa asemien välillä.");
            }
                
        } 
    }
}
