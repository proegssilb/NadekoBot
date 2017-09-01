using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NadekoBot.Services.Database.Models
{
    [Table("UptimeChannel")]
    public class UptimeChannel : DbEntity
    {
        public ulong GuildId { get; set; }
        
        public ulong ChannelId { get; set; }

        public bool Enabled { get; set; }

    }
}