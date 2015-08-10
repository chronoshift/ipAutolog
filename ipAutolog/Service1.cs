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
    public partial class Service1 : ServiceBase
    {
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            main();
 
        }

        public void main()

        {
            //Console.WriteLine("Initializing...");
            while (true)
            {
                String ip = null;
                ip = GetExternalIP();
                logIP(ip);
                //Console.WriteLine("IP Logged...");
                // Console.WriteLine("Sleeping...");
                System.Threading.Thread.Sleep(1800000);
            }
            
        }

        public void CheckFileAge()

        {
            DateTime fileCreatedDate = File.GetCreationTime(@"c:\ipExternalLog\IPlog.txt");
            DateTime today = System.DateTime.Now;
            Double totaldaysbetween = (today - fileCreatedDate).TotalDays;
            if (totaldaysbetween > 30.0)
                {
                File.Delete(@"c:\ipExternalLog\IPlog.txt");
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

        public void logIP(String ip)
        {
            string path = @"c:\ipExternalLog\IPlog.txt";
            // This text is added only once to the file. 
            if (!File.Exists(path))
            {
                // Create a file to write to. 
                CheckFileAge();
                using (StreamWriter sw = File.CreateText(path))
                {
                    sw.WriteLine(ip + " - " + DateTime.Now);
                }
            }
            using (StreamWriter sw = File.AppendText(path))
            {
                sw.WriteLine(ip + " - " + DateTime.Now);
            }
        }
        protected override void OnStop()
        {
        }
    }
}
