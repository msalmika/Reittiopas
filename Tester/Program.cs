using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace DigiTrafficTester
{
    class Program
    {
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää]");
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
            if (args[0].ToLower().StartsWith("-s"))
            {
                string asema;
                if (args.Length < 2)
                {
                    PrintUsage();
                    return;
                }
                asema = args[1];
                TulostaJunat(asema, true, "11.20");
            }
        }

        private static void TulostaJunat(string asema, bool lahteva = true, string mistaEteenpain = "")
        {
            DateTime haunAloitus;
            if (mistaEteenpain.Equals(""))
                haunAloitus = DateTime.Now;
            else if (Regex.IsMatch(mistaEteenpain, @"\d{2}\.\d{2}"))
                haunAloitus = DateTime.ParseExact(mistaEteenpain, "HH.mm", System.Globalization.CultureInfo.InvariantCulture);
            else
            {
                PrintUsage();
                return;
            }
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.SaapuvatJaLahtevat(asema);
            foreach( var ju in junat)
            {
                var aikataulutiedot = ju.timeTableRows;
                if (aikataulutiedot[0].scheduledTime > haunAloitus)
                    Console.WriteLine($"{asema} - {aikataulutiedot[aikataulutiedot.Count-1].stationShortCode, -4} : {aikataulutiedot[0].scheduledTime.ToShortTimeString()} - {aikataulutiedot[aikataulutiedot.Count-1].scheduledTime.ToShortTimeString()}"); 
            }

            //var juna = junat[0];
            //var tulostettavat = juna.timeTableRows
            //    .Where(j => j.commercialStop == true && j.type == "DEPARTURE")
            //    .Select(j => j);
            //var rivit = juna.timeTableRows;

            //foreach (var r in tulostettavat)
            //    Console.WriteLine($"liveestimate: {r.type}, scheduled: {r.scheduledTime.ToShortTimeString()}, {r.stationShortCode}");
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

        private static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("Ohje:");
            Console.WriteLine("DigiTrafficTester -a[semat] <asemanAlkukirjain>");
            Console.WriteLine("tai");
            Console.WriteLine("DigiTrafficTester -j[unat] alkuasemaLyhenne loppuasemaLyhenne");
            Console.WriteLine("tai");
            Console.WriteLine("DigiTrafficTester -s[aapuvat] asemaLyhenne");
            Console.WriteLine();
        }
    }
}

