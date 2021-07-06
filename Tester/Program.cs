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
                TulostaSeuraavaSuoraJuna(lähtöasema, kohdeasema);
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
            Console.WriteLine();
        }

        private static void TulostaSeuraavaSuoraJuna(string lähtöasema, string pääteasema)
        {
           // ei tarkista ovatko oikean tyyppisiä junia
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            Juna seuraavaJuna = rata.SeuraavaSuoraJunaVälillä(lähtöasema, pääteasema);
 
            Console.WriteLine($"\n{lähtöasema} ==> {pääteasema}\n");
            //Console.WriteLine($"{seuraavaJuna.trainType}{seuraavaJuna.trainNumber}");
            var lähtöaika = seuraavaJuna.timeTableRows[0].scheduledTime.ToLocalTime();
            var saapumisaika = seuraavaJuna.timeTableRows[seuraavaJuna.timeTableRows.Count - 1].scheduledTime.ToLocalTime();
            if (lähtöaika.Date == saapumisaika.Date)
            {
                
                Console.WriteLine($"{"aikataulu", -20} {"juna", -10} {"lähtölaituri", -5}");
                //Console.WriteLine();
                Console.WriteLine($"{lähtöaika.ToString("d.M.yyyy")}");
                string aikataulu = $"{lähtöaika.ToString("H:mm")} ==> {saapumisaika.ToString("H:mm")}";
                string juna = $"{seuraavaJuna.trainType}{seuraavaJuna.trainNumber}";
                Console.WriteLine($"{aikataulu, -20} " +
                    $"{juna, -10} {seuraavaJuna.timeTableRows[0].commercialTrack, -5}");

            }
            else
            {
                Console.WriteLine($"{lähtöaika.ToString("d.M.yyyy")} - {saapumisaika.ToString("d.M.yyyy")}");
                Console.WriteLine($"{lähtöaika.ToString("H:mm")} ==> {saapumisaika.ToString("H:mm")}      {seuraavaJuna.trainType}{seuraavaJuna.trainNumber}");

            }
        }
    }
}

