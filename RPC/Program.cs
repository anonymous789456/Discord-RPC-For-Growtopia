using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using DiscordRPC;
using Microsoft.Win32;

namespace RPC
{
    internal class Program
    {
        #region Initialize/Dispose
        public static DiscordRpcClient client;
        
        public static void Start()
        {
            client = new DiscordRpcClient("901417794661400606");
            client.Initialize();
        }

        public static void Stop()
        {
            client.Dispose();
        }
        #endregion

        #region Display and update
        public static FileSystemWatcher fileSystemWatcher = new FileSystemWatcher();

        public static void SCheck()
        {
            try
            {
                fileSystemWatcher.Path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\Growtopia";
                fileSystemWatcher.NotifyFilter = NotifyFilters.LastWrite;
                fileSystemWatcher.Filter = "*.dat";
                fileSystemWatcher.Changed += Changed;
                fileSystemWatcher.EnableRaisingEvents = true;
                while (true)
                {
                    Thread.Sleep(1000);
                }
            }
            catch
            { }
        }

        private static void Changed(object source, FileSystemEventArgs e)
        {
            try
            {
                client.SetPresence(new RichPresence()
                {
                    Timestamps = new Timestamps(DateTime.UtcNow),
                    Details = "GrowID: " + GrowID(),
                    State = "Last World: " + World(),
                    Assets = new Assets()
                    {
                        LargeImageKey = "growtopia",
                        LargeImageText = "Growtopia",
                    },
                    Buttons = new Button[]
                    {
                        new Button() { Label = "GitHub", Url = "https://github.com/extatent/" }
                    }
                });
            }
            catch
            { }

            Thread.Sleep(2000);
            Process[] running = Process.GetProcessesByName("Growtopia");
            if (running.Length == 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Growtopia is not running");
                Stop();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("GrowID and last world was updated");
            }
        }
        #endregion

        #region GrowID
        public static string GrowID()
        {
            try
            {
                return Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).Substring(Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).IndexOf("tankid_name") + 15, Convert.ToInt32(File.ReadAllBytes(SPath())[Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).IndexOf("tankid_name") + 11]));
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region World
        public static string World()
        {
            try
            {
                return Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).Substring(Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).IndexOf("lastworld") + 13, Convert.ToInt32(File.ReadAllBytes(SPath())[Encoding.ASCII.GetString(File.ReadAllBytes(SPath())).IndexOf("lastworld") + 9]));
            }
            catch
            {
                return "";
            }
        }
        #endregion

        #region Save.dat Path
        public static string SPath()
        {
            try
            {
                RegistryKey reg;
                if (Environment.Is64BitOperatingSystem)
                {
                    reg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
                }
                else
                {
                    reg = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
                }
                reg = reg.OpenSubKey("Software\\Growtopia", true);
                string path = (string)reg.GetValue("path");
                if (Directory.Exists(path))
                {
                    string savedat = File.ReadAllText(path + "\\save.dat");
                    if (savedat.Contains("tankid_password") && savedat.Contains("tankid_name"))
                    {
                        return path + "\\save.dat";
                    }
                    else
                    {
                        return Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Growtopia\\save.dat";
                    }
                }
                else
                {
                    return Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Growtopia\\save.dat";
                }
            }
            catch
            {
                return Environment.ExpandEnvironmentVariables("%LOCALAPPDATA%") + "\\Growtopia\\save.dat";
            }
        }
        #endregion

        public static void Main()
        {
            Console.Title = "Discord RPC For Growtopia";
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("An Unofficial Discord RPC For Growtopia Written in C#");
            Console.WriteLine("GitHub: https://github.com/extatent");

            Start();
            SCheck();
        }
    }
}
