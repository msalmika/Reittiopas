using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tester;

namespace DigiTrafficTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, string> asemat = Apufunktiot.HaeAsemat();
            
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

