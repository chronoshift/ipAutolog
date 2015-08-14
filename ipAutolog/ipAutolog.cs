using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Net;


namespace ipAutolog
{
    public partial class ipAutolog : ServiceBase
    {
        private ManualResetEvent _shutdownEvent = new ManualResetEvent(false);
        private Thread _thread;
        public ipAutolog()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            ThreadStart threadstart = this.main;
            _thread = new Thread(threadstart);
            _thread.Start();
        }

        public void main()

        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
                  {
                        new ipAutolog()
                  };

            ServiceBase.Run(ServicesToRun);

            String ip = null;
            String filepath = @"c:\ipExternalLog\IPlog.txt";
            String message = "Service Started";
            CreateOrAppendFile(filepath, message);
            ip = GetExternalIP();
            message = ip + " - " + DateTime.Now;
            CreateOrAppendFile(filepath, message);

            System.Threading.Thread.Sleep(initialTimeSleepsync(filepath,message));

            while (true)
            {
                ip = GetExternalIP();
                message = ip + " - " + DateTime.Now;
                CreateOrAppendFile(filepath,message);
                System.Threading.Thread.Sleep(1800000);
            }
            
        }
//--------------------------------------------------------------------------------------------------------------------------------------------

        public Int32 initialTimeSleepsync(String filepath,String message)
        {
            
            var datetime = DateTime.Parse(DateTime.Now.ToString());
            var minutesPastHalfHour = datetime.Minute % 30;
            var minutesBeforeHalfHour = 30 - minutesPastHalfHour;
            var sleeptime = minutesBeforeHalfHour * 60000;
            message = "sleeping " + minutesBeforeHalfHour.ToString() + " minutes to sync to clock";
            CreateOrAppendFile(filepath, message);

            return sleeptime;
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
                    WebClient client = new WebClient();
                    result = client.DownloadString(url);
                    return result;
                }
            catch (Exception ex)
                {
                    return ex.ToString();
                }

        }


        public void CreateOrAppendFile(String filepath, String message)
            {
            if (!File.Exists(filepath))
            {
                // Create a file to write to. 
                CheckFileAge(filepath);
                using (StreamWriter sw = File.CreateText(filepath))
                {
                    sw.WriteLine(message);
                }
            }
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(message);
            }
        }
        protected override void OnStop()
        {
            String filepath = @"c:\ipExternalLog\IPlog.txt";
            String message = "Service Stopped";
            CreateOrAppendFile(filepath, message);
        }
    }
}
