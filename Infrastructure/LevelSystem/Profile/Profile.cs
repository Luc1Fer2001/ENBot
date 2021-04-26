using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.LevelSystem
{
    public class Profile : Entity
    {
        public ulong DiscordId { get; set; }
        public ulong ServerId { get; set; }
        public int Xp { get; set; }

    }
}
