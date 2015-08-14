using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;


namespace ipAutolog
{
    public partial class ipAutolog : ServiceBase
    {
        public ipAutolog()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            String ip = null;
            String filepath = @"c:\ipExternalLog\IPlog.txt";
            using (StreamWriter sw = File.CreateText(filepath))
            {
                sw.WriteLine("Service Stopped");
            }
            main(ip, filepath);
        }

        public void main(String ip, String filepath)

        {
            //Console.WriteLine("Initializing...");
            while (true)
            {
                ip = GetExternalIP();
                logIP(ip,filepath);
                //Console.WriteLine("IP Logged...");
                // Console.WriteLine("Sleeping...");
                System.Threading.Thread.Sleep(1800000);
            }
            
        }

        public void CheckFileAge(String filepath)

        {
            DateTime fileCreatedDate = File.GetCreationTime(filepath);
            DateTime today = System.DateTime.Now;
            Double totaldaysbetween = (today - fileCreatedDate).TotalDays;
            if (totaldaysbetween > 30.0)
                {
                File.Delete(filepath);
                }
        }


        public string GetExternalIP()
        {
            String url = "http://bot.whatismyipaddress.com/";
            String result = null;

            try
                {
                    //Console.WriteLine("Scanning for IP...");
                    WebClient client = new WebClient();
                    result = client.DownloadString(url);
                   // Console.WriteLine(result);
                    return result;
                }
            catch (Exception ex)
                {
                    return ex.ToString();
                }

        }

        public void logIP(String ip, String filepath)
        { 
            // This text is added only once to the file. 
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                CheckFileAge(filepath);
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(ip + " - " + DateTime.Now);
                }
            }
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(ip + " - " + DateTime.Now);
            }
        }
        protected override void OnStop()
        {
            using (StreamWriter sw = File.CreateText(@"c:\ipExternalLog\IPlog.txt"))
            {
                sw.WriteLine("Service Stopped");
            }
        }
    }
}
