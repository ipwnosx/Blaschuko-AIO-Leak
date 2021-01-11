using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;



namespace Blaschuko_AIO
{
    public class dWebHook : IDisposable
    {
        private readonly WebClient dWebClient;
        private static NameValueCollection discordValues = new NameValueCollection();
        public string WebHook { get; set; }
        public string UserName { get; set; }
        public string ProfilePicture { get; set; }

        public dWebHook()
        {
            dWebClient = new WebClient();
        }


        public void SendMessage(string msgSend) 
        {
            discordValues.Clear();
            discordValues.Add("username", "BazookaAIO V2.2");
            discordValues.Add("avatar_url", "https://cdn.discordapp.com/icons/713435121335795762/fe400a8b90645fa1df599ff5793d1eea.png");
            discordValues.Add("content", msgSend);
            try
            {
                dWebClient.UploadValues(Config.webhook, discordValues);

            }
            catch (Exception)
            {
                Config.SendDiscord = false;
                Config.webhook = "";
                Colorful.Console.WriteLine("Bad Discord webhook!", Color.Red);
            }
        }

        public void Dispose()
        {
            dWebClient.Dispose();
        }
    }
}