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
            while (true)
            {
                Alkunaytto();
                Console.Write(">");
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
                    case "-q":
                        SwitchCase.CaseQ(args, asemat);
                        break;
                    default:
                        break;
                }
                if (args[0].ToLower() == "-x") { break; }
                Console.WriteLine();
                Console.WriteLine("Paina 'Enter' jatkaaksesi tai syötä -x lopettaaksesi");
                if (Console.ReadLine() == "-x") { break; }
                Console.Clear();
            }
        }
        public static void Alkunaytto()
        {
            Console.WriteLine("===================================================================================");
            Console.WriteLine("|                Tervetuloa reittioppaaseen!                                      |");
            Console.WriteLine("|                                                                                 |");
            Console.WriteLine("|           Ohjeet:                                                               |");
            Console.WriteLine("|    -a[semat] <asemanAlkukirjain>                                                |");
            Console.WriteLine("|    -j[unat] alkuasema loppuasema                                                |");
            Console.WriteLine("|    -s[aapuvat] asema junienLkm pvm(dd.mm.yyyy) aika(22.59)                      |");
            Console.WriteLine("|    -m [asemalta lähtevät junat] asema                                           |");
            Console.WriteLine("|    -e[tsi juna] junanTyyppi junanNro (korvaa tyyppi merkillä ? jos ei tiedossa) |");
            Console.WriteLine("|    -n [tulosta seuraava suora juna] lähtöasema kohdeasema                       |");
            Console.WriteLine("|    -q [kaikki seuraavat suorat junat asemien välillä] lähtöasema kohdeasema     |");
            Console.WriteLine("|    -l[ähtevät] asema junienLkm pvm(dd.mm.yyyy) aika(22.59)                      |");
            Console.WriteLine("|    -k[aikki junat]                                                              |");
            Console.WriteLine("|    -t[iedotteet] <asema>                                                        |");
            Console.WriteLine("|    -r[ajoitukset] <asema>                                                       |");
            Console.WriteLine("|    tai                                                                          |");
            Console.WriteLine("|    -x lopeta                                                                    |");
            Console.WriteLine("|                                                                                 |");
            Console.WriteLine("|                                                                                 |");
            Console.WriteLine("|                                                                                 |");
            Console.WriteLine("===================================================================================");




        }
    }
}

