using System;
using System.IO;
using System.Diagnostics;
using Newtonsoft.Json;

namespace SP_Wrapper
{
    class Config
    {
        public string viewer, switchpresence, ip, clientid;
        public bool showConsole;

        public Config()
        {
            viewer = "C:/Path/To/Viewer.exe";
            switchpresence = "C:/Path/To/switchpresence.exe";
            ip = "127.0.0.1";
            clientid = "0000000000000000";
            showConsole = true;
        }
    }
    
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting...");

            // load config
            Config cfg = new Config();
            if (File.Exists("config.json"))
            {
                cfg = JsonConvert.DeserializeObject<Config>(File.ReadAllText("config.json"));
            } 
            else
            {
                File.WriteAllText("config.json", JsonConvert.SerializeObject(new Config()));
                Console.WriteLine("Config missing! Creating...\nPlease populate config.json and launch again.\n\nPress any key to exit...");
                Console.ReadKey();
                return;
            }

            // prepare switchpresence
            ProcessStartInfo sp_info = new ProcessStartInfo();
            sp_info.Arguments = cfg.ip + " " + cfg.clientid;
            sp_info.FileName = cfg.switchpresence;
            if (cfg.showConsole)
                sp_info.WindowStyle = ProcessWindowStyle.Hidden;

            // prepare viewer app
            ProcessStartInfo view_info = new ProcessStartInfo();
            view_info.FileName = cfg.viewer;

            // run it
            using (Process view_proc = Process.Start(view_info))
            {
                using (Process sp_proc = Process.Start(sp_info))
                {
                    // wait for view to exit
                    view_proc.WaitForExit();

                    // kill switch presence
                    sp_proc.Kill();
                }
            }

            return;
        }
    }
}
