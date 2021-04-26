using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ENBot.Common;
using ENBot.Services;

namespace ENBot.Modules
{
    public class Moderation : ModuleBase<SocketCommandContext>
    {
        //clear
        [Command("убрать")]
        [RequireUserPermission(GuildPermission.ManageMessages)]
        [RequireBotPermission(GuildPermission.ManageMessages)]
        public async Task Clear(int amount)
        {
            var messages = await Context.Channel.GetMessagesAsync(amount + 1).FlattenAsync();
            await (Context.Channel as SocketTextChannel).DeleteMessagesAsync(messages);

            var message = await Context.Channel.SendMessageAsync($"Да ты достал мусорить. Вот, посмотри сколько мусора мне пришлось за тобой убрать: {messages.Count()}!");
            await Task.Delay(5000);
            await message.DeleteAsync();
        }

        [Command("мут")]
        [RequireUserPermission(GuildPermission.DeafenMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Mute(SocketGuildUser user, int minutes, [Remainder]string reason = null)
        {
            if (user.Hierarchy > Context.Guild.CurrentUser.Hierarchy)
            {
                await Context.Channel.CreateErrorEmbed("Invalid user", "That user has a higher position than the bot.");
                return;
            }

            var role = (Context.Guild as IGuild).Roles.FirstOrDefault(x => x.Name == "Мут");
            if (role == null)
                role = await Context.Guild.CreateRoleAsync("Мут", new GuildPermissions(sendMessages: false, useVoiceActivation: false), null, false, null);

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await Context.Channel.CreateErrorEmbed(":shushing_face:Модерация:shushing_face:Invalid permissions", "The muted role has a higher position than the bot.");
                return;
            }

            if (user.Roles.Contains(role))
            {
                await Context.Channel.CreateErrorEmbed("Already muted", "That user is already muted.");
                return;
            }

            await role.ModifyAsync(x => x.Position = Context.Guild.CurrentUser.Hierarchy);

            foreach (var channel in Context.Guild.TextChannels)
            {
                if (!channel.GetPermissionOverwrite(role).HasValue || channel.GetPermissionOverwrite(role).Value.SendMessages == PermValue.Allow)
                {
                    await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(sendMessages: PermValue.Deny));
                }
            }

            /*foreach (var channel in Context.Guild.VoiceChannels)
            {
                if (!channel.GetPermissionOverwrite(role).HasValue || channel.GetPermissionOverwrite(role).Value.Speak == PermValue.Allow)
                {
                    await channel.AddPermissionOverwriteAsync(role, new OverwritePermissions(useVoiceActivation: PermValue.Deny));
                }
            }*/

            CommandHandler.Mutes.Add(new Mute { Guild = Context.Guild, User = user, End = DateTime.Now + TimeSpan.FromMinutes(minutes), Role = role });
            await user.AddRoleAsync(role);
            await Context.Channel.CreateSuccessfullyEmbed($"Muted {user.Username}", $"Duration: {minutes} minutes\nReason: {reason ?? "None"}");
        }

        [Command("размут")]
        [RequireUserPermission(GuildPermission.KickMembers)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Unmute(SocketGuildUser user)
        {
            var role = (Context.Guild as IGuild).Roles.FirstOrDefault(x => x.Name == "Muted");
            if (role == null)
            {
                await Context.Channel.CreateErrorEmbed("Not muted", "This person has not been muted yet.");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await Context.Channel.CreateErrorEmbed("Invalid permissions", "The muted role has a higher position than the bot.");
                return;
            }

            if (!user.Roles.Contains(role))
            {
                await Context.Channel.CreateErrorEmbed("Not muted", "This person has not been muted yet.");
                return;
            }

            await user.RemoveRoleAsync(role);
            await Context.Channel.CreateSuccessfullyEmbed($"Unmuted {user.Username}", $"Successfully unmuted the user.");
        }

        [Command("слоумод")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        [RequireBotPermission(GuildPermission.ManageChannels)]
        public async Task Slowmode(int interval = 0)
        {
            await (Context.Channel as SocketTextChannel).ModifyAsync(x => x.SlowModeInterval = interval);
            await Context.Channel.CreateSuccessfullyEmbed(":shushing_face:Модерация:shushing_face:", $"Медленный режим для этого канала был установлен на {interval} секунд!");
        }
    }
}
