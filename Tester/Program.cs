using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;

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
                TulostaJunat(asema, false, "11.20");
            }
            if (args[0].ToLower().StartsWith("-l"))
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

        private static void TulostaJunat(string asema, bool lahteva = true, string mistaEteenpainAika = "", string mistaEteenpainPvm = "")
        {
            string lähteeSaapuu = "DEPARTURE";
            if (!lahteva)
                lähteeSaapuu = "ARRIVAL";
            DateTime haunAloitus;
            if (mistaEteenpainAika.Equals("") && mistaEteenpainPvm == "")
                haunAloitus = DateTime.Now;
            else if (Regex.IsMatch(mistaEteenpainAika, @"\d{2}\.\d{2}"))
                haunAloitus = DateTime.ParseExact(mistaEteenpainAika, "HH.mm", System.Globalization.CultureInfo.InvariantCulture);
            else
            {
                PrintUsage();
                return;
            }
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.SaapuvatJaLahtevat(asema);
            var tulostettavat = new List<List<IComparable>>();
            
            if (lahteva)
            {
                Console.WriteLine($"Aselmalta {asema} {haunAloitus} eteenpäin lähtevät junat:");
                foreach (var ju in junat)
                {
                    foreach (var aikataulutieto in ju.timeTableRows.OrderBy(j => j.scheduledTime))
                    {
                        if ((ju.trainCategory == "Commuter" || ju.trainCategory == "Long-distance") && aikataulutieto.commercialStop == true
                            && ju.cancelled == false && aikataulutieto.stationShortCode.Equals(asema) && aikataulutieto.scheduledTime.ToLocalTime() > haunAloitus
                            && aikataulutieto.type == lähteeSaapuu && !aikataulutieto.stationShortCode.Equals(ju.timeTableRows[^1].stationShortCode))
                        {
                            Console.WriteLine($"{aikataulutieto.stationShortCode} - {ju.timeTableRows[^1].stationShortCode,-4} : " +
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortTimeString(),-5} - " +
                            $"{ju.timeTableRows[^1].scheduledTime.ToLocalTime().ToShortTimeString(),-5} " +
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortDateString()}");
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine($"Aselmalle {asema} {haunAloitus} eteenpäin saapuvat junat:");
                foreach (var ju in junat)
                {
                    foreach (var aikataulutieto in ju.timeTableRows.OrderBy(j => j.scheduledTime))
                    {
                        if ((ju.trainCategory == "Commuter" || ju.trainCategory == "Long-distance") && aikataulutieto.commercialStop == true
                            && ju.cancelled == false && aikataulutieto.stationShortCode.Equals(asema) && aikataulutieto.scheduledTime.ToLocalTime() > haunAloitus
                            && aikataulutieto.type == lähteeSaapuu && !aikataulutieto.stationShortCode.Equals(ju.timeTableRows[^1].stationShortCode))
                        {
                            Console.WriteLine($"{aikataulutieto.stationShortCode} - {ju.timeTableRows[^1].stationShortCode,-4} : " +
                            $"{ju.timeTableRows[^1].scheduledTime.ToLocalTime().ToShortTimeString(),-5} - " +
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortTimeString(),-5} " +
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortDateString()}");
                        }
                    }
                }
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

