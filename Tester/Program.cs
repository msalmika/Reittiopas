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
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää], -m [asema] asemalta lähtevät ja saapuvat junat");
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
            if (args[0].ToLower().StartsWith("-m"))
            {
                string asema = "";
                if (args.Length < 2)
                {
                    PrintUsage();
                    return;
                }
                asema = args[1];
                TulostaAsemanJunat(asema);
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
        private static void TulostaAsemanJunat(string asema)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.AsemanJunat(asema);
            Console.WriteLine("LÄHTEVÄT JUNAT:");
            foreach (var juna in junat.OrderBy(x => x.timeTableRows.Where(x => x.stationShortCode == asema).Select(x => x.scheduledTime).FirstOrDefault()))
            {
                foreach (var t in juna.timeTableRows)
                {
                    if (t.type == "DEPARTURE" && t.commercialStop == true && t.cancelled == false 
                        && t.stationShortCode.Equals(asema) && juna.departureDate == DateTime.Today && juna.trainCategory == "Commuter"
                        || t.type == "DEPARTURE" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Long-distance")
                    {
                        Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(), -8}{juna.trainType + juna.trainNumber, -8} " +
                            $"Määränpää: {juna.timeTableRows[^1].stationShortCode} ");
                    }
                }
            }
            Console.WriteLine();
            Console.WriteLine("SAAPUVAT JUNAT:");
            foreach (var juna in junat.OrderBy(x => x.timeTableRows.Where(x => x.stationShortCode == asema).Select(x => x.scheduledTime).FirstOrDefault()))
            {
                foreach (var t in juna.timeTableRows)
                {
                    if (t.type == "ARRIVAL" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Commuter"
                        || t.type == "ARRIVAL" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Long-distance")
                    {
                        Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(),-8}{juna.trainType + juna.trainNumber,-8} " +
                                $"Lähtöasema: {juna.timeTableRows[0].stationShortCode} ");
                    }
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
            Console.WriteLine();
        }
    }
}

