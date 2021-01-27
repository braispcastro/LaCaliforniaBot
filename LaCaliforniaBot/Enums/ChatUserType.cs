using System;

namespace LaCaliforniaBot.Enums
{
    [Flags]
    public enum ChatUserType
    {
        Broadcaster = 1,
        Moderator = 2,
        Subscriber = 4,
        Vip = 8,
        Pleb = 16
    }
}
