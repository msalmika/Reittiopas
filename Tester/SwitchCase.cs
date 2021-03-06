using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiTrafficTester;

namespace Tester
{
    public class SwitchCase
    {
        public static void CaseA(string[] args, Dictionary<string, string> asemat)
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
                HeikinMetodit.TulostaAsemat(asema);
            }
        }
        public static void CaseJ(string[] args, Dictionary<string, string> asemat)
        {
            string lähtöasema;
            string kohdeasema;
            //if (args.Length < 3)
            //{
            //    PrintUsage();
            //    return;
            //}
            lähtöasema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
            kohdeasema = Apufunktiot.EtsiAsemaTunnus(args[2], asemat);
            HeikinMetodit.TulostaJunatVälillä(lähtöasema, kohdeasema);
        }
        public static void CaseK(string[] args, Dictionary<string, string> asemat)
        {
            MarkonMetodit.TulostaLiikkuvatJunat(asemat);
        }
        public static void CaseE(string[] args, Dictionary<string, string> asemat)
        {

            if (args.Length >= 3)
            {
                string junatype = args[1];
                if (junatype == "?")
                {
                    junatype = "";
                }
                int junanro = int.Parse(args[2]);
                MarkonMetodit.TulostaEtsittyJuna(junatype, junanro, asemat);
            }
            else
            {
                Console.WriteLine("Parametrit puutteelliset, tarkista syötä");
            }
        }
           
        public static void CaseR(string[] args, Dictionary<string, string> asemat)
        {
            if (args.Length == 1)
            {
                MarkonMetodit.TulostaRajoitukset();
            }
            else
            {
                MarkonMetodit.TulostaRajoitukset(args[1]);
            }
        }
        
        public static void CaseT(string[] args, Dictionary<string, string> asemat)
        {
            if (args.Length == 1)
            {
                MarkonMetodit.TulostaTiedotteet();
            }
            else
            {
                MarkonMetodit.TulostaTiedotteet(args[1]);
            }
        }

        /// <summary>
        /// Tulostaa kaikki seuraavat junat kahden aseman välillä (24h)
        /// </summary>
        /// <param name="args">käyttäjän syöte</param>
        /// <param name="asemat">tunnetut asemat ja niiden koodit, koodi avain, koko nimi arvo</param>
        public static void CaseQ(string[] args, Dictionary<string, string> asemat)
        {
            if(args.Length == 3)
            {
                string lähtöasema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
                string kohdeasema = Apufunktiot.EtsiAsemaTunnus(args[2], asemat);
                SeuraavaSuoraJuna.TulostaSuoratJunat(lähtöasema, kohdeasema, asemat);
            }
            else
            {
                Console.WriteLine("Liikaa tai liian vähän argumentteja. \nEtsitylle reitille tulee antaa lähtö- ja pääteasema.");
            }
                
        }

        /// <summary>
        /// Tulostaa suorat junat
        /// </summary>
        /// <param name="args">käyttäjän antama syöte taulukkona</param>
        /// <param name="asemat">käytettävissä olevat asemat aseman koodi - nimi pareina</param>
        public static void CaseN(string[] args, Dictionary<string, string> asemat)
        {
            string lähtöasema;
            string kohdeasema;

           if(args.Length == 3)
            {
                lähtöasema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
                kohdeasema = Apufunktiot.EtsiAsemaTunnus(args[2], asemat);
                SeuraavaSuoraJuna.TulostaSeuraavaSuoraJuna(lähtöasema, kohdeasema, asemat);
            }
            else
            {
                Console.WriteLine("Liikaa tai liian vähän argumentteja. \nEtsitylle reitille tulee antaa lähtö- ja pääteasema.");
            }
            
        }

        public static void CaseS(string[] args, Dictionary<string, string> asemat)
        {
            string asema;
            int lkm;
            string pvm;
            string aika;
            if (args.Length < 3)
            {
                return;
            }
            asema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
            lkm = Int32.Parse(args[2]);
            if (args.Length == 3)
            {
                SaapuvatJaLahtevat.TulostaSaapuvat(asema, lkm);
            }
            if (args.Length == 4)
            {
                pvm = args[3];
                SaapuvatJaLahtevat.TulostaSaapuvat(asema, lkm, pvm);
            }
            if (args.Length == 5)
            {
                pvm = args[3];
                aika = args[4];
                SaapuvatJaLahtevat.TulostaSaapuvat(asema, lkm, pvm, aika);
            }
        }
        public static void CaseL(string[] args, Dictionary<string, string> asemat)
        {
            string asema;
            int lkm;
            string pvm;
            string aika;
            if (args.Length < 3)
            {
                return;
            }
            asema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
            lkm = Int32.Parse(args[2]);
            if (args.Length == 3)
            {
                SaapuvatJaLahtevat.TulostaLähtevät(asema, lkm);
            }
            if (args.Length == 4)
            {
                pvm = args[3];
                SaapuvatJaLahtevat.TulostaLähtevät(asema, lkm, pvm);
            }
            if (args.Length == 5)
            {
                pvm = args[3];
                aika = args[4];
                SaapuvatJaLahtevat.TulostaLähtevät(asema, lkm, pvm, aika);
            }
        }
        public static void CaseM(string[] args, Dictionary<string, string> asemat)
        {
            string asema = "";
            if (args.Length < 2)
            {
                asema = Apufunktiot.EtsiAsemaTunnus("", asemat);
            }
            else
            {
                asema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
            }
            try
            {
                JunatAsemanPerusteella.TulostaAsemanJunat(asema, asemat);
            }
            catch (Exception)
            {

                Console.WriteLine("Asemaa ei löytynyt");
            }
        }

    }
}
