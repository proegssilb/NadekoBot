using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using NadekoBot.Extensions;
using System.Net.Http;
using System.Collections.Concurrent;
using NadekoBot.Common;
using NadekoBot.Common.Attributes;
using NadekoBot.Services;
using NadekoBot.Services.Database;
using NadekoBot.Modules.Uptime.Services;
using NadekoBot.Services.Database.Models;

namespace NadekoBot.Modules.Uptime 
{
    public static class UptimeExtensions
    {
        public static bool ToBool(this Uptime.EnableDisable endis)
        {
            return endis == Uptime.EnableDisable.enable;
        }
    }

    public class Uptime : NadekoTopLevelModule<LogDataService>
    {
        public enum EnableDisable {disable = 0, enable = 1}

        private DiscordSocketClient _client;
        private IBotCredentials _credentials;
        private DbService _db;

        public Uptime(NadekoBot nadeko, DiscordSocketClient client, IBotCredentials credentials, DbService db, IUnitOfWork uow) {
            _client = client;
            _credentials = credentials;
            _db = db;
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        [OwnerOnly]
        public async Task UptimeLog(EnableDisable enable=EnableDisable.enable) {
            await LogCommon(Context.Guild.Id, Context.Channel.Id, enable.ToBool()).ConfigureAwait(false);
        }

        [NadekoCommand, Usage, Description, Aliases]
        [RequireContext(ContextType.Guild)]
        [OwnerOnly]
        public async Task UptimeLog(ITextChannel channel, EnableDisable enable=EnableDisable.enable) {
            await LogCommon(Context.Guild.Id, channel.Id, enable.ToBool()).ConfigureAwait(false);
        }

        private async Task LogCommon(ulong guildId, ulong channelId, bool enable)
        {
            using (var uow = _db.UnitOfWork)
            {
                var channelSettings = await uow.UptimeChannels.GetByChannelId(guildId, channelId);
                if (channelSettings == default(UptimeChannel))
                {
                    channelSettings = new UptimeChannel {ChannelId = channelId, GuildId = guildId, Enabled=enable};
                    uow.UptimeChannels.Add(channelSettings);
                }
                else
                {
                    channelSettings.Enabled = enable;
                    uow.UptimeChannels.Update(channelSettings);
                }
                await uow.CompleteAsync();
                if (enable)
                {
                    _service.EnableChannel(channelSettings);
                    await ReplyConfirmLocalized("log_enabled", _client.GetChannel(channelId)).ConfigureAwait(false);
                }
                else
                {
                    _service.DisableChannel(channelSettings);
                    await ReplyConfirmLocalized("log_disabled", _client.GetChannel(channelId)).ConfigureAwait(false);
                }
            }
        }
    }
}