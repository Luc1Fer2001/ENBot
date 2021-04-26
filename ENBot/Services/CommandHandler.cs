using System;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using ENBot.Utilities;
using ENBot.Modules;
using Victoria;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using ENBot.Common;
using System.Linq;

namespace ENBot.Services
{
    public class CommandHandler
    {
        private readonly IServiceProvider _services;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;
        private readonly Servers _servers;
        private readonly AutoRolesHelper _autoRolesHelper;
        private readonly LavaNode _lavaNode;
        private readonly Images _images;
        private readonly MusicModule _musicModule;
        private readonly Microsoft.Extensions.Logging.ILogger _logger;
        public static List<Mute> Mutes = new List<Mute>();
        /*private static readonly string text = "{";
        private static readonly string text1 = "}";
        private static readonly string text2 = "public void";
        private static readonly string text3 = "private void";
        private static readonly string text4 = "if(";
        private static readonly string text5 = "foreach(";
        private static readonly string text6 = "for(";
        private static readonly string text7 = "if (";
        private static readonly string text8 = "protected void";
        private static readonly string text9 = "virtual protectet void";*/

        //private string[] keyWords = new string[] {"{", "}", "privat void", "" };
        private static string[] words = new string[] { "if", "{", "}", "int", "string" };
        private static int severityLevel = 2;

        public CommandHandler(IServiceProvider services)
        {
            _services = services;
            _client = services.GetRequiredService<DiscordSocketClient>();
            _commands = services.GetRequiredService<CommandService>();
            _logger = services.GetRequiredService<ILogger<CommandHandler>>();
            _servers = services.GetRequiredService<Servers>();
            _autoRolesHelper = services.GetRequiredService<AutoRolesHelper>();
            _lavaNode = services.GetRequiredService<LavaNode>();
            _images = services.GetRequiredService<Images>();
            _musicModule = services.GetRequiredService<MusicModule>();
            

            _commands.CommandExecuted += CommandExecutedAsync;
            _client.MessageReceived += MessageReceivedAsync;
            _client.MessageReceived += OnUserMessageAsync;
        }

        private async Task OnUserMessageAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            if (message.Channel is IDMChannel)
            {
                return;
            }

            var autoCode = await _servers.GetAutoCodeRedactAsync((message.Channel as SocketGuildChannel).Guild.Id);
            
            if (autoCode == false)
            {
                //return;
            }
            else
            {
                //if (StringContainsCodeWord(message.Content))
                if(IsCode(message))
                {
                    var code = message;
                    await message.DeleteAsync();
                    var context1 = new SocketCommandContext(_client, message);
                    await context1.Channel.SendMessageAsync("```cs\n" + code + "\n```");
                }
            }
        }

