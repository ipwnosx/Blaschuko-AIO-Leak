using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using Colorful;
using Console = Colorful.Console;
using System.Windows.Forms;
using System.Windows.Shapes;
using System.Text.RegularExpressions;

namespace Blaschuko_AIO
{
    class ComboEditor
    {

        public static List<string> RandomizedLines = new List<string>();
        public static List<string> OldLines = new List<string>();
        public static List<string> NewLines = new List<string>();
        public static List<string> AllLines = new List<string>();


        public static void ResetLines()
        {
            RandomizedLines.Clear();
            OldLines.Clear();
            NewLines.Clear();
            AllLines.Clear();
        }

        public static void Menu()
        {
            for (; ; )
            {
                ResetLines();
                ResetConsole();
                Program.Option("1", "General Edits");
                Program.Option("2", "Gaming Edits");
                Program.Option("3", "Steam Edits");
                Program.Option("4", "Ultra Edits");
                Program.Option("5", "Go Back");

                string option = Console.ReadLine();
                if (option == "1")
                {
                    GeneralEdits();
                }
                else if (option == "2")
                {
                    GamingEdits();
                }
                else if (option == "3")
                {
                    SteamEdits();
                }
                else if (option == "4")
                {
                    UltraEdits();
                }
                else if (option == "5") return;
                else
                {
                    Console.WriteLine("\nPlease choose a valid option!", Check.MainColor);
                    Thread.Sleep(1500);
                }
            }

        }



        public static string method_1(string pass)
        {
            string result;
            try
            {
                result = pass.First<char>().ToString().ToUpper() + pass.Substring(1);
            }
            catch
            {
                result = pass;
            }
            return result;
        }


        public static void ResetConsole()
        {
            Console.Clear();
            PrintLogo();
            Console.WriteLine();
        }

        public static string _filename;

        public static void GeneralEdits()
        {
            ResetConsole();

            Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Combo Editor | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                OldLines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;

            ResetConsole();
            Console.Write("Successfully loaded ", Check.MainColor);
            Console.Write(OldLines.Count);
            Console.WriteLine(" lines!\nPress any key to continue...", Check.MainColor);
            Console.ReadKey(true);

            ResetConsole();

            Console.WriteLine("Editing combo...", Check.MainColor);

            foreach (string line in OldLines)
            {
                if (line.Contains(":"))
                {
                    string user = line.Split(new string[]
                    {
                        ":"
                    }, StringSplitOptions.None)[0];

                    string password = line.Split(new string[] {
                        ":"
                    }, StringSplitOptions.None)[1];

                    int length = password.Length;
                    if (length > 0)
                    {
                        char c = password[length - 1];
                        Match match = Regex.Match(password, "(.{1})\\s*$");
                        if (c.ToString().All(new Func<char, bool>(char.IsLetter)))
                        {
                            if (!password.EndsWith("!"))
                            {
                                NewLines.Add(user + ":" + method_1(password) + "!");
                            }
                            NewLines.Add(user + ":" + method_1(password) + "1");
                            NewLines.Add(user + ":" + method_1(password) + "123");
                            NewLines.Add(user + ":" + method_1(password) + "$");
                        }
                        else if (c.ToString().Any(new Func<char, bool>(char.IsNumber)))
                        {
                            NewLines.Add(user + ":" + method_1(password) + "!");
                        }
                    }
                }
            }

            ResetConsole();


            Console.WriteLine("Do you want to save only the edited lines: [y / n]", Check.MainColor);
            string answer = Console.ReadLine().ToUpper();

            if (answer == "Y")
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                RandomizedLines = NewLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            else
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                AllLines = OldLines.Concat(NewLines).ToList();
                RandomizedLines = AllLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            Done();

        }

