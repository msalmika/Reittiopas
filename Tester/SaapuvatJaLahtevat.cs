using RataDigiTraffic.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    public class SaapuvatJaLahtevat
    {

        public const string lähtevä = "DEPARTURE";
        public const string saapuva = "ARRIVAL";

        /// <summary>
        /// Tulostaa asemalle saapuvat junat tietyn ajankohdan jälkeen.
        /// </summary>
        /// <param name="asema"> Aseman nimi </param>
        /// <param name="tulostettavienLkm"> Haettavien junien määrä </param>
        /// <param name="pvm"> Päivämäärä </param>
        /// <param name="klo"> Kellon aika </param>
        public static void TulostaLähtevät(string asema, int tulostettavienLkm, string pvm = "", string klo = "")
        {
            Dictionary<string, string> asemat = Apufunktiot.HaeAsemat();
            asema = Apufunktiot.EtsiAsemaTunnus(asema, asemat);
            DateTime haunAloitus = HaunAloitus(pvm, klo);
            if (haunAloitus == default)
            {
                Console.WriteLine("Päivämäärä tai kellonaika väärässä muodossa, yritä uudelleen.");
                return;
            }
            List<Juna> junat = PäivänKaikkiJunat(pvm);
            if (junat.Count == 0)
            {
                Console.WriteLine("Ei lähteviä junia valittuna päivänä.");
                return;
            }
            Console.WriteLine($"\nAsemalta {asema} ajankohdasta {haunAloitus} eteenpäin lähtevät junat:\n");
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
                        Console.WriteLine($"{asemat[aikataulutieto.stationShortCode], -20}==>     {asemat[ju.timeTableRows[^1].stationShortCode], -20}" +
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
        public static void TulostaSaapuvat(string asema, int tulostettavienLkm, string pvm = "", string klo = "")
        {
            Dictionary<string, string> asemat = Apufunktiot.HaeAsemat();
            asema = Apufunktiot.EtsiAsemaTunnus(asema, asemat);
            DateTime haunAloitus = HaunAloitus(pvm, klo);
            if (haunAloitus == default)
            {
                Console.WriteLine("Päivämäärä tai kellonaika väärässä muodossa, yritä uudelleen.");
                return;
            }
            var junat = PäivänKaikkiJunat(pvm);
            if (junat.Count == 0)
            {
                Console.WriteLine("Ei saapuvia junia valittuna päivänä.");
                return;
            }
            Console.WriteLine($"\nAsemalle {asema} ajankohdasta {haunAloitus} eteenpäin saapuvat junat:\n");
            bool riittää = false;
            int i = 0;
            foreach (var ju in junat.OrderBy(j => j.timeTableRows.Where(j => j.stationShortCode == asema && j.type == saapuva).Select(x => x.scheduledTime).FirstOrDefault()))
            {
                foreach (var aikataulutieto in ju.timeTableRows)
                {
                    if ((ju.trainCategory == "Commuter" || ju.trainCategory == "Long-distance") && aikataulutieto.commercialStop == true
                        && ju.cancelled == false && aikataulutieto.stationShortCode.Equals(asema) && aikataulutieto.scheduledTime.ToLocalTime() > haunAloitus
                        && aikataulutieto.type == saapuva)
                    {
                        Console.WriteLine($"{asemat[ju.timeTableRows[0].stationShortCode], -20}==>     {asemat[aikataulutieto.stationShortCode], -20}" +
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

        /// <summary>
        /// Palauttaa ajanmääreen annettujen parametrien perusteella.
        /// </summary>
        /// <param name="pvm"> haun aloituspäivämäärä </param>
        /// <param name="klo"> haun aloitusaika </param>
        /// <returns> haun aloitusaika DateTime-muodossa</returns>
        private static DateTime HaunAloitus(string pvm = "", string klo = "")
        {
            DateTime haunAloitus;
            string mistaLahtien = pvm + " " + klo;
            string[] muotoilut = { "dd.MM.yyyy HH.mm", "dd.MM.yyyy ", " HH.mm" };
            if (mistaLahtien.Equals(" "))
                haunAloitus = DateTime.Now;
            else if (DateTime.TryParseExact(mistaLahtien, muotoilut, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime aika))
            {
                haunAloitus = aika;
            }
            else
            {
                haunAloitus = default;
            }
            return haunAloitus;
        }

        /// <summary>
        /// Metodi palauttaa listan päivän kulkevista junista annetun päivämäärän perusteella.
        /// </summary>
        /// <param name="pvm"> päivämäärä </param>
        /// <returns> Lista kulkevsta junista </returns>
        private static List<Juna> PäivänKaikkiJunat(string pvm)
        {
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            var hakuPvm = String.Join('-', pvm.Split('.').Reverse());
            List<Juna> junat = rata.SaapuvatJaLahtevat(hakuPvm);
            return junat;
        }
    }
}
