using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace DigiTrafficTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> asemat = HaeAsemat();
            if (args.Length == 0)
            {
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää] -e [junatyyppi aka IC] [numero] -r -t -lj");
                args = Console.ReadLine().Split(" ");
                //PrintUsage();
                //return;
            }
            if (args[0].ToLower().StartsWith("-a"))
            {
                string asema = "";
                if (args.Length > 1)
                {
                    asema = args[1];
                }
                TulostaAsemat(asema);
                return;
            }
            if (args[0].ToLower().StartsWith("-j"))
            {
                string lähtöasema;
                string kohdeasema;
                //if (args.Length < 3)
                //{
                //    PrintUsage();
                //    return;
                //}
                lähtöasema = args[1];
                kohdeasema = args[2];
                TulostaJunatVälillä(lähtöasema, kohdeasema);
            }
            if (args[0].ToLower().StartsWith("-lj"))
            {
                TulostaLiikkuvatJunat(asemat);
                return;
            }
            if (args[0].ToLower().StartsWith("-e"))
            {
                string junatype = args[1];
                int junanro = int.Parse(args[2]);
                TulostaEtsittyJuna(junatype, junanro, asemat);
                return;
            }
            if (args[0].ToLower().StartsWith("-r"))
            {
                TulostaRajoitukset();
                return;
            }
            if (args[0].ToLower().StartsWith("-t"))
            {
                TulostaTiedotteet();
                return;
            }
        }
        /// <summary>
        /// Muokattu, MS
        /// </summary>
        /// <param name="lähtöasema">lähtöaseman nimi</param>
        /// <param name="kohdeasema">kohdeaseman nimi</param>
        private static void TulostaJunatVälillä(string lähtöasema, string kohdeasema)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.JunatVälillä(lähtöasema, kohdeasema);
            string s = string.Join(", ", junat.Select(j => $"{j.trainNumber} {j.trainType}"));
            Console.WriteLine($"Junat {lähtöasema} ==> {kohdeasema}: " + s);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asema">Vaihtoehtoinen parametri, palauttaa kaikki asemat tai "asema"lla alkavat asemat</param>
        private static void TulostaAsemat(string asema)
        {
            List<Liikennepaikka> paikat;
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            paikat = rata.Liikennepaikat();
            foreach (var item in paikat.Where(p => p.type == "STATION"))
            {
                if (item.stationName.StartsWith(asema))
                {
                    Console.WriteLine($"{item.stationName} - {item.stationShortCode}");
                }
            }
        }
        /// <summary>
        /// Haetaan rautatieverkoston asemat ja tehdään niistä sanakirja
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, string> HaeAsemat()
        {

            Dictionary<string, string> asemat = new Dictionary<string, string>();
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Liikennepaikka> paikat = rata.Liikennepaikat();
            foreach (var item in paikat.Where(p => p.type == "STATION"))
            {
                asemat.Add(item.stationShortCode, item.stationName);
            }
            return asemat;
        }
        /// <summary>
        /// Tulostaa kaikki liikenteessä olevat junat
        /// </summary>
        private static void TulostaLiikkuvatJunat(Dictionary<string,string> asemat)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.LiikkuvatJunat();
            foreach (var juna in junat)
            {
                string lahtoAsema;
                string maaraAsema;
                lahtoAsema = asemat[juna.timeTableRows[0].stationShortCode].Split(" ")[0];
                maaraAsema = asemat[juna.timeTableRows[^1].stationShortCode].Split(" ")[0];

                Console.WriteLine($"{juna.trainType}{juna.trainNumber}, Lähtöasema: {asemat[juna.timeTableRows[0].stationShortCode].Split(" ")[0]} {juna.departureDate.ToShortDateString()} " +
                    $" Määränpää: {asemat[juna.timeTableRows[^1].stationShortCode].Split(" ")[0]} (lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(),5} arvioitu saapumisaika: {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5})");
            }
        }
        /// <summary>
        /// Tulosta haluttu juna junan tyypin/nimen (esim. IC) ja junan numeron mukaan. Jos junaa ei löydy, palauttaa "Etsittyä junaa ei löytynyt".
        /// </summary>
        /// <param name="nimi">junan nimi</param>
        /// <param name="numero">junan numero</param>
        private static void TulostaEtsittyJuna(string nimi, int numero, Dictionary<string, string> stations)
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
        }
        /// <summary>
        /// Tulostaa radan käyttöön liittyvät rajoitukset
        /// </summary>
        private static void TulostaRajoitukset()
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Rajoitus> rajoitukset = rata.RadanRajoitukset();
            List<Liikennepaikka> asemat = rata.Liikennepaikat();
            foreach (var rajoitus in rajoitukset)
            {
                var latitude = rajoitus.location[1];
                var longitude = rajoitus.location[0];

                Console.WriteLine($"{rajoitus.limitation}\n" +
                    $"{rajoitus.startDate.ToShortDateString()} - {(rajoitus.endDate.ToShortDateString() != "01/01/0001" ? rajoitus.endDate.ToShortDateString() : "Käynnissä")}");

                var query = from a in asemat
                            orderby PisteidenEtaisyys((double)a.latitude, (double)a.longitude, latitude, longitude)
                            select a;

                Console.WriteLine($"Rajoituksia rataliikenteen toiminnassa lähellä asemaa: {query.First().stationName}");
                Console.WriteLine();


            }
        }
        /// <summary>
        /// Tulostaa radan liikennetiedotteet.
        /// </summary>
        private static void TulostaTiedotteet()
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Liikennetiedote> tiedotteet = rata.Liikennetiedotteet();
            List<Liikennepaikka> asemat = rata.Liikennepaikat();

            foreach (var tiedote in tiedotteet)
            {
                var latitude = tiedote.location[1];
                var longitude = tiedote.location[0];
                Console.WriteLine($"{tiedote.organization}\n" +
                    $"{tiedote.created} - {(tiedote.state)}");

                var query = from a in asemat
                            orderby PisteidenEtaisyys((double)a.latitude, (double)a.longitude, latitude, longitude)
                            select a;

                Console.WriteLine($"Vaikutus rataliikenteeseen lähellä asemaa: {query.First().stationName}");
                Console.WriteLine();

            }
        }
        /// <summary>
        /// Koordinaatiston etäisyyksien laskeminen.
        /// </summary>
        /// <param name="a_lat">aseman latitude </param>
        /// <param name="a_long">aseman longitude </param>
        /// <param name="latitude">kohteen x latitude </param>
        /// <param name="longitude">kohteen y longitude </param>
        /// <returns> Palauttaa aseman ja kohteen x koordinaattien välisen etäisyyden. </returns>
        public static double PisteidenEtaisyys(double a_lat, double a_long, double latitude, double longitude)
        {

            var x_ero = Math.Pow(a_lat - latitude, 2);
            double y_ero = Math.Pow(a_long - longitude, 2);
            double res = Math.Sqrt(y_ero + x_ero);
            return res;
        }

        //private static void PrintUsage()
        //{
        //    Console.WriteLine();
        //    Console.WriteLine("Ohje:");
        //    Console.WriteLine("-a[semat] <asemanAlkukirjain>");
        //    Console.WriteLine("-j[unat] alkuasemaLyhenne loppuasemaLyhenne");
        //    Console.WriteLine("tai");
        //    Console.WriteLine("-e[tsi] junanTyyppi(IC) junanNumero ");

        //    Console.WriteLine();
        //}
    }
}

