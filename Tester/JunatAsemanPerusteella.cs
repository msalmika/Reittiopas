using RataDigiTraffic.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public class JunatAsemanPerusteella
    {
        /// <summary>
        /// Tulostaa annetulta asemalta lähtevät ja saapuvat junat
        /// </summary>
        /// <param name="asema">aseman nimi</param>
        public static void TulostaAsemanJunat(string asema, Dictionary<string, string> asemat)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Juna> junat = rata.AsemanJunat(asema);
            Console.WriteLine();
            Console.WriteLine($"Asemalta {asemat[asema]} lähtevät ja saapuvat junat: ");
            Console.WriteLine();
            Console.WriteLine("LÄHTEVÄT JUNAT:");
            foreach (var juna in junat.OrderBy(x => x.timeTableRows.Where(x => x.stationShortCode == asema).Select(x => x.scheduledTime).FirstOrDefault()))
            {
                foreach (var t in juna.timeTableRows)
                {
                    if (t.type == "DEPARTURE" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Commuter"
                        || t.type == "DEPARTURE" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Long-distance")
                    {
                        if (t.differenceInMinutes != 0)
                        {
                            Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(),-8}{juna.trainType + juna.trainNumber,-8} " +
                            $"Määränpää: {asemat[juna.timeTableRows[^1].stationShortCode],-15} " +
                            $"Laituri: {t.commercialTrack,-4}" +
                            $"Poikkeama aikataulusta: {t.differenceInMinutes} minuuttia");
                        }
                        else
                        {
                            Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(),-8}{juna.trainType + juna.trainNumber,-8} " +
                            $"Määränpää: {asemat[juna.timeTableRows[^1].stationShortCode],-15} " +
                            $"Laituri: {t.commercialTrack}");
                        }
                    }
                }
            }

            Console.WriteLine();
            Console.WriteLine("SAAPUVAT JUNAT:");
            foreach (var juna in junat.OrderBy(x => x.timeTableRows.Where(x => x.stationShortCode == asema).Select(x => x.scheduledTime).LastOrDefault()))
            {
                foreach (var t in juna.timeTableRows)
                {
                    if (t.type == "ARRIVAL" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Commuter"
                        || t.type == "ARRIVAL" && t.commercialStop == true && t.cancelled == false
                        && t.stationShortCode.Equals(asema) && juna.trainCategory == "Long-distance")
                    {
                        if (t.differenceInMinutes != 0)
                        {
                            Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(),-8}{juna.trainType + juna.trainNumber,-8} " +
                            $"Lähtöasema: {asemat[juna.timeTableRows[0].stationShortCode], -15} " +
                            $"Laituri: {t.commercialTrack,-4} " +
                            $"Poikkeama aikataulusta: {t.differenceInMinutes} minuuttia");
                        }
                        else
                        {
                            Console.WriteLine($"{t.scheduledTime.ToLocalTime().ToShortTimeString(),-8}{juna.trainType + juna.trainNumber,-8} " +
                                $"Lähtöasema: {asemat[juna.timeTableRows[0].stationShortCode],-15} " +
                                $"Laituri: {t.commercialTrack}");
                        }
                    }
                }
            }
        }
    }
}
