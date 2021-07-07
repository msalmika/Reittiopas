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
                if (args.Length < 3)
                {
                    PrintUsage();
                    return;
                }
                lähtöasema = args[1];
                kohdeasema = args[2];
                TulostaJunatVälillä(lähtöasema, kohdeasema);
            }
            if (args[0].ToLower().StartsWith("-lj"))
            {
                TulostaLiikkuvatJunat();
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

        private static void TulostaJunatVälillä(string lähtöasema, string kohdeasema)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.JunatVälillä(lähtöasema, kohdeasema);
            string s = string.Join(", ", junat.Select(j => $"{j.trainNumber} {j.trainType}"));
            Console.WriteLine($"Junat {lähtöasema} ==> {kohdeasema}: " + s);
        }

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
        private static void TulostaLiikkuvatJunat()
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.LiikkuvatJunat();
            foreach (var juna in junat)
            {
                Console.WriteLine($"{juna.trainType,5} {juna.trainNumber,5}, Lähtöasema: {juna.timeTableRows[0].stationShortCode,-5} {juna.departureDate.ToShortDateString()} " +
                    $" Määränpää: {juna.timeTableRows[^1].stationShortCode,-5} (lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(),5} arvioitu saapumisaika: {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5})");
            }
        }
        /// <summary>
        /// Tulosta haluttu juna junan tyypin/nimen (esim. IC) ja junan numeron mukaan. Jos junaa ei löydy, palauttaa "Etsittyä junaa ei löytynyt".
        /// </summary>
        /// <param name="nimi"></param>
        /// <param name="numero"></param>
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
                
                Console.WriteLine($"{juna.trainType,5} {juna.trainNumber,5}, Lähtöasema: {stations[juna.timeTableRows[0].stationShortCode].Split(" ")[0]} " +
                    $" Määränpää: {stations[juna.timeTableRows[^1].stationShortCode].Split(" ")[0]} (lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(),5} " +
                    $"arvioitu saapumisaika: {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5})" +
                    $"\nEro aikatauluun: {juna.timeTableRows[1].differenceInMinutes} minuuttia");

               Console.WriteLine($"Seuraava pysähdys: {asemat.First().stationShortCode} {asemat.First().liveEstimateTime.ToLocalTime().ToShortTimeString()}");
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

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Ohje:");
            Console.WriteLine("-a[semat] <asemanAlkukirjain>");
            Console.WriteLine("-j[unat] alkuasemaLyhenne loppuasemaLyhenne");
            Console.WriteLine("tai");
            Console.WriteLine("-e[tsi] junanTyyppi(IC) junanNumero ");

            Console.WriteLine();
        }
    }
}

