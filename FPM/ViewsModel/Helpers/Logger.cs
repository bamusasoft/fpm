using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Fbm.ViewsModel.Properties;

namespace Fbm.ViewsModel.Helpers
{
    
    public static class Logger
    {
        const string LogFileName = "Log.xml";
        static string _filePath = Settings.Default.LogFilePath + "\\" + LogFileName;
        const long MaxFileSize = 100000;

        public static void Log(LogMessageTypes type, string msg, string targetSite, string trace)
        {
            string dateAndTime = DateTime.Now.ToString();
            XDocument xdoc = null;
            if (File.Exists(_filePath))
            {
                FileInfo info = new FileInfo(_filePath);
                var length = info.Length;
                if (length >= MaxFileSize)
                {
                    info.Delete();
                    Log(type, msg, targetSite, trace);
                }
               
            }
            if (File.Exists(_filePath))
            {
                xdoc = XDocument.Load(_filePath);
                xdoc.Element("Logs").Add(
                new XElement("Log",
                new XElement("DateAndTime", dateAndTime),
                new XElement("Type", type.ToString()),
                new XElement("Message",msg),
                new XElement("TargetSite", targetSite),
                new XElement("Tarce", trace)

                ));
            }
            else
            {
                xdoc = new XDocument(
                new XElement("Logs",
                    new XElement("Log",
                    new XElement("DateAndTime", dateAndTime),
                    new XElement("Type", type.ToString()),
                    new XElement("Message", msg),
                    new XElement("TargetSite", targetSite),
                    new XElement("Tarce", trace)

                    )));
            }

            xdoc.Save(_filePath);





        }
    }
}
