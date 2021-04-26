using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Faith_Discord_Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Faith_Discord_Main.Modules
{
    public class ServerStatsModule : ModuleBase<SocketCommandContext>
    {
        private readonly ServerStats _ServerStats;
        private readonly DiscordSocketClient _client;
 
        private ulong server_id = 0;
        private string oldName = string.Empty;
        private List<ulong> OnCoolDown = new List<ulong>();
 
        public ServerStatsModule(ServerStats serverstats, DiscordSocketClient client)
        {
            _ServerStats = serverstats;
            _client = client;
        }
 
        [Command("Count")]
        public async Task CountAsync()
        {
            var membercount = (Context.Guild as SocketGuild).MemberCount;
            var boosterscount = (Context.Guild as SocketGuild).PremiumSubscriptionCount;
            var onlinecount = Context.Guild.Users.Where((x) => x.Status == UserStatus.Online).Count();
            var botscount = Context.Guild.Users.Where((x) => x.IsBot).Count();
            var people = membercount - botscount;
 
            await ReplyAsync($"All Members: {membercount} \nMembers {people} \nBoosters: {boosterscount} \nOnline: {onlinecount} \nBots: {botscount}");
        }
 
        [Command("SetupServerStats")]
        [Summary("Set weather or not to use Server stats")]
        public async Task SetupServerStatsAsync()
        {
            await CreateServerStatsAsync();
        }
 
        public async Task CreateServerStatsAsync()
        {
           /* TODO: 
             *       Add Server ID to database  +++ Done
             *       Add Catagory ID to database +++ Done
             *       Add Channel ID to database +++ Done
             *       remove Names from database +++ Done
             */
 
            await Context.Channel.TriggerTypingAsync();
                
            var membercount = (Context.Guild as SocketGuild).MemberCount;
            var botscount = Context.Guild.Users.Where((x) => x.IsBot).Count();
            var people = membercount - botscount;
            var boosterscount = (Context.Guild as SocketGuild).PremiumSubscriptionCount;
 
            string CategoryName = "Server-Stats";
            var category = await Context.Guild.CreateCategoryChannelAsync($"{CategoryName}");
            await category.ModifyAsync((x) =>
            {
                x.Position = 0;
            });
 
            string AllMembersName = $"All-Members: {membercount}";     
            var allmembers = await Context.Guild.CreateVoiceChannelAsync($"{AllMembersName}");
            await allmembers.ModifyAsync((x) =>
            {
                x.CategoryId = category.Id;
            });
 
            string HumanMembersName = $"People: {people}";
            var humanMmembers = await Context.Guild.CreateVoiceChannelAsync($"{HumanMembersName}");
            await humanMmembers.ModifyAsync((x) =>
            {
                x.CategoryId = category.Id;
            });
 
            string BotMembersName = $"Bots: {botscount}";
            var botMembers = await Context.Guild.CreateVoiceChannelAsync($"{BotMembersName}");
            await botMembers.ModifyAsync(x =>
            {
                x.CategoryId = category.Id;
            });
 
            string BoosterName = $"Boosters: {boosterscount}";
            var boosters = await Context.Guild.CreateVoiceChannelAsync($"{BoosterName}");
            await boosters.ModifyAsync(x =>
            {
                x.CategoryId = category.Id;
            });
 
            await _ServerStats.SaveChannelIdsAsync(Context.Guild.Id,category.Id, allmembers.Id, humanMmembers.Id, botMembers.Id, boosters.Id);
        }
 
        public async Task RemoveServerStats()
        {
            await Task.Delay(1000);
        }
        public async Task AutoServerStatsUpdate(bool Cancellation)
        {
 
            while (true)
            {
                Console.WriteLine("\nAuto Stat Update\n");
                await Task.Delay(6000);
                //await ServerStatsUpdate();
                
                if (Cancellation)
                    break;
            }
        }
 
        public async Task ServerStatsUpdate()
        {
            var serverIds = await _ServerStats.GetServerIdsAsync();
            foreach (ulong serverId in serverIds)
            {
                server_id = serverId;
            }
 
            var guild = _client.GetGuild(server_id);
 
            var membercount = guild.MemberCount;
            var boosterscount = guild.PremiumSubscriptionCount;
            var onlinecount = guild.Users.Where((x) => x.Status == UserStatus.Online).Count();
            var botscount = guild.Users.Where((x) => x.IsBot).Count();
            var peoplecount = membercount - botscount;
 
            var allmemberid = await _ServerStats.GetServerStatAsync(guild.Id, "allmemebrsid");
            var memberid = await _ServerStats.GetServerStatAsync(guild.Id, "memebrsid");
            var botid = await _ServerStats.GetServerStatAsync(guild.Id, "botsid");
            var boostersid = await _ServerStats.GetServerStatAsync(guild.Id, "boostersid");
 
 
            string allmemberoldName = string.Empty;
            var allmemberChannel = guild.GetChannel(allmemberid);
           
            string allmemberName = allmemberChannel.Name;
            var pos = allmemberChannel.Position;
            string[] allmembernewName = allmemberName.Split(new[] { ':' }, 2);
            if (allmemberoldName != allmemberName)
            {
                await allmemberChannel.ModifyAsync((x) => x.Name = $"{allmembernewName[0]}: {membercount}");
                Console.WriteLine($"Server: {guild} \nServer ID: {server_id} \nChannel ID: {allmemberid} Changed Name to {allmemberName} \nPos: {pos}");
            }
            allmemberoldName = allmemberName;
 
            string memberoldName = string.Empty;
            var memberiChannel = guild.GetChannel(memberid);
 
            string memberName = memberiChannel.Name;
            string[] membernewName = memberName.Split(new[] { ':' }, 2);
            if (memberoldName != memberName)
            {
                await memberiChannel.ModifyAsync(x => x.Name = $"{membernewName[0]}: {peoplecount}");
                Console.WriteLine($"Server: {guild} \nServer ID: {server_id} \nChannel ID: {memberid} Changed Name to {memberName}");
            }
            memberoldName = memberName;
 
 
            string botoldName = string.Empty;
            var botChannel = guild.GetChannel(botid);
 
            string botName = botChannel.Name;
            string[] botnewName = botName.Split(new[] { ':' }, 2);
            if (botoldName != botName)
            {
                await botChannel.ModifyAsync(x => x.Name = $"{botnewName[0]}: {botscount}");
                Console.WriteLine($"Server: {guild} \nServer ID: {server_id} \nChannel ID: {botid} Changed Name to {botName}");
            }
            botoldName = botName;
 
     
            string boosteroldName = string.Empty;
            var boosterChannel = guild.GetChannel(boostersid);
            
            string boosterName = boosterChannel.Name;
            string[] boosternewName = boosterName.Split(new[] { ':' }, 2);
            if (boosteroldName != boosterName)
            {
                await boosterChannel.ModifyAsync(x => x.Name = $"{boosternewName[0]}: {boosterscount}");
                Console.WriteLine($"Server: {guild} \nServer ID: {server_id} \nChannel ID: {boostersid} Changed Name to {boosterName}");
            }
            oldName = boosterName;
        }
 
        public async Task RenameChannel(ulong channelId)
        {
            ulong ServerId = 0;
            if (channelId != 0)
            {
                ServerId = await _ServerStats.GetServerIdAsync(channelId);
 
                var guild = _client.GetGuild(ServerId);
 
                if(guild != null)
                {
                    var membercount = guild.MemberCount;
                    var boosterscount = guild.PremiumSubscriptionCount;
                    var onlinecount = guild.Users.Where((x) => x.Status == UserStatus.Online).Count();
                    var botscount = guild.Users.Where((x) => x.IsBot).Count();
                    var peoplecount = membercount - botscount;
 
                    var allmemberid = await _ServerStats.GetServerStatAsync(guild.Id, "allmemebrsid");
                    var memberid = await _ServerStats.GetServerStatAsync(guild.Id, "memebrsid");
                    var botid = await _ServerStats.GetServerStatAsync(guild.Id, "botsid");
                    var boostersid = await _ServerStats.GetServerStatAsync(guild.Id, "boostersid");
 
                    if (channelId == allmemberid)
                    {
                        if (!OnCoolDown.Contains(channelId))
                        {
                            var targetChannel = guild.GetChannel(channelId);
 
                            string targetName = targetChannel.Name;
                            string[] newName = targetName.Split(new[] { ':' }, 2);
                            if (oldName != targetName)
                            {
                                await targetChannel.ModifyAsync((x) => x.Name = $"{newName[0]}: {membercount}");
                                Console.WriteLine($"Server: {guild} \nServer ID: {ServerId} \nChannel ID: {channelId} Changed Name to {targetName}");
                            }
                            oldName = targetName;
                            await AddToCooldown(channelId);
                            await Task.Delay(600000);
                            await RemoveFromCooldown(channelId);
                        }
                    }
 
                    else if (channelId == memberid)
                    {
                        var targetChannel = guild.GetChannel(channelId);
 
                        string targetName = targetChannel.Name;
                        string[] newName = targetName.Split(new[] { ':' }, 2);
                        if (oldName != targetName)
                        {
                            await targetChannel.ModifyAsync(x => x.Name = $"{newName[0]}: {peoplecount}");
                            Console.WriteLine($"Server: {guild} \nServer ID: {ServerId} \nChannel ID: {channelId} Changed Name to {targetName}");
                        }
                        oldName = targetName;
                        await AddToCooldown(channelId);
                        await Task.Delay(600000);
                        await RemoveFromCooldown(channelId);
                    }
 
                    else if (channelId == botid)
                    {
                        var targetChannel = guild.GetChannel(channelId);
 
                        string targetName = targetChannel.Name;
                        string[] newName = targetName.Split(new[] { ':' }, 2);
                        if (oldName != targetName)
                        {
                            await targetChannel.ModifyAsync(x => x.Name = $"{newName[0]}: {botscount}");
                            Console.WriteLine($"Server: {guild} \nServer ID: {ServerId} \nChannel ID: {channelId} Changed Name to {targetName}");
                        }
                        oldName = targetName;
                        await AddToCooldown(channelId);
                        await Task.Delay(600000);
                        await RemoveFromCooldown(channelId);
                    }
 
                    else if (channelId == boostersid)
                    {
                        var targetChannel = guild.GetChannel(channelId);
 
                        string targetName = targetChannel.Name;
                        string[] newName = targetName.Split(new[] { ':' }, 2);
                        if (oldName != targetName)
                        {
                            await targetChannel.ModifyAsync(x => x.Name = $"{newName[0]}: {boosterscount}");
                            Console.WriteLine($"Server: {guild} \nServer ID: {ServerId} \nChannel ID: {channelId} Changed Name to {targetName}");
                        }
                        oldName = targetName;
                        await AddToCooldown(channelId);
                        await Task.Delay(600000);
                        await RemoveFromCooldown(channelId);
                    }
                }
            }
        }
 
        public Task AddToCooldown(ulong channelId)
        {
            OnCoolDown.Add(channelId);
            Console.WriteLine($"CID: {channelId} added to the cool down list");
            return Task.CompletedTask;
        }
 
        public Task RemoveFromCooldown(ulong channelId)
        {
            OnCoolDown.Remove(channelId);
            Console.WriteLine($"CID: {channelId} removed from the cool down list");
            return Task.CompletedTask;
        }
    }
}