using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using NadekoBot.Common.Replacements;
using NadekoBot.Extensions;
using NadekoBot.Services;
using NadekoBot.Services.Database;
using NadekoBot.Services.Database.Models;
using NadekoBot.Services.Impl;
using NLog;

namespace NadekoBot.Modules.Uptime.Services
{
    public class LogDataService : INService
    {
        private DiscordSocketClient _client;
        private DbService _db;
        private List<UptimeChannel> _activeChannels;
        private readonly Logger _log;

        private static readonly Regex pokemonTitle = new Regex("([a-zA-Z'0-9 ]*) (?:Sighting|has appeared!)");
        private static readonly Regex googleMapsUrl = new Regex("//maps\\.google\\.com/.*q=([-0-9.]+,[-0-9.]+)");

        public LogDataService(NadekoBot nadeko, DiscordSocketClient client, IBotCredentials credentials, DbService db, IUnitOfWork uow)
        {
            _log = LogManager.GetCurrentClassLogger();
            _client = client;
            _db = db;
            _activeChannels = uow.UptimeChannels.GetEnabled().ToList();
            _client.MessageUpdated += HandleMessageUpdated;
            _client.MessageReceived += HandleMessageReceived;
            // _log.Info("Logging channgels: {0}", String.Join(",", _activeChannels.Select(c => c.ChannelId)));
        }

        public void EnableChannel(UptimeChannel channel)
        {
            if (!channel.Enabled) return;
            try
            {
                var chan = _activeChannels.First(ch => ch.ChannelId == channel.ChannelId);
                chan.Enabled = channel.Enabled;
            } 
            catch (InvalidOperationException)
            {
                _activeChannels.Add(channel);
            }
        }

        public void DisableChannel(UptimeChannel channel)
        {
            if (channel.Enabled) return;
            _activeChannels.RemoveAll(ch => ch.ChannelId == channel.ChannelId);
        }

        private Task HandleMessageUpdated(Cacheable<IMessage, ulong> optmsg, SocketMessage imsg2, ISocketMessageChannel ch)
        {
            if(!_activeChannels.Any(chan => chan.ChannelId == ch.Id && chan.Enabled)) return Task.CompletedTask;
            // var _ = Task.Run( () => {});
            return Task.CompletedTask;
        }

        private Task HandleMessageReceived(SocketMessage imsg)
        {
            _log.Info("Received message.");
            var msg = imsg as IUserMessage;
            var chan = msg?.Channel as ITextChannel;
            if (msg == null || chan == null)
            {
                // _log.Info("Null message or channel.");
                return Task.CompletedTask;
            }
            if (!_activeChannels.Any(ch => chan.Id == ch.ChannelId && ch.Enabled))
            {
                // _log.Info("No matching channels.");
                return Task.CompletedTask;
            }
            var _ = Task.Run(async () =>
            {
                // This code should be broken out into a MessageParser/MessageHandler, but currently there's
                // only one use case, so why bother?
                var embed = msg.Embeds.First(e => !String.IsNullOrEmpty(e.Title));
                if (embed == null) return;
                var username = msg.Author.Username;
                var title = embed.Title;
                var url = embed.Url;
                var userMatch = pokemonTitle.Match(username);
                var gpsMatch = googleMapsUrl.Match(url);

                if (userMatch.Success && gpsMatch.Success)
                {
                    var pokemon = userMatch.Groups[1].Value;
                    var gpscoords = gpsMatch.Groups[1].Value.Split(',').Select(c => decimal.Parse(c)).ToArray();
                    var lat = gpscoords[0];
                    var lng = gpscoords[1];
                    var logEntry = new UptimeLog()
                    {
                        DateAdded = DateTime.Now,
                        GuildId = chan.GuildId,
                        ChannelId = chan.Id,
                        Pokemon = pokemon,
                        Lattitude = lat,
                        Longitude = lng
                    };
                    using (var uow = _db.UnitOfWork)
                    {
                        uow.UptimeLog.Add(logEntry);
                        await uow.CompleteAsync().ConfigureAwait(false);
                    }
                }
            });
            return Task.CompletedTask;
        }
    }
}