        public static void SteamEdits()
        {
            ResetConsole();

            Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Combo Editor | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                OldLines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;

            ResetConsole();
            Console.Write("Successfully loaded ", Check.MainColor);
            Console.Write(OldLines.Count);
            Console.WriteLine(" lines!\nPress any key to continue...", Check.MainColor);
            Console.ReadKey(true);

            ResetConsole();

            Console.WriteLine("Editing combo...", Check.MainColor);

            foreach (string line in OldLines)
            {
                if (line.Contains(":"))
                {
                    string user = line.Split(new string[]
                    {
                        ":"
                    }, StringSplitOptions.None)[0];

                    string password = line.Split(new string[] {
                        ":"
                    }, StringSplitOptions.None)[1];

                    int length = password.Length;
                    if (length > 0)
                    {
                        char c = password[length - 1];
                        Match match = Regex.Match(password, "(.{1})\\s*$");
                        if (c.ToString().All(new Func<char, bool>(char.IsLetter)))
                        {
                            NewLines.Add(user + ":" + method_1(password) + "1");
                            NewLines.Add(user + ":" + method_1(password) + "123");
                            NewLines.Add(user + ":" + method_1(password) + "69");
                            if (!password.EndsWith("!"))
                            {
                                NewLines.Add(user + ":" + method_1(password) + "!");
                            }
                            NewLines.Add(user + ":" + method_1(password) + "!!");

                        }
                        else if (c.ToString().All(new Func<char, bool>(char.IsNumber)))
                        {
                            int num;
                            try
                            {
                                num = Convert.ToInt32(match.ToString()) + 1;
                            }
                            catch
                            {
                                num = 0;
                            }
                            NewLines.Add(user + ":" + method_1(password) + num.ToString());
                            NewLines.Add(user + ":" + method_1(password) + "lol");
                            NewLines.Add(user + ":" + method_1(password) + "!");
                        }
                        else
                        {
                            if (!password.EndsWith("!"))
                            {
                                NewLines.Add(user + ":" + method_1(password) + "!");
                            }
                        }
                    }
                }
            }

            ResetConsole();


            Console.WriteLine("Do you want to save only the edited lines: [y / n]", Check.MainColor);
            string answer = Console.ReadLine().ToUpper();

            if (answer == "Y")
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                RandomizedLines = NewLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            else
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                AllLines = OldLines.Concat(NewLines).ToList();
                RandomizedLines = AllLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            Done();

        }

