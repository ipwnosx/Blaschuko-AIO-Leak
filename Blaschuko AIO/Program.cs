using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using Leaf.xNet;
using System.Net;
using System.Text;
using System.Windows;
using System.Diagnostics;
using Newtonsoft.Json;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;
using Color = System.Drawing.Color;
using System.Windows.Controls.Primitives;
using Newtonsoft.Json.Linq;
using System.Web.UI.WebControls.WebParts;
using System.Security.Cryptography;
using System.Management;
using System.Diagnostics.Eventing.Reader;
using System.Collections.Specialized;
using System.ComponentModel.Design;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Collections;

namespace Blaschuko_AIO
{
    class Program
    {

        public static List<string> Lines = new List<string>();
        public static string Username;
        public static string Email;
        public static string HWID;
        public static string Register_Date;
        public static bool verified = false;
        public static string id_choosed;
        public static bool load_messages = false;
        public static int refresh_time = 300;
        public static string Halloween;


        [STAThread]
        static void Main(string[] args)
        {
                Config.LoadConfig();
            if (Config.discord_id != "none")
            {
                verified = true;
            }
            Console.Title = "BazookaAIO V2.2 | Menu";
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Connecting to the Bazooka servers...", Check.MainColor);
            OnProgramStart.Initialize("Bazooka AIO V2", "728796", "QnsdxTy5swdQ9ePFF138CroluvD365iq7al", "1.1");
            if (!ApplicationSettings.Status)
            {
                System.Windows.MessageBox.Show("TPS AIO isn't avaible right now, please contact support!", OnProgramStart.Name, MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
            Console.WriteLine("");
            Colorful.Console.WriteLine("Successfully connected to the Bazooka servers", Check.MainColor);


            Thread.Sleep(1000);
            


            Modules.LoadModules();

            if (Atologin() || AskLogin()) 
            {
                Halloween = App.GrabVariable("eyNoQfPNMKnQhsEw86eucvfgHkuyu");
                if (Halloween == "p")
                {
                    Console.Clear();
                    PrintHallowenLogo();
                    Console.WriteLine();
                    centerText("Happy Halloween to everyone!");
                    centerText("Have a good time!");
                    Console.WriteLine();
                    centerText(" - Blaschuko & Freeload24");
                    Console.WriteLine("\n\nPress any key to continue...");
                    Console.ReadKey(true);
                }
                new Thread(runBot).Start();
                menu();

            }
        }


        public static bool Atologin()
        {
            if (File.Exists("LoginDetails.xml"))
            {
                string[] first = File.ReadAllLines("LoginDetails.xml");
                for (int i = 0; i < (int)first.Length; i++)
                {
                    string str = first[i];
                    string[] array = str.Split(new char[]
                    {
                        ':'
                    });
                    if (API.Login(array[0], array[1]))
                    {
                        Console.Clear();
                        PrintLogo();
                        Console.WriteLine("");
                        Console.WriteLine($"Welcome back, {User.Username}...");
                        Username = User.Username;
                        Email = User.Email;
                        HWID = User.HWID;
                        Register_Date = User.RegisterDate;
                        //dWebHook webHook = new dWebHook();
                        //webHook.SendMessage($"{User.Username} just logged in to BazookaAIO V2.1");
                        Thread.Sleep(2500);
                        return true;
                    }
                    else
                    {


                    }
                }
            }
            return false;
        }


        public static void centerText(String text)
        {
            Console.Write(new string(' ', (Console.WindowWidth - text.Length) / 2));
            Colorful.Console.WriteLine(text, ColorTranslator.FromHtml("#eb6123"));
        }



        public static string Route(string route) => Globals.RootUrl + route + ".php";

        public static void runBot()
        {
            Program program = new Program();
            try
            {
                program.MainAsync().GetAwaiter().GetResult();
            }
            catch
            {
                program.MainAsync().GetAwaiter().GetResult();
            }

        }


        public static int valid_hashes = 0;
        public static int invalid = 0;
        public static int counter_ = 0;

        public static void DeHasher()
        {

            string _filename;
            Console.Title = "BazookaAIO V2.2 | Dehasher";
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your hashed combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Dehasher | Choose your hashed combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                Lines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else
                return;

            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.Write($"Loaded ", Check.MainColor);
            Colorful.Console.Write(Lines.Count);
            Colorful.Console.WriteLine($" lines!", Check.MainColor);
            Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
            Console.ReadKey(true);
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine($"Please choose your file type:", Check.MainColor);
            Option("1", "Hashes only (hash)");
            Option("2", "Email:Hash / User:Hash");
            string typeChoice = Console.ReadKey(true).Key.ToString();
            bool _hashesOnly;

            switch (typeChoice)
            {
                case "D1":
                case "NumPad1":
                    _hashesOnly = true;
                    break;

                case "D2":
                case "NumPad2":
                    _hashesOnly = false;
                    break;

                default:
                    _hashesOnly = false;
                    break;
            }
            Console.Clear();
            PrintLogo();
            Console.WriteLine();
            int counter = 0;
            int loopcounter = 0;
            int expectedLoops = Lines.Count / 100;
            int expectedLinesLeft = Lines.Count % 100;
            string buildParams = "";

            new Thread(UpdateDehasherTitle).Start();
            foreach(string line in Lines)
            {
                if (_hashesOnly)
                {
                    buildParams += $"&search[]={line}";
                }
                else
                {
                    if (line.Contains(':'))
                    {
                        string[] splittedLines = line.Split(':');
                        string email = splittedLines[0];
                        string hash = splittedLines[1];

                        if (!string.IsNullOrWhiteSpace(email.Trim()) && !string.IsNullOrWhiteSpace(email.Trim()))
                        {
                            buildParams += $"&search[]={hash}";
                        }
                        else
                        {
                            Colorful.Console.WriteLine($"[/] {line} is broken, skipping...", Check.Frees_);
                        }
                    }
                    else
                    {
                        Colorful.Console.WriteLine($"[/] {line} is broken, skipping...", Check.Frees_);
                    }
                }

                counter++;
                if (counter == 100 || loopcounter == expectedLoops && counter == expectedLinesLeft)
                {
                    string[] splittedHashes = buildParams.Split(new[] { "&search[]=" }, StringSplitOptions.None).Skip(1).ToArray();
                    loopcounter++;
                    counter = 0;
                    NameValueCollection values = new NameValueCollection
                    {
                        ["username"] = "Blaschuko",
                        ["hwid"] = "D114-5D7A-8B15-7EBB-93E4-E192-68C5-8192",
                        ["params"] = buildParams
                    };

                    Globals.Response = Globals.SafeRequest.Request(Program.Route("dehasher"), values);

                    if (Globals.Response.status)
                    {
                        foreach (string hash in splittedHashes)
                        {
                            if (Globals.Response.message.Contains(hash))
                            {
                                counter_++;
                                valid_hashes++;
                                Colorful.Console.WriteLine($"[+] {hash}", Check.Goods_);
                            }
                            else
                            {
                                Colorful.Console.WriteLine($"[-] {hash}", Check.Bads_);
                                counter_++;
                                invalid++;
                            }
                        }

                        if (_hashesOnly)
                            SaveDehashed(Globals.Response.message, "Dehashed");
                        else
                        {
                            using (StringReader reader = new StringReader(Globals.Response.message))
                            {
                                string responseLine;
                                while ((responseLine = reader.ReadLine()) != null)
                                {
                                    string[] splittedResponse = responseLine.Split(':');
                                    string hash = splittedResponse[0];
                                    string decryptedHash = splittedResponse[1];

                                    foreach (string line2 in Globals.Lines)
                                    {
                                        if (line2.Contains(hash))
                                        {
                                            string newline = line2.Replace(hash, decryptedHash);
                                            SaveDehashed(newline, "Dehashed");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine(Globals.Response.message);
                    }
                    buildParams = "";
                    Array.Clear(splittedHashes, 0, splittedHashes.Length);
                }
            }

            y = false;
            return;
        }

        public static bool y = true;

        public static void UpdateDehasherTitle()
        {
            while (y)
            {
                Console.Title = $"BazookaAIO V2.2 | Dehasher - {counter_}/{Lines.Count} | Valid: {valid_hashes} | Bads: {invalid} | Made by Freeload24 and Blaschuko";
                Thread.Sleep(50);
            }
            Console.Title = "BazookaAIO V2.2 | Home";
        }

        public static void UtilsMenu()
        {
            for(; ; )
            {
                Console.Title = "BazookaAIO V2.2 | Utils Menu";
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Option("1", "Dehasher");
                Option("2", "Text Utils");
                Option("3", "Combo Editor");
                Option("4", "Go back");
                string answer = Console.ReadLine();
                if(answer == "1")
                {
                    DeHasher();
                } else if(answer == "2")
                {
                    TextUtils.TextUtilsMenu();
                } else if(answer == "3")
                {
                    ComboEditor.Menu();
                } else if(answer == "4")
                {
                    return;
                }
            }
        }

        

        public static void Home()
        {
            for (; ; )
            {
                Console.Title = "BazookaAIO V2.2 | Home";
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Option("1", "Modules");
                Option("2", "Proxy Scraper");
                Option("3", "Live chat");
                Option("4", "Nitro Sniper");
                Option("5", "Utils");
                Option("6", "Go back");
                string ans = Console.ReadLine().ToUpper();
                if (ans == "1")
                {
                    ShowModules();
                }
                else if (ans == "2")
                {
                    ScrapeProxies();
                }
                else if (ans == "6")
                {
                    return;
                } else if(ans == "5")
                {
                    UtilsMenu();
                }
                else if (ans == "3")
                {
                    LiveChat();
                } else if(ans == "4")
                {
                    NitroSniper.SniperMenu();
                }
                else
                {
                    Colorful.Console.WriteLine("\nPlease choose a valid option!", Check.MainColor);
                    Thread.Sleep(1500);
                }
            } 
        }

        static void UpdateLiveChat()
        {
            using (HttpRequest req = new HttpRequest())
            {
                req.IgnoreProtocolErrors = true;
                while (load_messages)
                {
                    string chat = req.Get("http://" + App.GrabVariable("sF9G2kFwhp3iNKOv753lYJ8zRdqjDd") + "/chat").ToString();
                    chat = chat.Replace("[", "");
                    chat = chat.Replace("]", "");
                    chat = chat.Replace("nigga", "awesome person");
                    chat = chat.Replace("nigger", "cute person");
                    chat = chat.Replace("dumb", "cool");
                    chat = chat.Replace("stupid", "intelligent");
                    chat = chat.Replace("\",", "");
                    chat = chat.Replace("\"", "");
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine(chat.Replace(@"\n", "\n"));
                    if(typing != "")
                    {
                        Console.WriteLine("\nYour message: " + typing);
                    }
                    Thread.Sleep(refresh_time);
                }
            }
        }

        public static string typing = "";

        private static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    if (cooldown_active) Console.WriteLine(" COOLDOWN !");
                }
                else break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
                typing = input.ToString();
            }
            typing = "";
            return input.ToString();
        }


        public static int cooldown = 0;
        public static bool cooldown_active = false;
        static void GetCooldown()
        {
            while (load_messages)
            {
                if (cooldown == 0) cooldown_active = false;
                else
                {
                    cooldown--;
                    cooldown_active = true;
                }
                Thread.Sleep(1000);
            }
        }

        static void LiveChat()
        {
            if (App.GrabVariable("sF9G2kFwhp3iNKOv753lYJ8zRdqjD") == "NO") return;
            Console.Title = "BazookaAIO v2.2 | Live Chat - type \"back\" to go back";
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("If you want to go back, type \"back\".", Check.MainColor);
            Colorful.Console.WriteLine("Please choose a refresh time for the live chat (300 MS default) [in milliseconds]: ", Check.MainColor);
            string amount = Console.ReadLine();
            try
            {
                refresh_time = Convert.ToInt32(amount);
            }
            catch
            {

            }
            load_messages = true;
            new Thread(UpdateLiveChat).Start();
            while (load_messages)
            {
                string message = GetHiddenConsoleInput();
                if(message == "back")
                {
                    load_messages = false;
                }
                else
                {
                    using(HttpRequest req = new HttpRequest())
                    {
                        req.IgnoreProtocolErrors = true;
                        req.AddHeader("Content-Type", "application/json");
                        string data = "{\"author\": \"" + Username + "\",\"message\": \"" + message + "\"}";
                        req.Post("http://" + App.GrabVariable("sF9G2kFwhp3iNKOv753lYJ8zRdqjDd") + "/chat", new BytesContent(Encoding.Default.GetBytes(data)));
                    }
                }

            }
            return;
        }

        public static void ScrapeBazookaSources()
        {

            for (; ; )
            {

                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Colorful.Console.WriteLine("What type of proxies do you want to scrape [HTTP / SOCKS4 / SOCKS5]:", Check.MainColor);
                string ans = Console.ReadLine().ToLower();
                string key = App.GrabVariable("ProxyGrabber");
                using (HttpRequest req = new HttpRequest())
                {
                    req.IgnoreProtocolErrors = true;
                    req.ConnectTimeout = 8000;
                    req.KeepAliveTimeout = 10000;
                    req.ReadWriteTimeout = 10000;

                    req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback,
                    new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));
                    if (ans == "http")
                    {
                        string url = "http://proxy.bluecode.info/proxy_api/?access_key=" + key + "&action=getproxy&proxy_type=" + ans + "&proxy_rate=2&proxy_country=ALL";
                        string proxies = req.Get(new Uri(url)).ToString();
                        Save(proxies, "HTTP");
                        return;
                    }
                    else if (ans == "socks4")
                    {
                        string url = "http://proxy.bluecode.info/proxy_api/?access_key=" + key + "&action=getproxy&proxy_type=" + ans + "&proxy_rate=2&proxy_country=ALL";
                        string proxies = req.Get(new Uri(url)).ToString();
                        Save(proxies, "SOCKS4");
                        return;
                    }
                    else if (ans == "socks5")
                    {
                        string url = "http://proxy.bluecode.info/proxy_api/?access_key=" + key + "&action=getproxy&proxy_type=" + ans + "&proxy_rate=2&proxy_country=ALL";
                        string proxies = req.Get(new Uri(url)).ToString();
                        Save(proxies, "SOCKS5");
                        return;
                    }
                    else
                    {
                        Colorful.Console.WriteLine("\nPlease choose a valid option!", Check.MainColor);
                        Thread.Sleep(1500);
                    }
                }

            }
        }

        public static List<string> Sources = new List<string>();
        public static Regex REGEX = new Regex(@"\b(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\:[0-9]{1,5}\b");

        public static void ScrapeCustomSources()
        {
            for(; ; )
            {
                Sources.Clear();
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                WebClient _WC = new WebClient();
                Colorful.Console.WriteLine("Press any key to load your sources...", Check.MainColor);
                Console.ReadKey(true);

                OpenFileDialog file = new OpenFileDialog
                {
                    Title = "BazookaAIO V2.2 | Text Utils | Choose a text file!",
                    Filter = "Text Files | *.txt",
                    FilterIndex = 2,
                    RestoreDirectory = true
                };

                if (file.ShowDialog() == DialogResult.OK)
                {
                    Sources = File.ReadLines(file.FileName).ToList();
                }
                else
                    return;

                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Colorful.Console.Write("Scraping proxies from ", Check.MainColor);
                Console.Write(Sources.Count);
                Colorful.Console.WriteLine(" URLs...", Check.MainColor);

                List<string> proxies = new List<string>();

                try
                {
                    foreach (string source in Sources)
                    {
                        string unParsedWebSource = _WC.DownloadString(source);

                        MatchCollection _MC = REGEX.Matches(unParsedWebSource);
                        foreach (Match proxy in _MC)
                        {
                            proxies.Add(proxy.ToString());
                        }
                    }
                }
                catch
                {

                }
                foreach(string proxy in proxies)
                {
                    Save(proxy, "Custom Sources");
                }


            }
        }



            public static void ScrapeProxies()
        {
            for (; ; )
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Option("1", "Bazooka Sources");
                Option("2", "Custom Sources");
                Option("3", "Go Back");
                string option = Console.ReadLine();
                if (option == "1") ScrapeBazookaSources();
                else if (option == "2") ScrapeCustomSources();
                else if (option == "3") return;
                else continue;
            }
        }


        public static readonly string day = DateTime.Now.ToString("hh-mm - MMM-dd-yyyy");
        public static string fileResult = "./Proxies/" + day + "/";
        public static void Save(string text, string file)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(fileResult);
                    using (var streamWriter = new StreamWriter(fileResult + file + ".txt", true))
                    {
                        streamWriter.WriteLine(text);
                    }

                    break;
                }
                catch(Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(100);
                }
            }
        }



        public static readonly string day3 = DateTime.Now.ToString("hh-mm - MMM dd, yyyy");
        public static string fileResult3 = "./Vuln Checker/" + day3 + "/";
        public static void SaveVuln(string text, string file)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(fileResult3);
                    using (var streamWriter = new StreamWriter(fileResult3 + file + ".txt", true))
                    {
                        streamWriter.WriteLine(text);
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(30);
                }
            }
        }


        public static readonly string day2 = DateTime.Now.ToString("hh-mm - MMM dd, yyyy");
        public static string fileResult2 = "./Dehasher/" + day2 + "/";
        public static void SaveDehashed(string text, string file)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(fileResult2);
                    using (var streamWriter = new StreamWriter(fileResult2 + file + ".txt", true))
                    {
                        streamWriter.WriteLine(text);
                    }

                    break;
                }
                catch
                {
                    Thread.Sleep(30);
                }
            }
        }

        public static void menu()
        {
            for (; ; ) {
                Console.Title = "BazookaAIO V2.2 | Main Menu";
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Colorful.Console.Write("    [");
                Colorful.Console.Write("1", Check.MainColor);
                Colorful.Console.Write("] Home\n");
                Colorful.Console.Write("    [");
                Colorful.Console.Write("2", Check.MainColor);
                Colorful.Console.Write("] Account info\n");
                Colorful.Console.Write("    [");
                Colorful.Console.Write("3", Check.MainColor);
                Colorful.Console.Write("] Discord stats\n");
                Colorful.Console.Write("    [");
                Colorful.Console.Write("4", Check.MainColor);
                Colorful.Console.Write("] Settings\n\n");
                Colorful.Console.Write("    [");
                Colorful.Console.Write("5", Check.MainColor);
                Colorful.Console.Write("] Exit\n");
                string option = Console.ReadLine();
                if (option == "1")
                { 
                    Home();
                } else if(option == "2")
                {
                    Accinfo();
                } else if(option == "4")
                {
                    ChangeSettings();
                } else if(option == "5")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Colorful.Console.Write("    Bye ");
                    Colorful.Console.Write(Username, Check.MainColor);
                    Colorful.Console.WriteLine("!");
                    Thread.Sleep(4000);
                    Environment.Exit(0);
                } else if(option == "3")
                {
                    Discord_Status();
                }
                else
                {
                    Colorful.Console.WriteLine("\nPlease select a valid option!", Check.MainColor);
                    Thread.Sleep(1500);
                }
            }
        }



        public static void Discord_Status()
        {
            for (; ; )
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Option("1", "Set up");
                Option("2", "Help");
                Option("3", "Go back");
                string option = Console.ReadLine();
                if(option == "1")
                {
                    for (; ; )
                    {
                        if (verified)
                        {
                            bool t = true;
                            while (t)
                            {
                                Colorful.Console.WriteLine("You already have a Discord ID, are you sure you want to change it [y/n]:", Check.MainColor);
                                string ans = Console.ReadLine().ToUpper();
                                if(ans == "Y")
                                {
                                    Config.discord_id = "none";
                                    Config.LoadConfig();
                                    t = false;
                                }
                                else if(ans == "N")
                                {
                                    return;
                                }
                                else
                                {
                                    Colorful.Console.WriteLine("Please choose a valid option!", Check.MainColor);
                                    Thread.Sleep(1500);
                                }
                            }
                        }
                        Config.LoadConfig();
                        verified = false;
                        Console.Clear();
                        PrintLogo();
                        Console.WriteLine("");
                        Colorful.Console.WriteLine("Please enter your Discord ID...", Check.MainColor);
                        id_choosed = Console.ReadLine();
                        Console.Clear();
                        PrintLogo();
                        Console.WriteLine("");
                        Colorful.Console.WriteLine("    Please go to the BazookaAIO and send the \"?verify\", then press any key... \n    If you want to go back, press any key", Check.MainColor);
                        Console.ReadKey();
                        if (!verified)
                        {
                            id_choosed = "";
                            return;
                        }
                        else
                        {
                            data data_ = new data
                            {
                                sendDiscord = Config.SendDiscord.ToString(),
                                showFails = Check.showFails.ToString(),
                                webhook = Config.webhook,
                                discord_id = id_choosed,
                                console = Config.use_console.ToString(),
                                mainColor = Config.main_color
                            };
                            using (StreamWriter file = File.CreateText("Config.json"))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                serializer.Serialize(file, data_);
                            }
                            Config.LoadConfig();
                            id_choosed = "";
                            return;
                        }
                    }
                } else if(option == "2")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");
                    Colorful.Console.Write("With this option, you can link your discord account to the checker, and check your current\nstats like CPM, Hits, Invalids, and more doing a command in the BazookaAIO discord server.\nTo set up this option, you first need to get your discord ID, if you don't know how to get\nit just create a ticket and we will help you. Once you put your Discord ID here, you will \nhave to send the commmand \"?verify\" in discord in order to verify if the Discord ID is yours.\nThen, you can simply to \"?stats\" to check your stats.", Check.MainColor);
                    Console.ReadKey(true);

                } else if(option == "3")
                {
                    break;
                }
                else
                {
                    Colorful.Console.WriteLine("\nPlease choose a valid option!", Check.MainColor);
                    Thread.Sleep(2500);
                }
            }
        }


        public static void ChangeSettings()
        {
            for( ; ; )
            {
                Config.LoadConfig();
                Console.Clear();
                PrintLogo();
                Console.WriteLine();
                Option("1", "Current settings");
                Option("2", "Change settings");
                Option("3", "Go back");
                string opt = Console.ReadLine();
                if(opt == "1")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Colorful.Console.Write("Display invalid accounts: ", Check.MainColor);
                    Console.WriteLine(Check.showFails.ToString());
                    Colorful.Console.Write("Hits display mode: ", Check.MainColor);
                    string mode = "";
                    if (Config.use_console)
                    {
                        mode = "LOG";
                    }
                    else
                    {
                        mode = "CUI";
                    }
                    Console.WriteLine(mode);
                    Colorful.Console.Write("BazookaAIO theme color: ", Check.MainColor);
                    Console.WriteLine(Config.main_color);
                    Colorful.Console.Write("Send hits to a discord webhook: ", Check.MainColor);
                    Console.WriteLine(Config.SendDiscord.ToString());
                    if (Config.SendDiscord)
                    {
                        Colorful.Console.Write("Discord webhook: ", Check.MainColor);
                        Console.WriteLine(Config.webhook);
                    }
                    Console.WriteLine("\nPress any key to go back!");
                    Console.ReadKey(true);
                } else if(opt == "2")
                {
                    string strShowFails;
                    string strSendDiscord;
                    string strWebhook;
                    string strCui;
                    bool c = true;
                    while(c)
                    {
                        Console.Clear();
                        PrintLogo();
                        Console.WriteLine();
                        Colorful.Console.WriteLine("Do you want to display invalid accounts [y/n]: ", Check.MainColor);
                        string res1 = Console.ReadLine().ToUpper();
                        if (res1 == "Y" || res1 == "N")
                        {
                            if (res1 == "Y") { strShowFails = "True"; } else { strShowFails = "False"; }
                            Colorful.Console.WriteLine("Do you want to send results to a discord webhook [y/n]: ", Check.MainColor);
                            string res2 = Console.ReadLine().ToUpper();
                            if (res2 == "Y" || res2 == "N")
                            {
                                if(res2 == "Y")
                                {
                                    strSendDiscord = "True";
                                    Colorful.Console.WriteLine("Discord webhook (link): ", Check.MainColor);
                                    strWebhook = Console.ReadLine();
                                }
                                else
                                {
                                    strWebhook = "";
                                    strSendDiscord = "False";
                                }

                                Colorful.Console.WriteLine("How do you want to display hits: [1] LOG | [2] CUI", Check.MainColor);
                                string res3 = Console.ReadLine().ToUpper();
                                if(res3 == "1")
                                {
                                    strCui = "True";
                                }
                                else
                                {
                                    strCui = "False";
                                }

                                string color_ = Config.main_color;
                                bool x = true;
                                while (x)
                                {
                                    Colorful.Console.ReplaceAllColorsWithDefaults();
                                    Colorful.Console.WriteLine("Do you want to change the BazookaAIO theme: [y/n]", Check.MainColor);
                                    string res4 = Console.ReadLine().ToUpper();
                                    if(res4 == "Y")
                                    {


                                        Colorful.Console.WriteLine("Please select the main color:\n", Check.MainColor);

                                        Colorful.Console.Write("    [1]");
                                            Colorful.Console.WriteLine(" Violet", Color.BlueViolet);

                                        Colorful.Console.Write("    [2]");
                                            Colorful.Console.WriteLine(" Red", Color.DarkRed);

                                        Colorful.Console.Write("    [3]");
                                            Colorful.Console.WriteLine(" Orange", Color.OrangeRed);

                                        Colorful.Console.Write("    [4]");
                                            Colorful.Console.WriteLine(" Light blue", Color.DeepSkyBlue);

                                        Colorful.Console.Write("    [5]");
                                            Colorful.Console.WriteLine(" Green", Color.ForestGreen);

                                        Colorful.Console.Write("    [6]");
                                            Colorful.Console.WriteLine(" Yellow", Color.Yellow);

                                        Colorful.Console.ResetColor();
                                        Console.ResetColor();
                                            string res5 = Console.ReadLine();
                                            if(res5 == "1")
                                            {
                                                color_ = "Violet";
                                            } else if(res5 == "2")
                                            {
                                                color_ = "Red";
                                            } else if (res5 == "3")
                                            {
                                                color_ = "Orange";
                                            } else if(res5 == "4")
                                            {
                                                color_ = "Light Blue";
                                            } else if(res5 == "5")
                                            {
                                                color_ = "Green";
                                            } else if (res5 == "6")
                                            {
                                                color_ = "Yellow";
                                            }
                                            else
                                            {
                                                color_ = Config.main_color;
                                            }
                                        x = false;
                                    }
                                    else
                                    {
                                        x = false;
                                    }
                                }

                                data _data = new data
                                {
                                    showFails = strShowFails,
                                    sendDiscord = strSendDiscord,
                                    webhook = strWebhook,
                                    console = strCui,
                                    mainColor = color_
                                };

                                using (StreamWriter file = File.CreateText("Config.json"))
                                {
                                    JsonSerializer serializer = new JsonSerializer();
                                    serializer.Serialize(file, _data);
                                }
                                Config.LoadConfig();
                                c = false;
                            }
                            else
                            {
                                Colorful.Console.WriteLine("Please select a valid option!", Check.Bads_);
                            }
                        }
                        else
                        {
                            Colorful.Console.WriteLine("Please select a valid option!", Check.Bads_);
                        }
                    }
                }
                else if(opt == "3")
                {
                    break;
                }

            }
        }

        private DiscordSocketClient _client;
        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _client.MessageReceived += CommandHandler;
            //_client.Log += Log;

            var token = App.GrabVariable("rDCpv8FPxtg3jipMkhObxG0IXMyhN");

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }

        private Task CommandHandler(SocketMessage message)
        {
            //variables
            
            int lengthOfCommand = -1;

            //filtering messages begin here
            if (!message.Content.StartsWith("?")) //This is your prefix
                return Task.CompletedTask;

            if (message.Author.IsBot) //This ignores all commands from bots
                return Task.CompletedTask;

            if (message.Content.Contains(' '))
                lengthOfCommand = message.Content.IndexOf(' ');
            else
                lengthOfCommand = message.Content.Length;

            string command = message.Content.Substring(1, lengthOfCommand - 1).ToLower();

            //Commands begin here
            if (command.Equals("verify"))
            {
                if(message.Author.Id.ToString() == id_choosed)
                {
                    verified = true;
                message.Channel.SendMessageAsync($@"You have been verified, {message.Author.Mention}");
                }
            }
            else if (command.Equals("status"))
            {
                if (verified && message.Author.Id.ToString()== Config.discord_id)
                {
                    var EmbedBuilder_ = new EmbedBuilder()
                    {
                        Title = "BazookaAIO " + Username + "'s status"
                    };
                    string status;
                    if (Check.running)
                    {
                        status = "**Running**";
                    }
                    else
                    {
                        status = "**Idle**";
                    }
                    
                    EmbedBuilder_.AddField($"Status", $"{status}");
                    EmbedBuilder_.AddField($"Modules running: ", $"{Check.public_modules_list.Replace("|", "-")}");
                    EmbedBuilder_.AddField($"Hits: ", $"{Check.hits}");
                    EmbedBuilder_.AddField($"Frees: ", $"{Check.frees}");
                    EmbedBuilder_.AddField($"Invalids: ", $"{Check.bads}");
                    EmbedBuilder_.AddField($"CPM: ", $"{Check.cpm}").WithFooter(footer => footer.Text = $"Requested by {message.Author.Username}").WithColor(Discord.Color.Blue).WithCurrentTimestamp();
                    Embed embed_ = EmbedBuilder_.Build();
                    message.Channel.SendMessageAsync(embed: embed_);
                }
                }
                    
            return Task.CompletedTask;
                
        

        }



        public static void Accinfo()
        {
            Console.Clear();
            PrintLogo();
            Console.WriteLine();
            Colorful.Console.Write("Username: ", Check.MainColor);
            Console.WriteLine(Username);
            Colorful.Console.Write("Email: ", Check.MainColor);
            Console.WriteLine(Email);
            Colorful.Console.Write("HWID: ", Check.MainColor);
            Console.WriteLine(HWID);
            Colorful.Console.Write("Register Date: ", Check.MainColor);
            Console.WriteLine(Register_Date);
            Colorful.Console.WriteLine("\nPress any key to go back!", Check.MainColor);
            Console.ReadKey(true);
        }


        static void ShowModules()
        {
            Config.LoadConfig();
            //for(int i = 0; i < Modules.moduleList.Count; i++)
            //{
            //   Console.Write("[");
            //  Console.Write((i + 1));
            // Console.Write("] ");
            //Console.Write(Modules.moduleList[i] + "\n");
            //}
            Console.Clear();
            PrintLogo();
            Console.Write("\n");
            Colorful.Console.WriteLine("GAMING         STREAMING         FOOD             VPN             SHOPPING           ANTIVIRUS        MISC", Check.MainColor);
            Console.WriteLine("");
            Console.WriteLine("[1] Origin     [9] Hulu          [24] Domino's    [34] NordVPN    [/] Chegg          [56] Avira       [59] Mail Access");
            Console.WriteLine("[2] Uplay      [10] Disney+      [25] BWW         [35] HMA        [42] Forever21     [57] Bitdefender [60] Reddit");
            Console.WriteLine("[3] Minecraft  [11] Funimation   [26] DoorDash    [36] TunnelBear [43] Gucci         [58] Kaspersky   [61] Yahoo");
            Console.WriteLine("[4] Valorant   [12] PornHub      [27] KFC         [37] IPvanish   [44] GoDaddy                        [62] Acorns");
            Console.WriteLine("[/] LoL EUW    [13] Napster      [28] ShakeShack  [38] VyprVPN    [45] Walmart                        [63] Honeygain");
            Console.WriteLine("[/] LoL NA     [14] WWE          [29] Wendy       [39] TigerVPN   [46] Fuel Rewards                   [64] Fluent");
            Console.WriteLine("[/] Steam      [15] EpixNow      [30] SliceSlife  [40] SurfShark  [47] FWRD                           ");
            Console.WriteLine("[8] COD        [16] DC Universe  [31] Glovo                       [48] Goat                   ");
            Console.WriteLine("               [17] Plex         [32] Cold Stone                  [49] Patreon");
            Console.WriteLine("               [18] Pandora      [33] Grubhub                     [50] Namecheap");
            Console.WriteLine("               [19] Crunchyroll                                   [51] Venmo");
            Console.WriteLine("               [20] Spotify                                       [52] Coinbase");
            Console.WriteLine("               [21] MyCanal                                       [53] Gamefly");
            Console.WriteLine("               [22] Twitch                                        [54] Restocks.net");
            Console.WriteLine("               [23] Fubo TV                                       [55] Gfuel");
            Console.WriteLine();
            AskModules();
        }

        public static void Option(string num, string option)
        {
            Colorful.Console.Write("    [");
            Colorful.Console.Write(num, Check.MainColor);
            Colorful.Console.Write("] " + option + "\n");
        }

        static bool AskLogin()
        {
            for (; ; )
            {
                Console.Clear();
                PrintLogo();
                Option("1", "Login");
                Option("2", "Register");
                string option = Console.ReadLine();
                if (option == "2")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Console.WriteLine("Username:");
                    string username = Console.ReadLine();
                    Console.WriteLine("Password:");
                    string password = Console.ReadLine();
                    Console.WriteLine("Email:");
                    string email = Console.ReadLine();
                    Console.WriteLine("License:");
                    string license = Console.ReadLine();
                    if (API.Register(username, password, email, license))
                    {
                        System.Windows.Forms.MessageBox.Show("You have successfully registered!", OnProgramStart.Name, (MessageBoxButtons)MessageBoxButton.OK, (MessageBoxIcon)MessageBoxImage.Information);
                        Thread.Sleep(1500);
                    }
                }
                else if (option == "1")
                {
                    if (File.Exists("LoginDetails.xml"))
                    {
                        string[] first = File.ReadAllLines("LoginDetails.xml");
                        for (int i = 0; i < (int)first.Length; i++)
                        {
                            string str = first[i];
                            string[] array = str.Split(new char[]
                            {
                        ':'
                            });
                            if (API.Login(array[0], array[1]))
                            {
                                Console.Clear();
                                PrintLogo();
                                Console.WriteLine("");
                                Console.WriteLine($"Welcome back, {User.Username}...");
                                Username = User.Username;
                                Email = User.Email;
                                HWID = User.HWID;
                                Register_Date = User.RegisterDate;
                                //dWebHook webHook = new dWebHook();
                                //webHook.SendMessage($"{User.Username} just logged in to BazookaAIO V2.1");
                                Thread.Sleep(2500);
                                return true;
                            }
                            else
                            {


                            }
                        }
                    }
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine();
                    Console.WriteLine("Username:");
                    string _username = Console.ReadLine();
                    Console.WriteLine("Password:");
                    StringBuilder passwordBuilder = new StringBuilder();
                    bool continueReading = true;
                    char newLineChar = '\r';
                    while (continueReading)
                    {
                        ConsoleKeyInfo consoleKeyInfo = Console.ReadKey(true);
                        char passwordChar = consoleKeyInfo.KeyChar;

                        if (passwordChar == newLineChar)
                        {
                            continueReading = false;
                        }
                        else
                        {
                            if (passwordChar != (char)Keys.Back)
                            {
                                Console.Write("*");
                            }
                            passwordBuilder.Append(passwordChar.ToString());
                        }
                    }

                    string pass = passwordBuilder.ToString();

                    if (API.Login(_username, pass))
                    {
                        Console.Clear();
                        PrintLogo();
                        Console.WriteLine("");
                        Console.WriteLine($"Welcome back, {User.Username}...");
                        Username = User.Username;
                        Email = User.Email;
                        HWID = User.HWID;
                        Register_Date = User.RegisterDate;
                        Thread.Sleep(2500);
                        Console.Clear();
                        PrintLogo();
                        API.Log(_username, "Logged in!");
                        using (StreamWriter sw = new StreamWriter("LoginDetails.xml", true))
                        {
                            sw.WriteLine(_username + ":" + pass);
                        }
                        return true;
                    }
                    else
                    {
                        Console.WriteLine("");
                        Colorful.Console.WriteLine("Invalid email/password!", Color.Red);
                        Thread.Sleep(1000);
                    }
                }
            }
        }


        static void AskModules()
        {
            Console.Write("> Number of the modules (separated by , if you use more than one): \n> TIP: Type \"back\" to go back to the menu!\n> ");
            string choose = Console.ReadLine();
            if(choose.ToLower() == "back")
            {
                menu();
            }
            List<string> modulesNumbers = choose.Split(',').ToList();
            foreach(string moduleNumber in modulesNumbers)
            {
                int moduleInt;
                if (!int.TryParse(moduleNumber, out moduleInt) || moduleInt < 1 || moduleInt > Modules.moduleList.Count)
                {
                    Console.WriteLine("> \"" + moduleNumber + "\" is not a valid number!");
                    Thread.Sleep(5069);
                    Environment.Exit(0);
                }
                string module = Modules.moduleList[moduleInt - 1];
                if(!Check.pickedModules.Contains(module))
                    Check.pickedModules.Add(module);
            }
            if(Check.pickedModules.Count <= 0)
                Environment.Exit(0);

            Console.ResetColor();


            Utils.Init();

            Console.Clear();
            Check.Start();

            Console.WriteLine("Done.");
            Check.running = false;

            Thread.Sleep(-1);
        }


        public static void PrintHallowenLogo()
        {
            Console.WriteLine("");

            //Colorful.Console.WriteLine(@"   _______________________    _____________________ ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   ___  __/__  __ \_  ___/    ___    |___  _/_  __ \", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   __  /  __  /_/ /____ \     __  /| |__  / _  / / /    Blaschuko#6265", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   _  /   _  ____/____/ /     _  ___ |_/ /  / /_/ /     Freeload24#0001", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   /_/    /_/     /____/      /_/  |_/___/  \____/  ", Color.BlueViolet);
            centerText("   ____                       _            _    ___ ___   __     ______                    ");
            centerText(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \                   ");
            centerText(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265");
            centerText(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload#0001 ");
            centerText(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|                  ");

            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                   Bazooka                                     ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                         ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);


            Console.WriteLine("");

        }


        public static void PrintLogo()
        {
            Console.WriteLine("");

            //Colorful.Console.WriteLine(@"   _______________________    _____________________ ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   ___  __/__  __ \_  ___/    ___    |___  _/_  __ \", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   __  /  __  /_/ /____ \     __  /| |__  / _  / / /    Blaschuko#6265", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   _  /   _  ____/____/ /     _  ___ |_/ /  / /_/ /     Freeload24#0001", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"   /_/    /_/     /____/      /_/  |_/___/  \____/  ", Color.BlueViolet);

            Colorful.Console.WriteLine(@"  ____                       _            _    ___ ___   __     ______  ", Check.MainColor);
            Colorful.Console.WriteLine(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \ ", Check.MainColor);
            Colorful.Console.WriteLine(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265", Check.MainColor);
            Colorful.Console.WriteLine(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload#0001", Check.MainColor);
            Colorful.Console.WriteLine(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|", Check.MainColor);

            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                   Bazooka                                     ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                         ", Color.BlueViolet);
            //Colorful.Console.WriteLine(@"                                                                        ", Color.BlueViolet);


            Console.WriteLine("");

        }


    }
}
