using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Threading;

namespace Blaschuko_AIO
{
    class TextUtils
    {

        static List<string> Lines = new List<string>();

        public static void TextUtilsMenu()
        {
            for(; ; )
            {
                Console.Title = "BazookaAIO V2.2 | Text Utils";
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Program.Option("1", "Remove Duplicates");
                Program.Option("2", "Sort Lines");
                Program.Option("3", "Combo Combiner");
                Program.Option("4", "Combo Splitter");
                Program.Option("5", "Go Back");
                string ans = Console.ReadLine();

                if (ans == "1")
                {
                    RemoveDuplicates();
                }
                else if (ans == "2")
                {
                    SortLines();
                }
                else if (ans == "3")
                {
                    ComboCombiner();
                }
                else if (ans == "4")
                {
                    ComboSplitter();
                } else if(ans == "5")
                {
                    return;
                }
                else
                {
                    Colorful.Console.WriteLine("Please choose a valid option!", Check.MainColor);
                }
            }
        }


    private static void ComboSplitter()
        {
            string _filename;
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Console.Clear();
            PrintLogo();

            Colorful.Console.WriteLine("Please load your combo...", Check.MainColor);

            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Text Utils | Choose a text file!",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                Globals.Lines = File.ReadLines(file.FileName).ToList();
                _filename = file.FileName;
            }
            else
                return;

            Console.Clear();
            PrintLogo();
            Console.WriteLine("");

            Colorful.Console.WriteLine("Please choose the folder where you want to save the combos...", Check.MainColor);
            string folder = "";

            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                    folder = fbd.SelectedPath;
                else
                    return;
            }

            Console.Clear();
            PrintLogo();
            Console.WriteLine("");

            Colorful.Console.WriteLine("How many lines do you want in each file: ", Check.MainColor);
            string linesAmount = Console.ReadLine();

            Console.Clear();
            PrintLogo();
            Console.WriteLine("");

            Colorful.Console.WriteLine($"Splitting combos in {linesAmount} lines files...", Check.MainColor);
            int linesAmountInt;
            try
            {
                linesAmountInt = Convert.ToInt32(linesAmount);
            }
            catch { return; }
            using (var lineIterator = File.ReadLines(_filename).GetEnumerator())
            {
                bool stillGoing = true;
                for (int chunk = 0; stillGoing; chunk++)
                {
                    stillGoing = WriteChunk(lineIterator, linesAmountInt, chunk, _filename, folder);
                }
            }
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");

