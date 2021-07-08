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
                    Alkunaytto();
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
                    Console.WriteLine();
                    Console.WriteLine("Paina 'Enter' jatkaaksesi");
                    Console.ReadLine();
                    Console.Clear();
                }
            }
        }
        public static void Alkunaytto()
        {
            Console.WriteLine("Tervetuloa reittioppaaseen!");
            Console.WriteLine();
            Console.WriteLine("Ohje:");
            Console.WriteLine("-a[semat] <asemanAlkukirjain>");
            Console.WriteLine("-j[unat] alkuasemaLyhenne loppuasemaLyhenne");
            Console.WriteLine("-s[aapuvat] asemaLyhenne");
            Console.WriteLine("-m [asemalta lähtevät junat] asema");
            Console.WriteLine("-e[tsi juna] junanTyyppi junanNro (korvaa tyyppi merkillä ? jos ei tiedossa) ");
            Console.WriteLine("-n [tulosta seuraava suora juna] lähtöasema kohdeasema");
            Console.WriteLine("-l[ähtevät] asema junienLkm pvm(dd.mm.yyyy) aika(22.59)");
            Console.WriteLine("-k[aikki junat]");
            Console.WriteLine("-t[iedotteet]");
            Console.WriteLine("-r[ajoitukset]");
            Console.WriteLine("tai");
            Console.WriteLine("-x lopeta");
            Console.WriteLine();
        }
    }
}

