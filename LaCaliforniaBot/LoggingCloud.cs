using Google.Api;
using Google.Cloud.Logging.Type;
using Google.Cloud.Logging.V2;
using Google.Protobuf.WellKnownTypes;
using System;
using System.Collections.Generic;
using System.IO;

namespace LaCaliforniaBot
{
    public class LoggingCloud
    {
        private static LoggingCloud instance;
        public static LoggingCloud Instance
        {
            get { return instance ?? (instance = new LoggingCloud()); }
        }

        private readonly LogName logName;
        private readonly MonitoredResource monitoredResource;
        private readonly LoggingServiceV2Client client;

        public LoggingCloud()
        {
            var loggingCredentials = File.ReadAllText("files/credentials.json");
            client = new LoggingServiceV2ClientBuilder { JsonCredentials = loggingCredentials }.Build();

            logName = new LogName("lacaliforniabot", Configuration.BasicConfiguration.Channel.ToLowerInvariant());
            monitoredResource = new MonitoredResource { Type = "global" };
        }

        #region Public Methods

        public void WriteLog(LogSeverity logSeverity, string message, string description = null)
        {
            try
            {
                LogEntry logEntry = new LogEntry
                {
                    LogName = logName.ToString(),
                    Severity = logSeverity,
                    TextPayload = message,
                    JsonPayload = GetJsonPayload(message, description)
                };

                IDictionary<string, string> entryLabels = new Dictionary<string, string>
                {
                    { "Version", Configuration.AppVersion },
                    { "Environment", Configuration.Environment.ToString() },
                    { "Channel", Configuration.BasicConfiguration.Channel },
                    { "Prefix", Configuration.BasicConfiguration.Prefix.ToString() },
                    { "MessageSpeed", Configuration.BasicConfiguration.MessageSpeed.ToString() },
                    { "TTS_Enabled", Configuration.TextToSpeechEnabled.ToString() },
                    { "TTS_Delay", Configuration.TextToSpeechDelay.ToString() },
                    { "MachineName", Environment.MachineName },
                    { "UserDomainName", Environment.UserDomainName },
                    { "UserName", Environment.UserName }
                };

                client.WriteLogEntries(logName, monitoredResource, entryLabels, new[] { logEntry });
            }
            catch (Exception)
            {
                // Leave empty
            }
        }

        #endregion

        #region Private Methods

        private Struct GetJsonPayload(string message, string description)
        {
            try
            {
                var result = new Struct();
                result.Fields.Add("Message", Value.ForString(message));
                result.Fields.Add("Description", Value.ForString(description ?? string.Empty));
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion
    }
}