        public static void UltraEdits()
        {
            ResetConsole();

            Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Combo Editor | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                OldLines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;

            ResetConsole();
            Console.Write("Successfully loaded ", Check.MainColor);
            Console.Write(OldLines.Count);
            Console.WriteLine(" lines!\nPress any key to continue...", Check.MainColor);
            Console.ReadKey(true);

            ResetConsole();

            Console.WriteLine("Editing combo...", Check.MainColor);

            foreach (string line in OldLines)
            {
                if (line.Contains(":"))
                {
                    string user = line.Split(new string[]
                    {
                        ":"
                    }, StringSplitOptions.None)[0];

                    string password = line.Split(new string[] {
                        ":"
                    }, StringSplitOptions.None)[1];

                    int length = password.Length;
                    if (length > 0)
                    {
                        if (!password.EndsWith("!"))
                        {
                            int a = RandomNumber(1, 2);
                            if(a == 1) NewLines.Add(user + ":" + password + "!");
                            else NewLines.Add(user + ":" + method_1(password) + "!");
                        }
                        int b = RandomNumber(1, 2);
                        if(b == 1) NewLines.Add(user + ":" + method_1(password) + "123");
                        else NewLines.Add(user + ":" + password + "123");

                        int c = RandomNumber(1, 2);
                        if(c == 1) NewLines.Add(user + ":" + method_1(password) + "?");
                        else NewLines.Add(user + ":" + password + "?");

                        int d = RandomNumber(1, 2); 
                        if(d == 1) NewLines.Add(user + ":" + method_1(password) + "$");
                        else NewLines.Add(user + ":" + password + "$");

                        int e = RandomNumber(1, 2);
                        if( e == 1) NewLines.Add(user + ":" + method_1(password) + "69");
                        else NewLines.Add(user + ":" + password + "69");

                        int f = RandomNumber(1, 2);
                        if(f == 1) NewLines.Add(user + ":" + method_1(password) + "12345");
                        else NewLines.Add(user + ":" + password + "12345");

                    }
                }
            }


            ResetConsole();


            Console.WriteLine("Do you want to save only the edited lines: [y / n]", Check.MainColor);
            string answer = Console.ReadLine().ToUpper();

            if (answer == "Y")
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                RandomizedLines = NewLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            else
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                AllLines = OldLines.Concat(NewLines).ToList();
                RandomizedLines = AllLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "General Edits");
            }
            Done();

        }



        public static void GamingEdits()
        {
            ResetConsole();

            Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Combo Editor | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                OldLines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;

            ResetConsole();
            Console.Write("Successfully loaded ", Check.MainColor);
            Console.Write(OldLines.Count);
            Console.WriteLine(" lines!\nPress any key to continue...", Check.MainColor);
            Console.ReadKey(true);

            ResetConsole();

            Console.WriteLine("Editing combo...", Check.MainColor);

            foreach (string line in OldLines)
            {
                if (line.Contains(":"))
                {
                    string user = line.Split(new string[]
                    {
                        ":"
                    }, StringSplitOptions.None)[0];

                    string password = line.Split(new string[] {
                        ":"
                    }, StringSplitOptions.None)[1];

                    int length = password.Length;
                    if (length > 0)
                    {
                        char c = password[length - 1];
                        Match match = Regex.Match(password, "(.{1})\\s*$");

                        if (c.ToString().All(new Func<char, bool>(char.IsNumber)))
                        {
                            int num;
                            try
                            {
                                num = Convert.ToInt32(match.ToString()) + 1;
                            }
                            catch (Exception)
                            {
                                num = 1;
                            }
                            NewLines.Add(user + ":" + method_1(password) + num.ToString());
                            NewLines.Add(user + ":" + method_1(password) + "!");
                            NewLines.Add(user + ":" + method_1(password) + "$");
                        }

                        else
                        {
                            NewLines.Add(user + ":" + method_1(password) + "1");
                            if (!password.EndsWith("!")) NewLines.Add(user + ":" + method_1(password) + "!");
                            NewLines.Add(user + ":" + method_1(password) + "123");
                            NewLines.Add(user + ":" + method_1(password) + "?");
                        }
                    }
                }
            }

            ResetConsole();


            Console.WriteLine("Do you want to save only the edited lines: [y / n]", Check.MainColor);
            string answer = Console.ReadLine().ToUpper();

            if (answer == "Y")
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                RandomizedLines = NewLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "Gaming Edits");
            }
            else
            {
                ResetConsole();
                Console.WriteLine("Randomizing lines...", Check.MainColor);
                AllLines = OldLines.Concat(NewLines).ToList();
                RandomizedLines = AllLines.OrderBy(a => Guid.NewGuid()).ToList();
                SaveList(RandomizedLines, _filename + " - Edited", "Gaming Edits");
            }
            Done();

        }



        private static readonly Random _random = new Random();

        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public static void Done()
        {
            ResetConsole();
            Console.WriteLine("Done!\nPress any key to go back to the menu...");
            Console.ReadKey(true);
        }



        public static readonly string day = DateTime.Now.ToString("hh:mm - MMM/dd/yyyy");
        public static string fileResult = "./Combo Editor/";
        public static void Save(string text, string file, string module)
        {
            while (true)
            {
                try
                {
                    Directory.CreateDirectory(fileResult + module + "/" + day + "/");
                    using (var streamWriter = new StreamWriter(fileResult + module + "/" + day + "/" + file + ".txt", true))
                    {
                        streamWriter.WriteLine(text);
                    }
                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(100);
                }
            }
        }



        public static readonly string day2 = DateTime.Now.ToString("hh-mm - MMM-dd-yyyy");
        public static string fileResult2 = "./Combo Editor/";
        public static void SaveList(List<string> listToSave, string file, string module)
        {
            while (true)
            {
                try
                {

                    Directory.CreateDirectory(fileResult2 + module + "/" + day2 + "/");
                    using (var streamWriter = new StreamWriter(fileResult2 + module + "/" + day2 + "/" + file + ".txt", true))
                    {
                        foreach (string line in listToSave)
                        {
                            streamWriter.WriteLine(line);
                        }
                    }


                    break;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Thread.Sleep(100);
                }
            }
        }





        public static void PrintLogo()
        {
            Console.WriteLine("");


            Colorful.Console.WriteLine(@"  ____                       _            _    ___ ___   __     ______  ", Check.MainColor);
            Colorful.Console.WriteLine(@" | __ )  __ _ _______   ___ | | ____ _   / \  |_ _/ _ \  \ \   / /___ \ ", Check.MainColor);
            Colorful.Console.WriteLine(@" |  _ \ / _` |_  / _ \ / _ \| |/ / _` | / _ \  | | | | |  \ \ / /  __) |    Blaschuko#6265", Check.MainColor);
            Colorful.Console.WriteLine(@" | |_) | (_| |/ / (_) | (_) |   < (_| |/ ___ \ | | |_| |   \ V /  / __/     Freeload#0001", Check.MainColor);
            Colorful.Console.WriteLine(@" |____/ \__,_/___\___/ \___/|_|\_\__,_/_/   \_\___\___/     \_/  |_____|", Check.MainColor);


            Console.WriteLine("");

        }

    }
}
