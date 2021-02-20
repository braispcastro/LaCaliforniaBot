using System;
using Google.Cloud.Logging.Type;
using LaCaliforniaBot.Commands.Attributes;
using LaCaliforniaBot.Enums;

namespace LaCaliforniaBot.Commands.Items
{
    public class SlowModeInfo : BaseItem
    {
        private static SlowModeInfo instance;
        public static SlowModeInfo Instance
        {
            get { return instance ?? (instance = new SlowModeInfo()); }
        }

        private DateTime? lastUsed;

        [Command("slowinfo", ChatUserType.Pleb)]
        public void ToggleTextToSpeech(object[] args)
        {
            try
            {
                if (CanUseCommand())
                {
                    if (!Configuration.TextToSpeechEnabled)
                    {
                        lastUsed = DateTime.UtcNow;
                        TwitchBot.Instance.SendMessage($"La !k está desactivada y solo puede ser usada por los mods");
                    }
                    else if (Configuration.TextToSpeechDelay > 0)
                    {
                        lastUsed = DateTime.UtcNow;
                        TwitchBot.Instance.SendMessage($"No podrás usar la !k de nuevo hasta que pasen {Configuration.TextToSpeechDelay} segundos. Los mensajes no se quedan en cola, así que no spamees!");
                    }
                }
            }
            catch (Exception ex)
            {
                TwitchBot.Instance.LogMessage(LogSeverity.Error, ex.Message, ex.StackTrace);
            }
        }

        #region Private Methods

        private bool CanUseCommand()
        {
            return !lastUsed.HasValue || (DateTime.UtcNow - lastUsed.Value).TotalSeconds >= 30;
        }

        #endregion
    }
}
