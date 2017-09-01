using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NadekoBot.Services.Database.Models;

namespace NadekoBot.Services.Database.Repositories.Impl
{
    public class UptimeLogRepository : Repository<UptimeLog>, IUptimeLogRepository
    {
        public UptimeLogRepository(DbContext context) : base(context)
        {
        }
    }
}