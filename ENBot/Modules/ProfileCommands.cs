using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using ENBot.Services;
using Infrastructure.LevelSystem;

namespace ENBot.Modules
{
    public class ProfileCommands : ModuleBase<SocketCommandContext>
    {
        /*private readonly IProfileService _profileService;

        public ProfileCommands(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx)
        {
            await GetProfileToDisplayAsync(ctx, ctx.User.Id);
        }

        [Command("profile")]
        public async Task Profile(CommandContext ctx, SocketGuildUser user)
        {
            await GetProfileToDisplayAsync(ctx, user.Id);
        }

        private async Task GetProfileToDisplayAsync(CommandContext ctx, ulong memberId)
        {
            Profile profile = await _profileService.GetOrCreateProfileAsync(memberId, ctx.Guild.Id).ConfigureAwait(false);

            //SocketGuildUser member = (SocketGuildUser)ctx.Guild.GetUserAsync(profile.DiscordId, CacheMode.AllowDownload) as Discord.IGuildUser;

            var profileEmbed = new EmbedBuilder
            {
                //Title = $"{member.Us}'s Profile",
                //ThumbnailUrl = member.AvatarUrl
            };

            profileEmbed.AddField("Xp", profile.Xp.ToString());
            var build = profileEmbed.Build();
            await ctx.Channel.SendMessageAsync(null, false, build).ConfigureAwait(false);
        }*/
    }
}
