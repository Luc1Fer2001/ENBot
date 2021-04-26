using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class Servers
    {
        private readonly ENBotContext _context;

        public Servers(ENBotContext context)
        {
            _context = context;
        }

        public async Task ModifyGuildPrefix(ulong id, string prefix)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
                _context.Add(new Server { Id = id, Prefix = prefix });
            else
                server.Prefix = prefix;

            await _context.SaveChangesAsync();
        }

        public async Task<string> GetGuildPrefix(ulong id)
        {
            var prefix = await _context.Servers
                .Where(x => x.Id == id)
                .Select(x => x.Prefix)
                .FirstOrDefaultAsync();

            return await Task.FromResult(prefix);
        }

        public async Task ModifyWelcomeAsync(ulong id, ulong channelId)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if(server == null)
            {
                _context.Add(new Server { Id = id, Welcome = channelId });
            }
            else
            {
                server.Welcome = channelId;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearWelcomeAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            server.Welcome = 0;
            await _context.SaveChangesAsync();
        }

        public async Task<ulong> GetWelcomeAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.Welcome);
        }

        public async Task ModifyInvaitAsync(ulong id, bool invait)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id, InvaitDiscord = invait });
            }
            else
            {
                server.InvaitDiscord = invait;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearInvaitAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            server.InvaitDiscord = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> GetInvaitAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.InvaitDiscord);
        }

        public async Task ModifyAutoCodeRedactAsync(ulong id, bool autoCode)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id, AutoCodeRedact = autoCode });
            }
            else
            {
                server.AutoCodeRedact = autoCode;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearAutoCodeRedactAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id });
            }

            server.AutoCodeRedact = false;
            await _context.SaveChangesAsync();
        }

        public async Task<bool> GetAutoCodeRedactAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id });
                await _context.SaveChangesAsync();
                return false;
            }

            return await Task.FromResult(server.AutoCodeRedact);
        }

        public async Task ModifyBackgroundAsync(ulong id, string url)
        {
            var server = await _context.Servers
                .FindAsync(id);

            if (server == null)
            {
                _context.Add(new Server { Id = id, Background = url });
            }
            else
            {
                server.Background = url;
            }

            await _context.SaveChangesAsync();
        }

        public async Task ClearBackgroundAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            server.Background = null;
            await _context.SaveChangesAsync();
        }

        public async Task<string> GetBackgroundAsync(ulong id)
        {
            var server = await _context.Servers
                .FindAsync(id);

            return await Task.FromResult(server.Background);
        }
    }
}
