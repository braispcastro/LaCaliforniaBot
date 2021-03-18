using System;

namespace LaCaliforniaBot.Model
{
    public class AllowedUser
    {
        public string Username { get; set; }
        public int MinutesAllowed { get; set; }
        public DateTime AllowedAt { get; set; }
    }
}
