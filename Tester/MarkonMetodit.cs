using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RataDigiTraffic.Model;


namespace Tester
{
    public static class MarkonMetodit
    {
        /// <summary>
        /// Tulosta haluttu juna junan tyypin/nimen (esim. IC) ja junan numeron mukaan. Jos junaa ei löydy, palauttaa "Etsittyä junaa ei löytynyt".
        /// </summary>
        /// <param name="nimi">junan nimi</param>
        /// <param name="numero">junan numero</param>
        public static void TulostaEtsittyJuna(string nimi, int numero, Dictionary<string, string> stations)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.EtsiJuna(nimi.ToUpper(), numero);
            foreach (var juna in junat)
            {
                var asemat = from a in juna.timeTableRows
                             where a.commercialStop == true
                             where a.liveEstimateTime.ToLocalTime() >= DateTime.Now
                             where a.type == "ARRIVAL"
                             select a;

                Console.WriteLine($"{juna.trainType}{juna.trainNumber}\n" +
                    $"Lähtöasema: {stations[juna.timeTableRows[0].stationShortCode].Split(" ")[0]}\n" +
                    $"Määränpää: {stations[juna.timeTableRows[^1].stationShortCode].Split(" ")[0]}\n" +
                    $"Lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString()}\n" +
                    $"Arvioitu saapumisaika: {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString()}\n" +
                    $"Ero aikatauluun: {juna.timeTableRows[1].differenceInMinutes} minuuttia");

                Console.WriteLine($"Seuraava pysähdys: {stations[asemat.First().stationShortCode].Split(" ")[0]} {asemat.First().liveEstimateTime.ToLocalTime().ToShortTimeString()}");
                Console.WriteLine();
            }
            if (junat.Count() == 0) { Console.WriteLine("Annetulla numerolla ja nimellä ei löytynyt junia."); }
        }
        /// <summary>
        /// Tulostaa radan käyttöön liittyvät rajoitukset
        /// </summary>
        public static void TulostaRajoitukset(string asema = "")
        {

            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Rajoitus> rajoitukset = rata.RadanRajoitukset();
            List<Liikennepaikka> asemat = rata.Liikennepaikat();
            if (asema == "")
            {
                foreach (var rajoitus in rajoitukset)
                {
                    var latitude = rajoitus.location[1];
                    var longitude = rajoitus.location[0];

                    Console.WriteLine($"{rajoitus.limitation}\n" +
                        $"{rajoitus.startDate.ToShortDateString()} - {(rajoitus.endDate.ToShortDateString() != "01/01/0001" ? rajoitus.endDate.ToShortDateString() : "Käynnissä")}");

                    var query = from a in asemat
                                orderby Apufunktiot.PisteidenEtaisyys((double)a.latitude, (double)a.longitude, latitude, longitude)
                                select a;

                    Console.WriteLine($"Rajoituksia rataliikenteen toiminnassa lähellä asemaa: {query.First().stationName}");
                    Console.WriteLine();
                }
            }
            else
            {
                var rajoiteQuery = from r in rajoitukset
                                   where (from a in asemat
                                          orderby Apufunktiot.PisteidenEtaisyys((double)a.latitude, (double)a.longitude, r.location[1], r.location[0])
                                          select a).First().stationName.Contains(asema)
                                   select r;
                if (rajoiteQuery.Count() == 0) { Console.WriteLine("Ei liikennerajoituksia lähellä asemaa " + asema); }
                foreach (var raj in rajoiteQuery)
                {
                    Console.WriteLine($"{raj.organization}\n" +
                        $"{raj.created} - {(raj.state)}");
                    Console.WriteLine($"Vaikutus rataliikenteeseen lähellä asemaa: {asema}");
                    Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// Tulostaa radan liikennetiedotteet.
        /// </summary>
        public static void TulostaTiedotteet(string asema = "")
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Liikennetiedote> tiedotteet = rata.Liikennetiedotteet();
            List<Liikennepaikka> asemat = rata.Liikennepaikat();
            if (asema == "")
            {
                foreach (var tiedote in tiedotteet)
                {
                    var latitude = tiedote.location[1];
                    var longitude = tiedote.location[0];
                    Console.WriteLine($"{tiedote.organization}\n" +
                        $"{tiedote.created} - {(tiedote.state)}");

                    var query = from a in asemat
                                orderby Apufunktiot.PisteidenEtaisyys((double)a.latitude, (double)a.longitude, latitude, longitude)
                                select a;

                    Console.WriteLine($"Vaikutus rataliikenteeseen lähellä asemaa: {query.First().stationName}");
                    Console.WriteLine();
                }
            }
            else
            {
                var tiedoteQuery = from t in tiedotteet
                          where (from a in asemat
                                 orderby Apufunktiot.PisteidenEtaisyys((double)a.latitude, (double)a.longitude, t.location[1], t.location[0])
                                 select a).First().stationName.Contains(asema)
                          select t;
                if (tiedoteQuery.Count() == 0) { Console.WriteLine("Ei liikennetiedotteita lähellä asemaa " + asema); }
                foreach (var tiedote in tiedoteQuery)
                {
                    Console.WriteLine($"{tiedote.organization}\n" +
                        $"{tiedote.created} - {(tiedote.state)}");
                    Console.WriteLine($"Vaikutus rataliikenteeseen lähellä asemaa: {asema}");
                    Console.WriteLine();
                }
            }
        }
        /// <summary>
        /// Tulostaa kaikki liikenteessä olevat junat
        /// </summary>
        public static void TulostaLiikkuvatJunat(Dictionary<string, string> asemat)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.LiikkuvatJunat();
            Console.WriteLine("Rataverkossa tällä hetkellä liikkeellä olevat junat: ");
            Console.WriteLine();
            foreach (var juna in junat)
            {
                string lahtoAsema;
                string maaraAsema;
                if (asemat.ContainsKey(juna.timeTableRows[0].stationShortCode) == false)
                {
                    lahtoAsema = juna.timeTableRows[0].stationShortCode;
                }
                else
                {
                    lahtoAsema = asemat[juna.timeTableRows[0].stationShortCode];
                }
                if (asemat.ContainsKey(juna.timeTableRows[^1].stationShortCode) == false)
                {
                    maaraAsema = juna.timeTableRows[^1].stationShortCode;
                }
                else
                {
                    maaraAsema = asemat[juna.timeTableRows[^1].stationShortCode];

                }
                string junaNimi = juna.trainType + juna.trainNumber;

                Console.WriteLine($"{junaNimi,10}\t{lahtoAsema,-25} " +
                    $"=>\t{maaraAsema,-25}" +
                    $"\t{juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(),5} " +
                    $"- {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5}");
            }
        }
    }
}
