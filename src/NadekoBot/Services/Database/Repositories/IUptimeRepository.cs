using NadekoBot.Services.Database.Models;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace NadekoBot.Services.Database.Repositories
{
    public interface IUptimeRepository : IRepository<UptimeChannel>
    {
        Task<UptimeChannel> GetByChannelId(ulong guildId, ulong channelId);
        IEnumerable<UptimeChannel> GetEnabled();
    }
}