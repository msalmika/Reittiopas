using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DigiTrafficTester;

namespace Tester
{
    class SwitchCase
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
        public static void CaseN(string[] args, Dictionary<string, string> asemat)
        {
            string lähtöasema;
            string kohdeasema;
            if (args.Length < 3)
            {
                //HeikinMetodit.PrintUsage();
            }
            lähtöasema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);

            kohdeasema = args[2];

            Tester.SeuraavaSuoraJuna.TulostaSeuraavaSuoraJuna(lähtöasema, kohdeasema);
        }
        public static void CaseS(string[] args, Dictionary<string, string> asemat)
        {
            string asema;
            int lkm;
            string pvm;
            string aika;
            if (args.Length < 3)
            {
                //PrintUsage();
            }
            asema = args[1];
            lkm = Int32.Parse(args[2]);
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
                //PrintUsage();
                return;
            }
            asema = args[1];
            lkm = Int32.Parse(args[2]);
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
                //PrintUsage();
            }
            //asema = args[1];
            asema = Apufunktiot.EtsiAsemaTunnus(args[1], asemat);
            JunatAsemanPerusteella.TulostaAsemanJunat(asema, asemat);
        }
    }
}
