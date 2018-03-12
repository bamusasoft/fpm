using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Globalization;
using System.IO;
using System.Xml.Linq;
using Prism.Logging;

namespace FlopManager.Services
{
  
    [Export(typeof(ILogger))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class XmlLogger:ILogger
    {
        private Action<string> _callback;
        private readonly Queue<string> _savedNotifications = new Queue<string>();


        [ImportingConstructor]
        public XmlLogger([Import("LogFilePath")] string logFilePath)
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                logFilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            }
            _filePath = logFilePath + @"\" + LOG_FILE_NAME;
        }

        private const string LOG_FILE_NAME = "FlopLog.xml";
        private const long MAX_FILE_SIZE = 100000;
        private readonly string _filePath;
        XDocument _xDoc;

        public void Log(string message, Category category, Priority priority)
        {
            var dateAndTime = DateTime.Now.ToString(CultureInfo.InvariantCulture);
            if (File.Exists(_filePath))
            {
                var info = new FileInfo(_filePath);
                var length = info.Length;
                if (length >= MAX_FILE_SIZE)
                {
                    info.Delete();
                    Log(message, category, priority);//Recursive
                }
            }
            if (File.Exists(_filePath))
            {
               WriteToExisitingFile(dateAndTime, message, category, priority);
            }
            else
            {
               WriteToNewFile(dateAndTime, message, category, priority);
            }

            _xDoc.Save(_filePath);
        }

        public void LogUiNotifications(string message)
        {
            _savedNotifications.Enqueue(message);
        }

        private void WriteToNewFile(string dateAndTime, string message, Category category, Priority priority)
        {
            _xDoc = new XDocument();
            XElement parent = new XElement("Logs");
            XElement subParent = new XElement("log");
            parent.Add(subParent);

            _xDoc = new XDocument(
                   new XElement("Logs",
                       new XElement("Log",
                           new XElement("DateAndTime", dateAndTime),
                           new XElement("Message", message),
                           new XElement("Category", category),
                           new XElement("Priority", priority))
                           ));
        }

        private void WriteToExisitingFile(string dateAndTime, string message, Category category, Priority priority)
        {
            _xDoc = XDocument.Load(_filePath);
            _xDoc.Element("Logs")?.Add(
                new XElement("Log",
                    new XElement("DateAndTime", dateAndTime),
                    new XElement("Message", message),
                    new XElement("Category", category),
                    new XElement("Priority", priority)
                    ));
        }

        /// <summary>
        /// Replays the saved logs if the Callback has been set.
        /// </summary>
        public void ReplaySavedLogs()
        {
            if (Callback != null)
            {
                while (_savedNotifications.Count > 0)
                {
                    string notification = _savedNotifications.Dequeue();
                    Callback(notification);
                }
            }
        }

        /// <summary>
        /// Gets or sets the callback to receive logs.
        /// </summary>
        /// <value>An Action&lt;string, Category, Priority&gt; callback.</value>
        public Action<string> Callback
        {
            get { return _callback; }
            set { _callback = value; }
        }
    }
}