            Colorful.Console.WriteLine($"Done!", Check.MainColor);
            Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
            Console.ReadKey(true);
        }

        private static void RemoveDuplicates()
        {
            string _filename;
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Text Utils | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                Lines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Removing duplicates...", Check.MainColor);
            Lines = Lines.Distinct().ToList();
            SaveList(Lines, _filename, "Remove Duplicates");
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");

            Colorful.Console.WriteLine($"Done!", Check.MainColor);
            Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
            Console.ReadKey(true);
        }

        private static void SortLines()
        {
            string _filename;
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Text Utils | Choose your combo",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                Lines = File.ReadLines(file.FileName).ToList();
                _filename = file.SafeFileName;
            }
            else return;
            for (; ; )
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");
                Colorful.Console.WriteLine("Please choose a sorting option:", Check.MainColor);
                Program.Option("1", "Sort Alphabetically");
                Program.Option("2", "Sort By Length");
                Program.Option("3", "Reverse lines");
                Program.Option("4", "Randomize");
                Program.Option("5", "Go back");
                string choice = Console.ReadLine();
                if(choice == "1")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");
                    Colorful.Console.WriteLine("Sorting lines alphabetically...", Check.MainColor);
                    Lines.Sort();
                    SaveList(Lines, _filename, "Alphabetically Sorter");
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");

                    Colorful.Console.WriteLine($"Done!", Check.MainColor);
                    Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
                    Console.ReadKey(true);
                } else if(choice == "2")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");
                    Colorful.Console.WriteLine("Sorting lines by length...", Check.MainColor);
                    Lines.Sort((a, b) => a.Length.CompareTo(b.Length));
                    SaveList(Lines, _filename, "Length Sorter");
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");

                    Colorful.Console.WriteLine($"Done!", Check.MainColor);
                    Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
                    Console.ReadKey(true);
                } else if(choice == "3")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");
                    Colorful.Console.WriteLine("Reversing lines...", Check.MainColor);
                    Lines.Reverse();
                    SaveList(Lines, _filename, "Reverse Sorter");
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");

                    Colorful.Console.WriteLine($"Done!", Check.MainColor);
                    Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
                    Console.ReadKey(true);
                } else if(choice == "4")
                {
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");
                    Colorful.Console.WriteLine("Randomizing lines...", Check.MainColor);
                    Lines = Lines.OrderBy(a => Guid.NewGuid()).ToList();
                    SaveList(Lines, _filename, "Randomized");
                    Console.Clear();
                    PrintLogo();
                    Console.WriteLine("");

                    Colorful.Console.WriteLine($"Done!", Check.MainColor);
                    Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
                    Console.ReadKey(true);
                } else if(choice == "5")
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

        private static void ComboCombiner()
        {
            Console.Clear();
            PrintLogo();
            Console.WriteLine("");
            Colorful.Console.WriteLine("Press any key to load your combo...", Check.MainColor);
            Console.ReadKey(true);
            OpenFileDialog file = new OpenFileDialog
            {
                Title = "BazookaAIO V2.2 | Text Utils | Choose a text file!",
                Filter = "Text Files | *.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                Multiselect = true
            };

            if (file.ShowDialog() == DialogResult.OK)
            {
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");

                Colorful.Console.WriteLine("Combining combos...", Check.MainColor);

                foreach (string filename in file.FileNames)
                { 
                    Lines = File.ReadLines(filename).ToList();
                }

                SaveList(Lines, "Combined Combos", "Combo Combiner");
                Console.Clear();
                PrintLogo();
                Console.WriteLine("");

                Colorful.Console.WriteLine($"Done!", Check.MainColor);
                Colorful.Console.WriteLine($"Press any key to continue...", Check.MainColor);
                Console.ReadKey(true);
            }
            else
                return;
        }

        public static readonly string day = DateTime.Now.ToString("hh-mm - MMM dd, yyyy");
        public static string fileResult = "./Text Utils/" + day + "/";
        public static void Save(string text, string file, string module = "")
        {
            while (true)
            {
                try
                {
                    if (module != "")
                    {
                        Directory.CreateDirectory(fileResult + module + "/");
                        using (var streamWriter = new StreamWriter(fileResult + module + "/" + file + ".txt", true))
                        {
                            streamWriter.WriteLine(text);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(fileResult);
                        using (var streamWriter = new StreamWriter(fileResult + file + ".txt", true))
                        {
                            streamWriter.WriteLine(text);
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



        public static readonly string day2 = DateTime.Now.ToString("hh-mm - MMM dd, yyyy");
        public static string fileResult2 = "./Text Utils/" + day + "/";
        public static void SaveList(List<string> listToSave, string file, string module = "")
        {
            while (true)
            {
                try
                {
                    if (module != "")
                    {
                        Directory.CreateDirectory(fileResult2 + module + "/");
                        using (var streamWriter = new StreamWriter(fileResult2 + module + "/" + file + ".txt", true))
                        {
                            foreach (string line in listToSave)
                            {
                                streamWriter.WriteLine(line);
                            }
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(fileResult2);
                        using (var streamWriter = new StreamWriter(fileResult2 + file + ".txt", true))
                        {
                            foreach (string line in listToSave)
                            {
                                streamWriter.WriteLine(line);
                            }
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


        private static bool WriteChunk(IEnumerator<string> lineIterator, int splitSize, int chunk, string filenameparam, string folder)
        {
            string filename = Path.GetFileName(filenameparam);
            using (var writer = File.CreateText($"{folder}\\{filename}_{chunk}.txt"))
            {
                for (int i = 0; i < splitSize; i++)
                {
                    if (!lineIterator.MoveNext())
                    {
                        return false;
                    }
                    writer.WriteLine(lineIterator.Current);
                }
            }
            return true;
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
