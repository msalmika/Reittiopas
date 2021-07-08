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
            lähtöasema = args[1];
            kohdeasema = args[2];
            HeikinMetodit.TulostaJunatVälillä(lähtöasema, kohdeasema);
        }
        public static void CaseK(string[] args, Dictionary<string, string> asemat)
        {
            MarkonMetodit.TulostaLiikkuvatJunat(asemat);
        }
        public static void CaseE(string[] args, Dictionary<string, string> asemat)
        {
            string junatype = args[1];
            int junanro = int.Parse(args[2]);
            MarkonMetodit.TulostaEtsittyJuna(junatype, junanro, asemat);
        }
        public static void CaseR(string[] args, Dictionary<string, string> asemat)
        {
            MarkonMetodit.TulostaRajoitukset();
        }
        public static void CaseT(string[] args, Dictionary<string, string> asemat)
        {
            MarkonMetodit.TulostaTiedotteet();
        }
        public static void CaseN(string[] args, Dictionary<string, string> asemat)
        {
            string lähtöasema;
            string kohdeasema;
            if (args.Length < 3)
            {
                //HeikinMetodit.PrintUsage();
            }
            lähtöasema = args[1];
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
            asema = args[1];
            JunatAsemanPerusteella.TulostaAsemanJunat(asema);
        }
    }
}
