using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RataDigiTraffic.Model;


namespace Tester
{
    static public class Apufunktiot
    {
        /// <summary>
        /// Haetaan rautatieverkoston asemat ja tehdään niistä sanakirja
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> HaeAsemat()
        {

            Dictionary<string, string> asemat = new Dictionary<string, string>();
            RataDigiTraffic.APIUtil rata = new RataDigiTraffic.APIUtil();
            List<Liikennepaikka> paikat = rata.Liikennepaikat();
            foreach (var item in paikat.Where(p => p.type == "STATION"))
            {
                asemat.Add(item.stationShortCode, item.stationName.Substring(0, 1).ToUpper() + item.stationName.Substring(1).ToLower());
            }
            return asemat;
        }
        /// <summary>
        /// Koordinaatiston etäisyyksien laskeminen.
        /// </summary>
        /// <param name="a_lat">aseman latitude </param>
        /// <param name="a_long">aseman longitude </param>
        /// <param name="latitude">kohteen x latitude </param>
        /// <param name="longitude">kohteen y longitude </param>
        /// <returns> Palauttaa aseman ja kohteen x koordinaattien välisen etäisyyden. </returns>
        public static double PisteidenEtaisyys(double a_lat, double a_long, double latitude, double longitude)
        {

            var x_ero = Math.Pow(a_lat - latitude, 2);
            double y_ero = Math.Pow(a_long - longitude, 2);
            double res = Math.Sqrt(y_ero + x_ero);
            return res;
        }
        /// <summary>
        /// Apufunktio asemien hakemiseksi ja muotoiluun.
        /// </summary>
        /// <param name="asema">Etsittävä asema</param>
        /// <param name="asemat">Sanakirja asemoista ja niiden lyhenteistä</param>
        /// <returns>Palauttaa aseman lyhenteen</returns>
        public static string EtsiAsemaTunnus(string asema, Dictionary<string, string> asemat)
        {
            if (asemat.ContainsKey(asema.ToUpper())) { return asema; }
            else
            {
                var query = from a in asemat
                            where a.Value.ToUpper().StartsWith(asema.ToUpper())
                            select a;
                var stations = query.ToList();
                int input = 0;
                bool onnistuiko = false;
                if (query.Count() == 1) { return query.First().Key.ToUpper(); }
                else if (query.Count() > 1)
                {
                    int jnro = 0;
                    while (onnistuiko == false)
                    {
                        Console.WriteLine("Löytyi useampi hakuehdot täyttävä vaihtoehto. Valitse listalta haluamasi asema: ");
                        foreach (var s in stations)
                        {
                            Console.WriteLine($"{s.Value}: valitse {jnro + 1}");
                            jnro++;
                        }
                        onnistuiko = int.TryParse(Console.ReadLine(), out input);
                        if (onnistuiko == false)
                        {
                            Console.WriteLine("Arvoa ei löytynyt listalta, tarkista syöte.");
                        }
                    }
                    return stations[input - 1].Key.ToUpper();
                }
            }
            return "Ei löytynyt";
        }

    }
}
