using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using LeagueSharp;

namespace Jhin___The_Virtuoso.Plugins
{
    class VersionCheck
    {
        public static void UpdateCheck()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    using (var c = new WebClient())
                    {
                        var rawVersion = c.DownloadString("https://raw.githubusercontent.com/HikigayaAss/LeagueSharp/master/Jhin%20-%20The%20Virtuoso/Properties/AssemblyInfo.cs");
                        var match = new Regex(@"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]").Match(rawVersion);

                        if (match.Success)
                        {
                            var gitVersion = new Version(string.Format("{0}.{1}.{2}.{3}", match.Groups[1], match.Groups[2], match.Groups[3], match.Groups[4]));

                            if (gitVersion != typeof(Program).Assembly.GetName().Version)
                            {
                                Game.PrintChat("<font color='#15C3AC'>Jhin - The Virtuoso:</font> <font color='#FF0000'>" + "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                                Game.PrintChat("<font color='#15C3AC'>Jhin - The Virtuoso:</font> <font color='#FF0000'>" + "OUTDATED - Please Update to Version: " + gitVersion + "</font>");
                            }
                            else
                            {
                                Game.PrintChat("<font color='#15C3AC'>Jhin - The Virtuoso:</font> <font color='#40FF00'>" + "UPDATED - Version: " + gitVersion + "</font>");
                            }
                        }
                        else
                        {
                            Game.PrintChat("wrong github link :roto2:");
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            });
        }
    }
}