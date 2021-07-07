using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tester;
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
            Dictionary<string, string> asemat = Apufunktiot.HaeAsemat();
            
            if (args.Length == 0)
            {
                Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää], -l [asema][lkm][pvm][aika], -s[asema][pvm][aika]");
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
                if (asemat.Where(a => a.Key.Contains(asema)).Count() == 0)
                {
                    Console.WriteLine("Kyseisillä ehdoilla ei löytynyt asemia");
                }
                else
                {
                    TulostaAsemat(asema);
                }
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
                MarkonMetodit.TulostaLiikkuvatJunat(asemat);
                return;
            }
            if (args[0].ToLower().StartsWith("-e"))
            {
                string junatype = args[1];
                int junanro = int.Parse(args[2]);
                MarkonMetodit.TulostaEtsittyJuna(junatype, junanro, asemat);
                return;
            }
            if (args[0].ToLower().StartsWith("-r"))
            {
                MarkonMetodit.TulostaRajoitukset();
                return;
            }
            if (args[0].ToLower().StartsWith("-t"))
            {
                MarkonMetodit.TulostaTiedotteet();
                return;
            }
            if (args[0].ToLower().StartsWith("-sj"))
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

                Tester.SeuraavaSuoraJuna.TulostaSeuraavaSuoraJuna(lähtöasema, kohdeasema);
                
            }
        
            if (args[0].ToLower().StartsWith("-s"))
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
                    TulostaSaapuvat(asema, lkm, pvm);
                }
                if (args.Length == 5)
                {
                    pvm = args[3];
                    aika = args[4];
                    TulostaSaapuvat(asema, lkm, pvm, aika);
                }
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
            if (args[0].ToLower().StartsWith("-m"))
            {
                string asema = "";
                if (args.Length < 2)
                {
                    PrintUsage();
                    return;
                }
                asema = args[1];
                JunatAsemanPerusteella.TulostaAsemanJunat(asema);
            }
        }

        /// <summary>
        /// Tulostaa asemalle saapuvat junat tietyn ajankohdan jälkeen.
        /// </summary>
        /// <param name="asema"> Aseman nimi </param>
        /// <param name="tulostettavienLkm"> Haettavien junien määrä </param>
        /// <param name="pvm"> Päivämäärä </param>
        /// <param name="klo"> Kellon aika </param>
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

        /// <summary>
        /// Tulostaa asemalta tietyn ajankohdan jälkeen lähtivien junien tiedot.
        /// </summary>
        /// <param name="asema"> aseman nimi </param>
        /// <param name="tulostettavienLkm"> tulostetavien junien lukumäärä </param>
        /// <param name="pvm"> päivämäärä </param>
        /// <param name="klo"> kellonaika </param>
        private static void TulostaSaapuvat(string asema, int tulostettavienLkm, string pvm = "", string klo = "")
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
                        && aikataulutieto.type == saapuva)
                    {
                        Console.WriteLine($"{ju.timeTableRows[0].stationShortCode,-4} - {aikataulutieto.stationShortCode} : " +
                        $"{ju.timeTableRows[0].scheduledTime.ToLocalTime().ToShortTimeString(),-5} - " +
                        $"{aikataulutieto.scheduledTime.ToLocalTime().ToShortTimeString(),-5} " +
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

