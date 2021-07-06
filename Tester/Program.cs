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

        /// <summary>
        /// Tulostaa seuraavan suoran junan tiedot (aika, lähtö- ja pääteasemat, junan koodi, lähtölaituri) 
        /// lähtö- ja pääteaseman perusteella.
        /// </summary>
        /// <param name="lähtöasema">string, asema, jolta juna lähtee.</param>
        /// <param name="pääteasema">string, asema, jolle juna saapuu.</param>
        private static void TulostaSeuraavaSuoraJuna(string lähtöasema, string pääteasema)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            Juna seuraavaJuna = rata.SeuraavaSuoraJunaVälillä(lähtöasema, pääteasema);
            var lähtöaika = seuraavaJuna.timeTableRows[0].scheduledTime.ToLocalTime();
            var saapumisaika = seuraavaJuna.timeTableRows[seuraavaJuna.timeTableRows.Count - 1].scheduledTime.ToLocalTime();
            string pvm = $"{lähtöaika.ToString("d.M.yyyy")}";

            if (lähtöaika.Date != saapumisaika.Date)
            {
                pvm += $" - {saapumisaika.ToString("d.M.yyyy")}";
            }
            string aikataulu = $"{lähtöaika.ToString("H:mm")} ==> {saapumisaika.ToString("H:mm")}";
            string juna = $"{seuraavaJuna.trainType}{seuraavaJuna.trainNumber}";

            Console.WriteLine($"\n{lähtöasema} ==> {pääteasema}\n");
            Console.WriteLine($"{"aikataulu",-20} {"juna",-10} {"lähtölaituri",-5}");
            Console.WriteLine($"{pvm}");
            Console.WriteLine($"{aikataulu,-20} " +
                $"{juna,-10} {seuraavaJuna.timeTableRows[0].commercialTrack,-5}");
            TulostaJunanPysäkkienTiedot(seuraavaJuna);
        }

        /// <summary>
        /// Tulostaa välipysäkkien tiedot (pysäkin nimi, laituri, saapumisaika ja lähtöaika, pysähdysaika)
        /// </summary>
        /// <param name="juna">Juna, junaa kuvaava olio</param>
        private static void TulostaJunanPysäkkienTiedot(Juna juna)
        {
            Console.WriteLine("\njunan reitti:");
            Console.WriteLine($"{"pysäkki", -9} {"asemalla", -16} {"pysähdyksen kesto", -18} {"laituri", -3}");
            Console.WriteLine($"{juna.timeTableRows[0].stationShortCode, -9} " +
                $"{juna.timeTableRows[0].scheduledTime.ToString("H:mm"), -16} {"", -18} {juna.timeTableRows[0].commercialTrack, -3}");
            Aikataulurivi edellinen = juna.timeTableRows[0];
            TimeSpan erotus;
            string pysähdyksenkesto;

            foreach (Aikataulurivi pysähdys in juna.timeTableRows.GetRange(1, juna.timeTableRows.Count()-2))
            {
                
                if(pysähdys.trainStopping  && pysähdys.commercialStop)
                {
                    if(pysähdys.stationShortCode == edellinen.stationShortCode && edellinen.type == "ARRIVAL" && 
                        pysähdys.type == "DEPARTURE")
                    {
                        erotus = pysähdys.scheduledTime - edellinen.scheduledTime;
                        pysähdyksenkesto = erotus.TotalMinutes.ToString() + " min";
                        
                        Console.WriteLine($"{pysähdys.stationShortCode, -9} {pysähdyksenkesto, -16} {erotus.TotalMinutes, -18} {pysähdys.commercialTrack, -3}");
                    }
                    edellinen = pysähdys;
                }
            }
            Console.WriteLine($"{juna.timeTableRows[juna.timeTableRows.Count - 1].stationShortCode, -9} " +
                $"{juna.timeTableRows[juna.timeTableRows.Count - 1].scheduledTime.ToString("H:mm"), -16} " +
                $"{"",-18} {juna.timeTableRows[juna.timeTableRows.Count - 1].commercialTrack, -3}");
        }
    }
}

