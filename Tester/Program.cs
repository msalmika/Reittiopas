using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace DigiTrafficTester
{
    class Program
    {
        private const string lähtevä = "DEPARTURE";
        private const string saapuva = "ARRIVAL";
        static void Main(string[] args)
        {

            if (args.Length == 0)
            {
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää], -l [asema][lkm][pvm][aika], -s[asema][pvm][aika]");
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
                string pvm;
                string aika;
                if (args.Length < 4)
                {
                    PrintUsage();
                    return;
                }
                asema = args[1];
                pvm = args[2];
                aika = args[3];
                TulostaJunat(asema, false, pvm, aika);
            }
            if (args[0].ToLower().StartsWith("-l"))
            {
                string asema;
                int lkm;
                string pvm;
                string aika;
                if (args.Length < 3)
                {
                    PrintUsage();
                    return;
                }
                asema = args[1];
                lkm = Int32.Parse(args[2]);
                if (args.Length == 4)
                {
                    pvm = args[3];
                    TulostaLähtevät(asema, lkm, pvm);
                }
                if (args.Length == 5)
                {
                    pvm = args[3];
                    aika = args[4];
                    TulostaLähtevät(asema, lkm, pvm, aika);
                }

            }
        }

        private static void TulostaLähtevät(string asema, int tulostettavienLkm, string pvm = "", string klo = "")
        {
            DateTime haunAloitus;
            string mistaLahtien = pvm + " " + klo;
            string[] muotoilut = { "dd.MM.yyyy HH.mm", "dd.MM.yyyy", "HH.mm" };
            if (mistaLahtien.Equals(" "))
                haunAloitus = DateTime.Now;
            else if (DateTime.TryParseExact(mistaLahtien, muotoilut, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime aika))
            {
                haunAloitus = aika;
            }
            else
            {
                PrintUsage();
                return;
            }
            var hakuPVM = String.Join('-', pvm.Split('.').Reverse());
            
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.SaapuvatJaLahtevat(hakuPVM);
            Console.WriteLine($"\nAsemalta {asema} {haunAloitus} eteenpäin lähtevät junat:\n");
            bool riittää = false;
            int i = 0;
            foreach (var ju in junat.OrderBy(j => j.timeTableRows.Where(j => j.stationShortCode == asema).Select(x => x.scheduledTime).FirstOrDefault()))
            {
                foreach (var aikataulutieto in ju.timeTableRows)
                {
                    if ((ju.trainCategory == "Commuter" || ju.trainCategory == "Long-distance") && aikataulutieto.commercialStop == true
                        && ju.cancelled == false && aikataulutieto.stationShortCode.Equals(asema) && aikataulutieto.scheduledTime.ToLocalTime() > haunAloitus
                        && aikataulutieto.type == lähtevä)
                    {
                        Console.WriteLine($"{aikataulutieto.stationShortCode} - {ju.timeTableRows[^1].stationShortCode,-4} : " +
                        $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortTimeString(),-5} - " +
                        $"{ju.timeTableRows[^1].scheduledTime.ToLocalTime().ToShortTimeString(),-5} " +
                        $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortDateString()}");
                        i++;
                        if (i == tulostettavienLkm)
                            riittää = true;
                    }
                    if (riittää)
                        return;
                }
                if (riittää)
                    return;
            }
            
        }

        private static void TulostaJunat(string asema, bool lahteva = true, string mistaEteenpainPvm = "", string mistaEteenpainAika = "")
        {
            string lähteeSaapuu = "DEPARTURE";
            if (!lahteva)
                lähteeSaapuu = "ARRIVAL";
            var PvmKlo = new string[] { mistaEteenpainPvm, mistaEteenpainAika };
            DateTime haunAloitus;
            if (mistaEteenpainAika.Equals("") && mistaEteenpainPvm == "")
                haunAloitus = DateTime.Now;
            else if (Regex.IsMatch(mistaEteenpainPvm, @"\d{2}.\d{2}.\d{4}") && mistaEteenpainAika.Equals(""))
                haunAloitus = DateTime.ParseExact(mistaEteenpainPvm, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            else if (mistaEteenpainPvm.Equals("") && Regex.IsMatch(mistaEteenpainAika, @"\d{2}\.\d{2}"))
                haunAloitus = DateTime.ParseExact(mistaEteenpainAika, "HH.mm", CultureInfo.InvariantCulture);
            else if (Regex.IsMatch(String.Join(' ', new string[] { mistaEteenpainPvm, mistaEteenpainAika }), @"\d{2}\.\d{2}\.\d{4}.\d{2}\.\d{2}"))
                haunAloitus = DateTime.ParseExact(String.Join(' ', PvmKlo), "dd.MM.yyyy HH.mm", CultureInfo.InvariantCulture);
            else
            {
                PrintUsage();
                return;
            }
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.SaapuvatJaLahtevat("2021 - 09 - 09");
            var tulostettavat = new List<List<IComparable>>();
            
            if (lahteva)
            {
                Console.WriteLine($"\nAsemalta {asema} {haunAloitus} eteenpäin lähtevät junat:\n");
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
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortDateString()}" +
                            $", Raide : {aikataulutieto.commercialTrack}");
                        }
                    }
                }

            }
            else
            {
                Console.WriteLine($"\nAsemalle {asema} {haunAloitus} eteenpäin saapuvat junat:\n");
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
                            $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortDateString()}" +
                            $", Raide : {aikataulutieto.commercialTrack}");
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

