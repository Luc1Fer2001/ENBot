using System.Threading.Tasks;
using Infrastructure.LevelSystem;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ENBot.Services
{
    public interface IProfileService
    {
        Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong serverId);
    }
    public class ProfileService : IProfileService
    {
        private readonly ENBotContext _context;

        public ProfileService(ENBotContext context)
        {
            _context = context;
        }

        public async Task<Profile> GetOrCreateProfileAsync(ulong discordId, ulong serverId)
        {
            var profile = await _context.Profiles.AsQueryable()
                .Where(x => x.ServerId == serverId)
                .FirstOrDefaultAsync(x => x.DiscordId == discordId).ConfigureAwait(false);

            if (profile != null)
            { 
                return profile; 
            }

            profile = new Profile
            {
                DiscordId = discordId,
                ServerId = serverId
            };

            _context.Add(profile);

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return profile;
        }
    }
}