        public static bool IsCode(SocketMessage message)
        {
            int similiarCount = 0;

            // Считывание сопоставлений
            foreach (var word in words)
            {
                if (message.Content.Contains(word))
                {
                    similiarCount++;
                }
            }

            if (similiarCount >= severityLevel)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /*private bool StringContainsCodeWord(string string1)
        {
            string[] words = string1.Split(' ');

            foreach(var word in words)
            {
                foreach(var keyWord in keyWords)
                {
                    if(String.Compare(word, keyWord, true) == 0) return true;
                }
            }
            return false;
        }*/

        public async Task InitializeAsync()
        {
            _client.Ready += OnReadyAsync;
            _client.UserJoined += OnUserJoined;
            _lavaNode.OnTrackStarted += _musicModule.OnTrackStarted;
            _lavaNode.OnTrackEnded += _musicModule.TrackEnded;

            var newTask = new Task(async () => await MuteHandler());
            newTask.Start();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
        }

        private async Task MuteHandler()
        {
            List<Mute> Remoove = new List<Mute>();

            foreach(var mute in Mutes)
            {
                if(DateTime.Now < mute.End)
                {
                    continue;
                }

                var guild = _client.GetGuild(mute.Guild.Id);

                if(guild.GetRole(mute.Role.Id) == null)
                {
                    Remoove.Add(mute);
                    continue;
                }

                var role = guild.GetRole(mute.Role.Id);

                if(guild.GetUser(mute.User.Id) == null)
                {
                    Remoove.Add(mute);
                    continue;
                }

                var user = guild.GetUser(mute.User.Id);

                if(role.Position > guild.CurrentUser.Hierarchy)
                {
                    Remoove.Add(mute);
                    continue;
                }

                await user.RemoveRoleAsync(mute.Role);
                Remoove.Add(mute);
            }

            Mutes = Mutes.Except(Remoove).ToList();

            await Task.Delay(1 * 60 * 1000);
            await MuteHandler();
        }

        private async Task OnUserJoined(SocketGuildUser arg)
        {
            
            var newTask = new Task(async () => await HandleUserJoined(arg));
            newTask.Start();
        }

        private async Task HandleUserJoined(SocketGuildUser arg)
        {
            var roles = await _autoRolesHelper.GetAutoRolesAsync(arg.Guild);
            if (roles.Count > 0)
                await arg.AddRolesAsync(roles);

            var channelId = await _servers.GetWelcomeAsync(arg.Guild.Id);
            if (channelId == 0)
                return;

            var channel = arg.Guild.GetTextChannel(channelId);
            if (channel == null)
            {
                await _servers.ClearWelcomeAsync(arg.Guild.Id);
                return;
            }

            var background = await _servers.GetBackgroundAsync(arg.Guild.Id);
            string path = await _images.CreateImageAsync(arg, background);

            await channel.SendFileAsync(path, null);
            System.IO.File.Delete(path);
        }

        private async Task MessageReceivedAsync(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            

            if (message.Channel is IDMChannel)
            {
                await Extensions.CreateErrorEmbed(message.Channel, ":robot:Бот:robot:", "извини но ты можешь писать мне только на сервере!");
                return;
            }

            /*var autoCode = await _servers.GetAutoCodeRedactAsync((message.Channel as SocketGuildChannel).Guild.Id);

            if (autoCode == false)
            {
                //return;
            }
            else
            {
                //if (StringContainsCodeWord(message.Content))
                if (message.Content.Contains("{") || message.Content.Contains("}") || message.Content.Contains("public void") || message.Content.Contains("private void") || message.Content.Contains("if(") || message.Content.Contains("foreach(") || message.Content.Contains("for(")
                    || message.Content.Contains("if (") || message.Content.Contains("protected void") || message.Content.Contains("virtual protectet void"))
                {
                    var code = message;
                    await message.DeleteAsync();
                    var context1 = new SocketCommandContext(_client, message);
                    await context1.Channel.SendMessageAsync("```cs\n" + code + "\n```");
                }
            }*/

            if (message.Content.Contains("https://discord.gg/") )
            {
                
                if (!(message.Channel as SocketGuildChannel).Guild.GetUser(message.Author.Id).GuildPermissions.Administrator)
                {
                    var antiInvait = await _servers.GetInvaitAsync((message.Channel as SocketGuildChannel).Guild.Id);
                    if (antiInvait == false)
                    {
                        return;
                    }
                    await message.DeleteAsync();
                    await Extensions.CreateErrorEmbed(message.Channel, ":shushing_face:Авто-Модерация:shushing_face:", $"{message.Author.Mention} извини но на этом сервере тебе запрещенно зармещять ссылки на другие сервера Дискорд");
                    return;
                }
            }

            var argPos = 0;
            var prefix = await _servers.GetGuildPrefix((message.Channel as SocketGuildChannel).Guild.Id) ?? "*";
            if (!message.HasStringPrefix(prefix, ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }
        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected)
            {
                await _lavaNode.ConnectAsync();
            }
            await _client.SetGameAsync(" за тобой 👀", null, ActivityType.Watching);
            //await _client.SetGameAsync(" test", null, ActivityType.Watching);
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!command.IsSpecified)
            {
                _logger.LogError($"Пользователь [{context.User.Username}] Далбоёб(даже команду правильно не может написать) <-> [{result.ErrorReason}]!");
                return;
            }

            if (result.IsSuccess)
            {
                //_logger.LogInformation($"Command [{command.Value.Name}] executed for [{context.User.Username}] on [{context.Guild.Name}]");
                return;
            }
            var builder = new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n:robot:Команды:robot:**\n\n**Содержание сообщения:** \n\nПохоже такой команды не существует, или она была написана неверно.\nКод ошибки: \n{result}")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp();
            var embed = builder.Build();
            await context.Channel.SendMessageAsync(null, false, embed);
        }
    }
}