using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;
using System.Collections.Concurrent;
using System.Threading;
using Victoria.EventArgs;
using ENBot.Common;
using Discord.Addons.Interactive;
using Infrastructure;
using ENBot.Services;
using Discord.WebSocket;

namespace ENBot.Modules
{
    public class MusicModule : InteractiveBase//, ModuleBase<SocketCommandContext>
    {
        private readonly LavaNode _lavaNode;
        private readonly ConcurrentDictionary<ulong, CancellationTokenSource> _disconnectTokens;

        public MusicModule(LavaNode lavaNode)
        {
            _lavaNode = lavaNode;
            _disconnectTokens = new ConcurrentDictionary<ulong, CancellationTokenSource>();
        }

        [Command("играть", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Connect)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task PlayAsync([Remainder] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Пожалуйста, укажите условия поиска.");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Вы должны быть подключены к голосовому каналу!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.JoinAsync(voiceState.VoiceChannel, Context.Channel as ITextChannel);
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }

            var searchResponse = await _lavaNode.SearchYouTubeAsync(query);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", $"Я не смог найти ничего для `{query}`.");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {

                var track = searchResponse.Tracks[0];
                var track1 = searchResponse.Tracks[1];
                var track2 = searchResponse.Tracks[2];
                var track3 = searchResponse.Tracks[3];
                var track4 = searchResponse.Tracks[4];
                await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Выберите песню из списка используя '1', '2'...\n\n\n " +
                    $"1. **{track.Title} ({track.Duration})**\n\n" +
                    $"2. **{track1.Title} ({track1.Duration})**\n\n" +
                    $"3. **{track2.Title} ({track2.Duration})**\n\n" +
                    $"4. **{track3.Title} ({track3.Duration})**\n\n" +
                    $"5. **{track4.Title} ({track4.Duration})**"
                    );
                var response = await NextMessageAsync(true, true, new TimeSpan(0, 0, 60));
                if (response != null)
                {
                    if (response.Content == "1")
                    {
                        player.Queue.Enqueue(track);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Поставлен в очередь: **{track.Title} ({track.Duration})**");
                    }
                    else if (response.Content == "2")
                    {
                        player.Queue.Enqueue(track1);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Поставлен в очередь: **{track1.Title} ({track1.Duration})**");
                    }
                    else if (response.Content == "3")
                    {
                        player.Queue.Enqueue(track2);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Поставлен в очередь: **{track2.Title} ({track2.Duration})**");
                    }
                    else if (response.Content == "4")
                    {
                        player.Queue.Enqueue(track3);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Поставлен в очередь: **{track3.Title} ({track3.Duration})**");
                    }
                    else if (response.Content == "5")
                    {
                        player.Queue.Enqueue(track4);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Поставлен в очередь: **{track4.Title} ({track4.Duration})**");
                    }
                }
                else
                {
                    await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", $"Вы не выбрали песню для поиска из списка, пожалуйста повторите поиск.");
                    return;
                }
            }
            else
            {

                var track = searchResponse.Tracks[0];
                var track1 = searchResponse.Tracks[1];
                var track2 = searchResponse.Tracks[2];
                var track3 = searchResponse.Tracks[3];
                var track4 = searchResponse.Tracks[4];
                await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Выберите песню из списка написав '1', '2'...\n\n\n " +
                    $"1. **{track.Title} ({track.Duration})**\n\n" +
                    $"2. **{track1.Title} ({track1.Duration})**\n\n" +
                    $"3. **{track2.Title} ({track2.Duration})**\n\n" +
                    $"4. **{track3.Title} ({track3.Duration})**\n\n" +
                    $"5. **{track4.Title} ({track4.Duration})**"
                    );
                var response = await NextMessageAsync(true, true, new TimeSpan(0, 0, 60));
                if (response != null)
                {
                    if (response.Content == "1")
                    {
                        await track.FetchArtworkAsync();
                        await player.PlayAsync(track);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{track.Title} ({track.Duration})**");
                    }
                    else if (response.Content == "2")
                    {
                        await track1.FetchArtworkAsync();
                        await player.PlayAsync(track1);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{track1.Title} ({track1.Duration})**");
                    }
                    else if (response.Content == "3")
                    {
                        await track2.FetchArtworkAsync();
                        await player.PlayAsync(track2);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{track2.Title} ({track2.Duration})**");
                    }
                    else if (response.Content == "4")
                    {
                        await track3.FetchArtworkAsync();
                        await player.PlayAsync(track3);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{track3.Title} ({track3.Duration})**");
                    }
                    else if (response.Content == "5")
                    {
                        await track4.FetchArtworkAsync();
                        await player.PlayAsync(track4);
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{track4.Title} ({track4.Duration})**");
                    }
                }
                else
                {
                    await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", $"Вы не выбрали песню для поиска из списка, пожалуйста повторите поиск.");
                    return;
                }
            }
        }

        [Command("скип", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task Skip()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Вы должны быть подключены к голосовому каналу!.");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Я не подключен к голосовому каналу!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Вы должны быть в том же голосовом канале, что и я!");
                return;
            }

            if (player.Queue.Count == 0)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "В очереди больше нет песен.");
                return;
            }

            await player.SkipAsync();
            await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Песня пропущена! Сейчас играет **{player.Track.Title} ({player.Track.Duration})**");
        }

        [Command("пауза", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task Pause()
        {
            var voiceState = Context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Вы должны быть подключены к голосовому каналу!");
                return;
            }

            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Я не подключен к голосовому каналу!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);

            if (voiceState.VoiceChannel != player.VoiceChannel)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Вы должны быть в том же голосовом канале, что и я!");
                return;
            }

            if (player.PlayerState == PlayerState.Paused || player.PlayerState == PlayerState.Stopped)
            {
                await player.ResumeAsync();
                await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Музыка возобновлена!");///
            }
            else
            {
                await player.PauseAsync();
                await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Музыка преиостановлена!");
            }
        }

        [Command("плейлист", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task ListAsync()
        {
            try
            {
                var descriptionBuilder = new StringBuilder();

                var player = _lavaNode.GetPlayer(Context.Guild);
                if (player == null)
                {
                    await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", $"Музыка сейчас не используется.");
                    return;
                }

                if (player.PlayerState is PlayerState.Playing)
                {
                    if (player.Queue.Count < 1 && player.Track != null)
                    {
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{player.Track.Title} ({player.Track.Duration})**\nБольше ничего не стоит в очереди.");
                        return;
                    }
                    else
                    {
                        var trackNum = 2;
                        foreach (LavaTrack track in player.Queue)
                        {
                            descriptionBuilder.Append($"{trackNum}: [{track.Title}]({track.Url})\n");
                            trackNum++;
                        }
                        await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Сейчас играет: **{player.Track.Title} ({player.Track.Duration})**({player.Track.Url}) \n{descriptionBuilder}");
                        return;
                    }
                }
                else
                {
                    await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Похоже что сейчас музыка не играет.");
                    return;
                }
            }
            catch (Exception ex)
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", $"Похоже что сейчас музыка не используется\nКод ошибки который вам **может** поможет:\n{ex.Message}");
                return;
            }

        }

        [Command("дисконект", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task Disconnect()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Я не подключен к голосовому каналу!");
                return;
            }

            var voiceState = Context.User as IVoiceState;
            if (_lavaNode.HasPlayer(Context.Guild))
            {
                try
                {
                    await _lavaNode.LeaveAsync(voiceState.VoiceChannel);
                }
                catch (Exception exception)
                {
                    await ReplyAsync(exception.Message);
                }
            }
        }

        [Command("стоп", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(ChannelPermission.Speak)]
        public async Task Stop()
        {
            if (!_lavaNode.HasPlayer(Context.Guild))
            {
                await Context.Channel.CreateErrorEmbed(":headphones: Музыка :headphones:", "Я не подключен к голосовому каналу!");
                return;
            }

            var player = _lavaNode.GetPlayer(Context.Guild);
            player.Queue.Clear();
            await player.StopAsync();
            await Context.Channel.CreateSuccessfullyEmbed(":headphones: Музыка :headphones:", $"Очередь очищена");
            await InitiateDisconnectAsync(player, TimeSpan.FromSeconds(60));

        }

        public async Task InitiateDisconnectAsync(LavaPlayer player, TimeSpan timeSpan)
        {
            if (!_disconnectTokens.TryGetValue(player.VoiceChannel.Id, out var value))
            {
                value = new CancellationTokenSource();
                _disconnectTokens.TryAdd(player.VoiceChannel.Id, value);
            }
            else if (value.IsCancellationRequested)
            {
                _disconnectTokens.TryUpdate(player.VoiceChannel.Id, new CancellationTokenSource(), value);
                value = _disconnectTokens[player.VoiceChannel.Id];
            }

            var builder = new EmbedBuilder()
                .WithTitle($":no_entry: Упс... Ошибка(")
                .WithDescription($"**Модуль с которым произошла ошибка:\n:headphones: Музыка :headphones:**\n\n**Подробности ошибки**: \nМне нечего играть! Я отключусь через: {timeSpan} секунд")
                .WithColor(new Color(255, 0, 0))
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await player.TextChannel.SendMessageAsync(null, false, embed);

            var isCancelled = SpinWait.SpinUntil(() => value.IsCancellationRequested, timeSpan);
            if (isCancelled)
            {
                return;
            }

            if (player.PlayerState == PlayerState.Playing)
            {
                var builder1 = new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n:headphones: Музыка :headphones:**\n\n**Содержание сообщения:** \n\nХорошая песня, я пока что останусь)")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp();
                var embed1 = builder1.Build();
                await player.TextChannel.SendMessageAsync(null, false, embed1);
                return;
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);
            var builder0 = new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n:headphones: Музыка :headphones:**\n\n**Содержание сообщения:** \n\nЗахочешь что-то послушать позови")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp();
            var embed0 = builder0.Build();
            await player.TextChannel.SendMessageAsync(null, false, embed0);
        }

        public async Task OnTrackStarted(TrackStartEventArgs arg)
        {
            if (!_disconnectTokens.TryGetValue(arg.Player.VoiceChannel.Id, out var value))
            {
                return;
            }

            if (value.IsCancellationRequested)
            {
                return;
            }

            value.Cancel(true);
            var builder = new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n:headphones: Музыка :headphones:**\n\n**Содержание сообщения:** \n\nХорошая песня, я пока что останусь)")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await arg.Player.TextChannel.SendMessageAsync(null, false, embed);
        }

        public async Task TrackEnded(TrackEndedEventArgs args)
        {
            if (!args.Reason.ShouldPlayNext())
            {
                return;
            }

            var player = args.Player;
            if (!player.Queue.TryDequeue(out var queueable))
            {
                _ = InitiateDisconnectAsync(args.Player, TimeSpan.FromSeconds(60));
                return;
            }

            if (!(queueable is LavaTrack track))
            {
                var builder1 = new EmbedBuilder()
                .WithTitle($":no_entry: Упс... Ошибка(")
                .WithDescription($"**Модуль с которым произошла ошибка:\n:headphones: Музыка :headphones:**\n\n**Подробности ошибки**: \nПлейлист пуст.")
                .WithColor(new Color(255, 0, 0))
                .WithCurrentTimestamp();
                var embed1 = builder1.Build();
                await player.TextChannel.SendMessageAsync(null, false, embed1);
                return;
            }

            await args.Player.PlayAsync(track);
            var builder = new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n:headphones: Музыка :headphones:**\n\n**Содержание сообщения:** \n\nСейчас играет **{track.Title} ({track.Duration})**\n({track.Url})")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await player.TextChannel.SendMessageAsync(null, false, embed);
        }
    }
}
