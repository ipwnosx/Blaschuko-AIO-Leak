
using System.Drawing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Web;
using System.Windows.Forms;

namespace Blaschuko_AIO
{
    class Utils
    {
        public static string date = DateTime.Now.ToLongDateString() + " - " + DateTime.Now.ToLongTimeString().Replace(":", ".");

        public static void WriteResult(string textToWrite, string file, bool append = true)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory("results/" + date);
                    using (StreamWriter streamWriter = new StreamWriter("results/" + date + "/" + file + ".txt", append))
                    {
                        streamWriter.WriteLine(textToWrite);
                    }
                    return;
                }
                catch
                {
                    Thread.Sleep(50);
                }
            }
        }


        private static Random _random = new Random();

        public static string GenerateRandomNo()
        {
            return _random.Next(0, 10).ToString("D1");
        }

        public static IEnumerable<string> GetCombo()
        {
            IEnumerable<string> accounts;
            while (true)
            {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Load your combo",
                        Filter = "Text File|*.txt"
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        /*comboName = openFileDialog.FileName;
                        if (comboName.Contains("\\"))
                            comboName = comboName.Split('\\')[comboName.Split('\\').Length - 1].Split('.')[0];*/

                        accounts = File.ReadLines(openFileDialog.FileName);

                        if (accounts.Count<string>() > 0)
                        {
                            return accounts;
                        }
                    }

                }
                catch { }
            }
        }
        public static IEnumerable<string> GetProxies()
        {
            IEnumerable<string> proxies;
            while (true)
            {
                try
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog
                    {
                        Title = "Load your proxies",
                        Filter = "Text File|*.txt"
                    };
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        proxies = File.ReadLines(openFileDialog.FileName);

                        if (proxies.Count() > 0)
                        {
                            return proxies;
                        }
                    }
                }
                catch { }
            }
        }
        static Random random = new Random();
        public static string GetRandomHexNumber(int digits)
        {
            byte[] buffer = new byte[digits / 2];
            random.NextBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + random.Next(16).ToString("X");
        }

        public static string RandomCapitalsAndDigits(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string RandomUpper()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, 1)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }







        public static void Init()
        {
            Console.Clear();
            Print_Logo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            Check.comboList = GetCombo();
            Check.remainingLines = Check.comboList.ToList();
            Check.comboListLength = Check.comboList.Count();
            Console.Clear();
            Print_Logo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your proxies...", Check.MainColor);
            Console.ReadKey(true);
            Check.proxies = GetProxies();
            Check.proxyListLength = Check.proxies.Count();


            if (Config.useProxies)
            {

                bool ok = false;
                for (; ; )
                {
                    Console.Clear();
                    Print_Logo();
                    Console.WriteLine("");
                    Colorful.Console.Write("\nPlease select your proxy type [HTTP / SOCKS4 / SOCKS5]: ", Check.MainColor);
                    string type = Console.ReadLine().ToLower();
                    switch (type)
                    {
                        case "http":
                            Config.proxyType = "HTTP";
                            ok = true;
                            break;

                        case "socks4":
                            Config.proxyType = "SOCKS4";
                            ok = true;
                            break;

                        case "socks5":
                            Config.proxyType = "SOCKS5";
                            ok = true;
                            break;

                        case null:
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nPlease select a valid proxy type!", Check.MainColor);
                            Thread.Sleep(1500);
                            ok = false;
                            break;
                    }
                    if (ok)
                    {
                        break;
                    }
                }
                for (; ; )
                {
                    Console.Clear();
                    Print_Logo();
                    Console.WriteLine("");
                    Colorful.Console.Write("Please choose the amount of threads you want to use: ", Check.MainColor);
                    try
                    {
                        Config.threads = int.Parse(Console.ReadLine());
                        break;
                    }
                    catch
                    {
                        continue;
                    }
                }


            }
        }

        public static byte[] HexStringToHex(string inputHex)
        {
            var resultantArray = new byte[inputHex.Length / 2];
            for (var i = 0; i < resultantArray.Length; i++)
            {
                resultantArray[i] = System.Convert.ToByte(inputHex.Substring(i * 2, 2), 16);
            }
            return resultantArray;
        }

        public static string HexString2B64String(string input)
        {
            return System.Convert.ToBase64String(HexStringToHex(input));
        }

        public static string RSAEncryption(string strText, string mod, string exp)
        {
            var publicKey = $"<RSAKeyValue><Modulus>{HexString2B64String(mod)}</Modulus><Exponent>{HexString2B64String(exp)}</Exponent></RSAKeyValue>";

            var data = Encoding.UTF8.GetBytes(strText);

            using (var rsa = new RSACryptoServiceProvider(1024))
            {
                try
                {
                    // client encrypting data with public key issued by server                    
                    /*rsa.FromXmlString(publicKey.ToString());*/


                    RSAParameters p = new RSAParameters();
                    p.Modulus = HexStringToHex(mod);
                    p.Exponent = HexStringToHex(exp);

                    rsa.ImportParameters(p);

                    var encryptedData = rsa.Encrypt(data, false);

                    var base64Encrypted = Convert.ToBase64String(encryptedData);

                    return base64Encrypted;
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }




        public static void Print_Logo()
        {
            Console.WriteLine("");
            
            Colorful.Console.WriteLine(@"  ____                       _            _    ___ ___   __     ______  ", Check.MainColor);
            Colorful.Console.WriteLine(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \ ", Check.MainColor);
            Colorful.Console.WriteLine(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265", Check.MainColor);
            Colorful.Console.WriteLine(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload24#0001", Check.MainColor);
            Colorful.Console.WriteLine(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|", Check.MainColor);

            Console.WriteLine("");

        }

        public static string Base64Encode(string plainText)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
        }
        public static string Base64Decode(string plainText)
        {
            return System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(plainText));
        }


        public static readonly string day = DateTime.Now.ToString("hh - MMM dd, yyyy");
        public static string fileResult = "./Results/" + day + "/";
        public void Results(string text, string file, string module)
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

    }
}


