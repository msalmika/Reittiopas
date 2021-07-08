using RataDigiTraffic.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public static class SeuraavaSuoraJuna
    {
        public static void TulostaSuoratJunat(string lähtöasema, string pääteasema, Dictionary<string, string> asemat)
        {

        }


        /// <summary>
        /// Tulostaa seuraavan suoran junan tiedot (aika, lähtö- ja pääteasemat, junan koodi, lähtölaituri) 
        /// lähtö- ja pääteaseman perusteella. 
        /// </summary>
        /// <param name="lähtöasema">asema, joka on haussa asetettu lähtöasemaksi</param>
        /// <param name="pääteasema">asema, joka on haussa asetettu määränpääasemaksi</param>
        /// <param name="asemat">asemien koodit ja koko nimet</param>
        public static void TulostaSeuraavaSuoraJuna(string lähtöasema, string pääteasema, Dictionary<string, string> asemat)
        {
            try
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

                Console.WriteLine($"\n{asemat[lähtöasema]} ==> {asemat[pääteasema]}\n");
                Console.WriteLine($"{"aikataulu",-20} {"juna",-10} {"lähtölaituri",-5}");
                Console.WriteLine($"{pvm}");
                Console.WriteLine($"{aikataulu,-20} " +
                    $"{juna,-10} {seuraavaJuna.timeTableRows[0].commercialTrack,-5}");

                TulostaJunanPysäkkienTiedot(seuraavaJuna, lähtöasema, pääteasema, asemat);
                //TulostaJunanPysäkkienTiedot(seuraavaJuna, asemat);
            }
            // jos juna ei kulje asemien välillä
            catch (ArgumentException e)
            {
                Console.WriteLine(e.Message);
            }
        }


        /// <summary>
        /// Tulostaa koko junan reitin välipysäkkien tiedot.
        /// (pysäkin nimi, laituri, saapumisaika ja lähtöaika, aika pysähdyksissä)
        /// </summary>
        /// <param name="juna">junaa kuvaava olio</param>
        /// <param name="asemat">asemien koodit ja koko nimet</param>
        public static void TulostaJunanPysäkkienTiedot(Juna juna, Dictionary<string, string> asemat)
        {
            Console.WriteLine("\njunan reitti:");
            Console.WriteLine($"{"pysäkki",-20} {"asemalla",-16} {"pysähdyksen kesto",-18} {"laituri",-3}");

            // Tulostaa ensimmäisen pysäkin
            TulostaPysäkki(asemat, juna.timeTableRows[0]);

            Aikataulurivi edellinen = juna.timeTableRows[0];
            // Tulostaa muut paitsi ensimmäisen ja viimeisen pysäkin
            foreach (Aikataulurivi pysähdys in juna.timeTableRows.GetRange(1, juna.timeTableRows.Count() - 2))
            {

                if (pysähdys.trainStopping && pysähdys.commercialStop)
                {
                    if (pysähdys.stationShortCode == edellinen.stationShortCode && edellinen.type == "ARRIVAL" &&
                        pysähdys.type == "DEPARTURE")
                    {
                        TulostaPysäkki(asemat, pysähdys, false, edellinen);
                    }
                    edellinen = pysähdys;
                }
            }
            // Tulostaa viimeisen pysäkin
            TulostaPysäkki(asemat, juna.timeTableRows[juna.timeTableRows.Count - 1]);
        }


        /// <summary>
        /// Tulostaa junan koko reitin pysäkkien tiedot korostaen lähtö- ja pääteasemaa.
        /// </summary>
        /// <param name="juna">junaa kuvaava olio.</param>
        /// <param name="lähtöasema">haetun lähtöaseman koodi</param>
        /// <param name="pääteasema">haetun määränpääaseman koodi</param>
        /// <param name="asemat">asemien koodit ja koko nimet</param>
        public static void TulostaJunanPysäkkienTiedot(Juna juna, string lähtöasema, string pääteasema, Dictionary<string, string> asemat)
        {
            Aikataulurivi edellinen = juna.timeTableRows[0];
            Console.WriteLine("\njunan reitti:");
            Console.WriteLine($"{"pysäkki",-20} {"asemalla",-16} {"pysähdyksen kesto",-18} {"laituri",-3}");

            // tulostaa ensimmäisen reitin pysäkin
            TarkistaJaTulostaPysäkki(asemat, lähtöasema, pääteasema, juna.timeTableRows[0], true);

            // tulostaa muut paitsi ensimmäisen ja viimeisen pysäkin
            foreach (Aikataulurivi pysähdys in juna.timeTableRows.GetRange(1, juna.timeTableRows.Count() - 2))
            {
                if (pysähdys.trainStopping && pysähdys.commercialStop)
                {
                    if (pysähdys.stationShortCode == edellinen.stationShortCode && edellinen.type == "ARRIVAL" &&
                        pysähdys.type == "DEPARTURE")
                    {
                        TarkistaJaTulostaPysäkki(asemat, lähtöasema, pääteasema, pysähdys, false, false, edellinen);
                    }
                    edellinen = pysähdys;
                }
            }
            // tulostaa viimeisen pysäkin tiedot
            TarkistaJaTulostaPysäkki(asemat, lähtöasema, pääteasema, juna.timeTableRows[juna.timeTableRows.Count - 1], false, true);
        }


        /// <summary>
        /// Tulostaa pysäkin tiedot, jos juna pysähtyy asemalla ja se on tarkoitettu asiakkaille.
        /// </summary>
        /// <param name="asemat">asemien koodit ja koko nimet</param>
        /// <param name="pysähdys">pysähdystä(/sijaintia) kuvaava olio</param>
        /// <param name="EkaTaiVikapysähdys">true, jos eka tai vika pysähdys</param>
        /// <param name="edellinen">nykyisitä pysähdystä(/sijaintia) edeltävä pysähdys(/sijainti) olio.</param>
        private static void TulostaPysäkki(Dictionary<string, string> asemat, Aikataulurivi pysähdys, bool EkaTaiVikapysähdys = true, Aikataulurivi edellinen = null)
        {
            if (pysähdys.trainStopping && pysähdys.commercialStop)
            {
                string pysähdyksenkesto;
                string pysäkillä;

                if (EkaTaiVikapysähdys)
                {
                    pysähdyksenkesto = "";
                    pysäkillä = $"{pysähdys.scheduledTime.ToLocalTime().ToString("H:mm")}";
                }
                else
                {
                    pysähdyksenkesto = (pysähdys.scheduledTime - edellinen.scheduledTime).TotalMinutes.ToString() + " min";
                    pysäkillä = $"{edellinen.scheduledTime.ToLocalTime().ToString("H:mm")} -" +
                        $" {pysähdys.scheduledTime.ToLocalTime().ToString("H:mm")}";
                }
                Trace.WriteLine(pysähdys.stationShortCode);
                Console.WriteLine($"{asemat[pysähdys.stationShortCode],-20} {pysäkillä,-16} {pysähdyksenkesto,-18}" +
                                $" {pysähdys.commercialTrack,-3}");
            }
        }


        /// <summary>
        /// Kutsuu TulostaPysäkki-metodia, ja korostaa haun perusteella määritetyt lähtö- ja pääteasemat.
        /// Jos asema on merkitty lähtö- tai pääteasemaksi, tulostaa sen korostetusti.
        /// </summary>
        /// <param name="asemat">asemien koodit ja koko nimet</param>
        /// <param name="lähtöasema">haussa valittu lähtöasema</param>
        /// <param name="pääteasema">haussa valittu pääteasema</param>
        /// <param name="pysähdys">pysähdystä(/sijaintia) kuvaava olio</param>
        /// <param name="reitinEnsimmäinenpysähdys">true, jos junan koko reitin ensimmäinen pysähdys(/sijainti)</param>
        /// <param name="reitinViimeinenPysähdys">true, jos junan koko reitin viimeinen pysähdys(/sijainti)</param>
        /// <param name="edellinen">nykyisitä pysähdystä(/sijaintia) edeltävä pysähdys(/sijainti) olio.</param>
        private static void TarkistaJaTulostaPysäkki(Dictionary<string, string> asemat, string lähtöasema, string pääteasema, Aikataulurivi pysähdys,
            bool reitinEnsimmäinenpysähdys = false, bool reitinViimeinenPysähdys = false, Aikataulurivi edellinen = null)
        {
            if ((reitinEnsimmäinenpysähdys && lähtöasema == pysähdys.stationShortCode)
                || (reitinViimeinenPysähdys && pääteasema == pysähdys.stationShortCode))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                TulostaPysäkki(asemat, pysähdys);
                Console.ResetColor();
            }
            else if ((reitinEnsimmäinenpysähdys && lähtöasema != pysähdys.stationShortCode)
                || (reitinViimeinenPysähdys && pääteasema != pysähdys.stationShortCode))
            {
                TulostaPysäkki(asemat, pysähdys);
            }
            else
            {
                if (lähtöasema == pysähdys.stationShortCode | pääteasema == pysähdys.stationShortCode)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    TulostaPysäkki(asemat, pysähdys, false, edellinen);
                    Console.ResetColor();
                }
                else
                {
                    TulostaPysäkki(asemat, pysähdys, false, edellinen);
                }
            }
        }
    }
}
