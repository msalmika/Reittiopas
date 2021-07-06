using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigiTrafficTester
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää] - e [junatyyppi aka IC] [numero]");
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
            //if (args[0].ToLower().StartsWith("-lj"))
            //{
            //    TulostaLiikkuvatJunat();
            //    return;
            //}
            if (args[0].ToLower().StartsWith("-e"))
            {
                string junatype = args[1];
                int junanro = int.Parse(args[2]);
                TulostaEtsittyJuna(junatype, junanro);
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
        //private static void TulostaLiikkuvatJunat()
        //{
        //    RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
        //    List<Juna> junat = rata.LiikkuvatJunat();
        //    foreach (var juna in junat)
        //    {
        //        Console.WriteLine($"{juna.trainType, 5} {juna.trainNumber, 5}, Lähtöasema: {juna.timeTableRows[0].stationShortCode, -5} {juna.departureDate.ToShortDateString()} " +
        //            $" Määränpää: {juna.timeTableRows[^1].stationShortCode, -5} (lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(), 5} arvioitu saapumisaika: {juna.timeTableRows[^1].scheduledTime.ToLocalTime().ToShortTimeString()} {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5})");
        //    }
        //}
        private static void TulostaEtsittyJuna(string nimi, int numero)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.EtsiJuna(nimi, numero);
            foreach (var juna in junat)
            {
                var asemat = from a in juna.timeTableRows
                             where a.commercialStop == true
                             where a.liveEstimateTime.ToLocalTime() >= DateTime.Now
                             where a.type == "ARRIVAL"
                             select a;
                
                Console.WriteLine($"{juna.trainType,5} {juna.trainNumber,5}, Lähtöasema: {juna.timeTableRows[0].stationShortCode,-5} " +
                    $" Määränpää: {juna.timeTableRows[^1].stationShortCode,-5} (lähtöaika: {juna.timeTableRows[0].actualTime.ToLocalTime().ToShortTimeString(),5} " +
                    $"arvioitu saapumisaika: {juna.timeTableRows[^1].liveEstimateTime.ToLocalTime().ToShortTimeString(),5})" +
                    $"\nEro aikatauluun: {juna.timeTableRows[1].differenceInMinutes} minuuttia");

               Console.WriteLine($"Seuraava pysähdys: {asemat.First().stationShortCode} {asemat.First().liveEstimateTime.ToLocalTime().ToShortTimeString()}");
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Ohje:");
            Console.WriteLine("DigiTrafficTester -a[semat] <asemanAlkukirjain>");
            Console.WriteLine("tai");
            Console.WriteLine("DigiTrafficTester -j[unat] alkuasemaLyhenne loppuasemaLyhenne");
            Console.WriteLine();
        }
    }
}

