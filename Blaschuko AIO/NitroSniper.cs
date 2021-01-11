using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Web.ModelBinding;
using Discord;
using Discord.Gateway;
using Console = Colorful.Console;
using Colorful;
using Microsoft.SqlServer.Server;
using System.Linq;
using WebSocketSharp;
using System.Runtime.InteropServices;

namespace Blaschuko_AIO
{
    class NitroSniper
    {


        public static DiscordSocketClient mainacc = new DiscordSocketClient();

        public static void SniperMenu()
        {
            for (; ; )
            {
                Console.Title = "BazookaAIO V2.2 | Nitro Sniper";
                ResetConsole();
                Program.Option("1", "Go to Nitro Sniper");
                Program.Option("2", "How to get my token");
                Program.Option("3", "Go Back");
                string option = Console.ReadLine();

                if(option == "1")
                {
                    ResetConsole();
                    Console.Write("Please paste your Discord Token: ");
                    Sniper(Console.ReadLine());
                } else if(option == "2")
                {
                    OpenUrl("https://www.youtube.com/watch?v=YEgFvgg7ZPI");
                }
                else
                {
                    return;
                }

            }

        }



        private static void OpenUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Process.Start("xdg-open", url);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Process.Start("open", url);
                }
                else
                {
                    throw;
                }
            }
        }


        public static void Sniper(string token)
        {
            DiscordSocketClient client = new DiscordSocketClient();
            mainacc.OnMessageReceived += OnMessageReceived;

            try
            {
                mainacc.Login(token);
                ResetConsole();
                Console.WriteLine("Sniping Nitro codes...");
                for (; ; )
                {
                    Console.WriteLine("TIP: If you want to go back, send \"back\"!\n");
                    string a = Console.ReadLine().ToUpper();
                    if( a == "BACK" || a == "back")
                    {
                        mainacc.OnMessageReceived -= OnMessageReceived;
                        mainacc.Logout();
                        return;
                    }
                }
            }
            catch( Exception e)
            {
                Console.WriteLine(e, Check.MainColor);
                mainacc.OnMessageReceived -= OnMessageReceived;
                Thread.Sleep(2500);
                return;
            }
        }




        private static string redeemcode(string result, Stopwatch timer)
        {
            timer.Start();
            if (result.Contains("https://discord.gift/"))
            {
                result = result.Replace("https://discord.gift/", "");

            }
            else if (result.Contains("https://discord.com/gifts/"))
            {
                result = result.Replace("https://discord.com/gifts/", "");
            }
            string rstatus;
            try { 
                mainacc.RedeemGift(result);
                timer.Stop();

                rstatus = "REDEEMED";
            }
            catch
            {
                try
                {
                    if (mainacc.GetNitroGift(result).Consumed == true)
                    {
                        rstatus = "ALREADY REDEEMED";
                    }
                    else
                    {
                        rstatus = "ERROR REDEEMING";
                    }
                }
                catch
                {
                    rstatus = "BAD CODE";
                }
            }
            return rstatus;
        }


        private static void OnMessageReceived(DiscordSocketClient client, Discord.MessageEventArgs args)
        {
            Stopwatch timer = new Stopwatch();
            string message = args.Message.Content.ToString();
            if (message.Contains("https://discord.gift/") || message.Contains("https://discord.com/gifts/"))
            {
                string status = redeemcode(message, timer);

                string time = "0." + timer.ElapsedMilliseconds.ToString();


                Console.Write("[", Check.MainColor);
                Console.Write(time);
                Console.Write("] Code: ", Check.MainColor);
                Console.Write(message);
                Console.Write(" | Status: ", Check.MainColor);
                Console.WriteLine(status);
            }
        }



        private static void ResetConsole()
        {
            Console.Clear();
            PrintLogo();
            Console.WriteLine();
        }

        public static void PrintLogo()
        {
            Console.WriteLine("");


            Console.WriteLine(@"  ____                       _            _    ___ ___   __     ______  ", Check.MainColor);
            Console.WriteLine(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \ ", Check.MainColor);
            Console.WriteLine(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265", Check.MainColor);
            Console.WriteLine(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload#0001", Check.MainColor);
            Console.WriteLine(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|", Check.MainColor);


            Console.WriteLine("");

        }

    }
}
