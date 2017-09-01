using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NadekoBot.Services.Database.Models;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class UptimeRepository : Repository<UptimeChannel>, IUptimeRepository
    {
        public UptimeRepository(DbContext context) : base(context)
        {
        }

        public async Task<UptimeChannel> GetByChannelId(ulong guildId, ulong channelId) => 
            await _set.FirstOrDefaultAsync(i => i.ChannelId == channelId && i.GuildId == guildId);

        public IEnumerable<UptimeChannel> GetEnabled() => 
            _set.Where(i => i.Enabled == true);
    }
}