using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Drawing;

namespace Blaschuko_AIO
{
    class Config
    {

        public static int threads;
        public static bool useProxies = true;
        public static string proxyType;
        public static bool SendDiscord = false;
        public static string webhook;
        public static int retries = 0;
        public static string discord_id;
        public static bool use_console;
        public static string main_color = "Violet";

        
        public static void LoadConfig()
        {
            if (File.Exists("Config.json"))
            {
                using (StreamReader r = new StreamReader("Config.json"))
                {
                    string json = r.ReadToEnd();
                    dynamic cfg = JsonConvert.DeserializeObject(json);
                    Check.showFails = Convert.ToBoolean(cfg.showFails);
                    discord_id = cfg.discord_id;
                    SendDiscord = Convert.ToBoolean(cfg.sendDiscord);
                    webhook = cfg.webhook;
                    use_console = Convert.ToBoolean(cfg.console);
                    string color = cfg.mainColor;
                    main_color = color;
                    if(color == "Violet")
                    {
                        Check.MainColor = Color.BlueViolet;
                    } else if (color == "Red")
                    {
                        Check.MainColor = Color.DarkRed;
                    } else if(color == "Orange")
                    {
                        Check.MainColor = Color.OrangeRed;
                    } else if(color == "Light Blue")
                    {
                        Check.MainColor = Color.DeepSkyBlue; ;
                    } else if(color == "Green")
                    {
                        Check.MainColor = Color.ForestGreen;
                    } else if(color == "Yellow")
                    {
                        Check.MainColor = Color.Yellow;
                    }
                    else
                    {
                        Check.MainColor = Color.BlueViolet;
                        main_color = "Violet";
                    }
                }

            }
            else
            {
                data _data = new data
                {
                    showFails = "True",
                    sendDiscord = "False",
                    webhook = "",
                    discord_id = "none",
                    console = "True",
                    mainColor = "Violet"
                };

                using (StreamWriter file = File.CreateText("Config.json"))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Serialize(file, _data);
                }
                LoadConfig();

            }

        }

    }
}
