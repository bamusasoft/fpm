using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Logging;

namespace FlopManager.Services
{
    /// <summary>
    /// To create custom logger away from prism default one, which come with MefBootstrapper
    /// </summary>
    public interface ILogger
    {
        void Log(string message, Category category, Priority priority);
        void LogUiNotifications(string message);

        /// <summary>
        /// Replays the saved logs if the Callback has been set.
        /// </summary>
        void ReplaySavedLogs();
        Action<string> Callback { get; set; }
       
    }
}
