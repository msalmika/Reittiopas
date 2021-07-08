using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RataDigiTraffic.Model;


namespace Tester
{
    public class HeikinMetodit
    {
        public static void TulostaJunatVälillä(string lähtöasema, string kohdeasema)
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
        public static void TulostaAsemat(string asema)
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
        public static void PrintUsage()
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
