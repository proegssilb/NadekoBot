using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NadekoBot.Services.Database.Models
{
    public class UptimeLog : DbEntity
    {
        public ulong GuildId { get; set; }
        public ulong ChannelId { get; set; }
        [MaxLength(64)]
        public string Pokemon { get; set; }
        public decimal Lattitude { get; set; }
        public decimal Longitude { get; set; }
    }
}