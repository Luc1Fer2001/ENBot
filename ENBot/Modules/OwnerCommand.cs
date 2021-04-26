using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ENBot.Modules
{
    public class OwnerCommand : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _config;
        private readonly DiscordSocketClient _client;

        public OwnerCommand(IConfiguration config, DiscordSocketClient client)
        {
            _config = config;
            _client = client;
        }

        //life?
        [Command("живой?")]
        public async Task LifeAsync()
        {
            await Context.Channel.SendMessageAsync("Естественно!");
        }

        [Command("бот")]
        public async Task BotInfo()
        {
            /*var auhtor = (Context.User as SocketGuildUser).Id;
            if (auhtor == 561975249147527313)
            {*/
                var time = DateTime.Now - Process.GetCurrentProcess().StartTime;
                var upTime = "Бот запущен уже: ";

                if (time.Days > 0)
                {
                    if (time.Hours <= 0 || time.Minutes <= 0)
                    {
                        upTime += $"{time.Days} Day(s) и ";
                    }
                    else
                    {
                        upTime += $"{time.Days} Day(s),";
                    }
                }

                if (time.Hours > 0)
                {
                    if (time.Minutes > 0)
                    {
                        upTime += $" {time.Hours} Hour(s), ";
                    }
                    else
                    {
                        upTime += $"{time.Hours} Hour(s) и ";
                    }
                }

                if (time.Minutes > 0)
                {
                    upTime += $"{time.Minutes} Minute(s)";
                }

                if (time.Seconds >= 0)
                {
                    if (time.Hours > 0 || time.Minutes > 0)
                    {
                        upTime += $" и {time.Seconds} Second(s)";
                    }
                    else
                    {
                        upTime += $"{time.Seconds} Second(s)";
                    }
                }

                var process = Process.GetCurrentProcess();
                var mem = process.PrivateMemorySize64;
                var memory = mem / 1024 / 1024;
                var totalUsers = Context.Client.Guilds.Sum(guild => guild.MemberCount);

                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())// середнє зображення зправа
                    .WithTitle($"Информация бота **ENBot**")// назва сервера
                    .WithDescription($"Привет **{Context.User.Username}**, здесь ты можешь увидеть информацию о боте **ENBot**")// опис, типу текст в середині
                    .WithColor(new Color(32, 255, 0))// колір лінії зліва
                    .AddField("Пинг:", $"```fix\n{Context.Client.Latency}ms```", true)
                    .AddField("Версия бота:", $"```fix\n{_config["version"]}```", true)
                    .AddField("Стоит на:", $"```fix\n{Context.Client.Guilds.Count} серверах```", true)
                    .AddField("Статус текущего\nзапуска:", "```fix\nзапущен в\nтестовом режиме```", true)
                    .AddField("Статус разработки бота:\n", "```fix\nв активной разработке```", true)
                    .AddField("Количество пользователей:", $"```fix\n{totalUsers}```", true)
                    .AddField("Сожрал памяти:", $"```fix\n{memory}Mb```", true)
                    .AddField("Время активности:", $"```prolog\n{upTime}```", true)
                    .WithFooter(
                    x =>
                    {
                        x.WithText($"Ух как много всего) ©️{Context.Client.CurrentUser.Username}");
                        x.WithIconUrl(Context.Client.CurrentUser.GetAvatarUrl());
                    })
                    .WithCurrentTimestamp();// час коли відправлено повідомлення
                var ember = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, ember);
            /*}
            else
            {
                await Context.Channel.SendMessageAsync("Извини но данную команду может использовать **только** мой создатесь @𝕷𝖚𝖈𝖎𝖋𝖊𝖗");
            }*/
        }

        [Command("создать")]
        public async Task Create()
        {
            var auhtor = (Context.User as SocketGuildUser).Id;
            if (auhtor == 561975249147527313)
            {
                
                var createdChannel = await Context.Guild.CreateTextChannelAsync($"user-counter");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Извини но ты не мой создатель, чтобы указывать мне!");
            }
        }

        [Command("статусбота")]
        public async Task StatusBot(string typetext, [Remainder] string status)
        {
            var activtype = ActivityType.Watching;
            var auhtor = (Context.User as SocketGuildUser).Id;
            if (auhtor == 561975249147527313)
            {
                if (typetext == "играет")
                {
                    activtype = ActivityType.Playing;
                    await _client.SetGameAsync($"{status}", null, activtype);
                }
                if (typetext == "смотрит")
                {
                    activtype = ActivityType.Watching;
                    if (status == "количество")
                    {
                        await _client.SetGameAsync($"Количество серверов: {_client.Guilds.Count}", null, activtype);
                    }
                    else
                    {
                        await _client.SetGameAsync($"{status}", null, activtype);
                    }
                }
                if (typetext == "слушает")
                {
                    activtype = ActivityType.Listening;
                    await _client.SetGameAsync($"{status}", null, activtype);
                }
                if (typetext == "стримит")
                {
                    activtype = ActivityType.Streaming;
                    await _client.SetGameAsync($"{status}", null, activtype);
                }

                var messages = await Context.Channel.GetMessagesAsync(1).FlattenAsync();
                await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.Client.CurrentUser.GetAvatarUrl())// середнє зображення зправа
                    .WithTitle($"Настройка бота **ENBot**")// назва сервера
                    .WithDescription($"Привет **{Context.User.Username}**, ты сменил мой статус)\nНовый статус:\n{typetext} {status}!\nМне нравится такой статус🥰🥰🥰")// опис, типу текст в середині
                    .WithColor(new Color(255, 0, 200))// колір лінії зліва
                    .WithCurrentTimestamp();// час коли відправлено повідомлення
                var ember = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, ember);
            }
            else
            {
                await Context.Channel.SendMessageAsync("Извини но ты не мой создатель, чтобы указывать мне!");
            }
        }
        
        [Command("стат")]
        public async Task Counter([Remainder] string status)
        {
            var auhtor = (Context.User as SocketGuildUser).Id;
            if (auhtor == 561975249147527313)
            {
                int memberCount = (Context.Guild as SocketGuild).MemberCount;

                var channelName = Context.Guild.TextChannels.FirstOrDefault(x => x.Name == "user-counter");
                var channelId = channelName.Id;

                var targetChannel = Context.Guild.GetChannel(channelId);
                await targetChannel.ModifyAsync(prop => prop.Name = $"user-counter {memberCount}");

                await Context.Channel.SendMessageAsync($"{channelId}");
            }
            else
            {
                await Context.Channel.SendMessageAsync("Извини но ты не мой создатель, чтобы указывать мне!");
            }
        }

    }
}
