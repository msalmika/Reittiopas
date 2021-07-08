using System;
using RataDigiTraffic.Model;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Tester;
using System.Text.RegularExpressions;
using System.Collections;
using System.Globalization;

namespace DigiTrafficTester
{
    class Program
    {
        
        static void Main(string[] args)
        {
            Dictionary<string, string> asemat = Apufunktiot.HaeAsemat();

            if (args.Length == 0)
            {

                
                while (true)
                {
                    Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää], -l [asema][lkm][pvm][aika], -s[asema][lkm][pvm][aika]");
                    Console.WriteLine("-a printtaa asemat, -j [lähtöasema][määränpää], -m [asema] asemalta lähtevät ja saapuvat junat");
                    args = Console.ReadLine().Split(" ");
                    switch (args[0].ToLower())
                    {
                        case "-a":
                            SwitchCase.CaseA(args, asemat);
                            break;
                        case "-j":
                            SwitchCase.CaseJ(args, asemat);
                            break;
                        case "-k":
                            SwitchCase.CaseK(args, asemat);
                            break;
                        case "-e":
                            SwitchCase.CaseE(args, asemat);
                            break;
                        case "-r":
                            SwitchCase.CaseR(args, asemat);
                            break;
                        case "-t":
                            SwitchCase.CaseT(args, asemat);
                            break;
                        case "-n":
                            SwitchCase.CaseN(args, asemat);
                            break;
                        case "-s":
                            SwitchCase.CaseS(args, asemat);
                            break;
                        case "-l":
                            SwitchCase.CaseL(args, asemat);
                            break;
                        case "-m":
                            SwitchCase.CaseM(args, asemat);
                            break;
                        default:
                            break;
                    }
                    if (args[0].ToLower() == "-x") { break; }
                }
                //PrintUsage();
                //return;
            }
            //if (args[0].ToLower().StartsWith("-a"))
            //{
            //    string asema = "";
            //    if (args.Length > 1)
            //    {
            //        asema = args[1];
            //    }
            //    if (asemat.Where(a => a.Key.Contains(asema)).Count() == 0)
            //    {
            //        Console.WriteLine("Kyseisillä ehdoilla ei löytynyt asemia");
            //    }
            //    else
            //    {
            //        TulostaAsemat(asema);
            //    }
            //    return;
            //}
            //if (args[0].ToLower().StartsWith("-j"))
            //{
            //    string lähtöasema;
            //    string kohdeasema;
            //    //if (args.Length < 3)
            //    //{
            //    //    PrintUsage();
            //    //    return;
            //    //}
            //    lähtöasema = args[1];
            //    kohdeasema = args[2];
            //    TulostaJunatVälillä(lähtöasema, kohdeasema);
            //}
            //if (args[0].ToLower().StartsWith("-lj"))
            //{
            //    MarkonMetodit.TulostaLiikkuvatJunat(asemat);
            //    return;
            //}
            //if (args[0].ToLower().StartsWith("-e"))
            //{
            //    string junatype = args[1];
            //    int junanro = int.Parse(args[2]);
            //    MarkonMetodit.TulostaEtsittyJuna(junatype, junanro, asemat);
            //    return;
            //}
            //if (args[0].ToLower().StartsWith("-r"))
            //{
            //    MarkonMetodit.TulostaRajoitukset();
            //    return;
            //}
            //if (args[0].ToLower().StartsWith("-t"))
            //{
            //    MarkonMetodit.TulostaTiedotteet();
            //    return;
            //}
            //if (args[0].ToLower().StartsWith("-n"))
            //{
            //    string lähtöasema;
            //    string kohdeasema;
            //    if (args.Length < 3)
            //    {
            //        PrintUsage();
            //        return;
            //    }
            //    lähtöasema = args[1];
            //    kohdeasema = args[2];

            //    Tester.SeuraavaSuoraJuna.TulostaSeuraavaSuoraJuna(lähtöasema, kohdeasema);

            //}

            //if (args[0].ToLower().StartsWith("-s"))
            //{
            //    string asema;
            //    int lkm;
            //    string pvm;
            //    string aika;
            //    if (args.Length < 3)
            //    {
            //        PrintUsage();
            //        return;
            //    }
            //    asema = args[1];
            //    lkm = Int32.Parse(args[2]);
            //    if (args.Length == 4)
            //    {
            //        pvm = args[3];
            //        TulostaSaapuvat(asema, lkm, pvm);
            //    }
            //    if (args.Length == 5)
            //    {
            //        pvm = args[3];
            //        aika = args[4];
            //        TulostaSaapuvat(asema, lkm, pvm, aika);
            //    }
            //}
            //if (args[0].ToLower().StartsWith("-l"))
            //{
            //    string asema;
            //    int lkm;
            //    string pvm;
            //    string aika;
            //    if (args.Length < 3)
            //    {
            //        PrintUsage();
            //        return;
            //    }
            //    asema = args[1];
            //    lkm = Int32.Parse(args[2]);
            //    if (args.Length == 4)
            //    {
            //        pvm = args[3];
            //        TulostaLähtevät(asema, lkm, pvm);
            //    }
            //    if (args.Length == 5)
            //    {
            //        pvm = args[3];
            //        aika = args[4];
            //        TulostaLähtevät(asema, lkm, pvm, aika);
            //    }

            //}
            //if (args[0].ToLower().StartsWith("-m"))
            //{
            //    string asema = "";
            //    if (args.Length < 2)
            //    {
            //        PrintUsage();
            //        return;
            //    }
            //    asema = args[1];
            //    JunatAsemanPerusteella.TulostaAsemanJunat(asema);
            //}
        }

        

        
    }
}

