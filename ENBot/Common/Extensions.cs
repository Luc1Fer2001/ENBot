using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace ENBot.Common
{
    public static class Extensions
    {
        public static async Task<IMessage> CreateBasicEmbed(this ISocketMessageChannel channel, string title, string description, Color color)
        {
            var embed = await Task.Run(() => (new EmbedBuilder()
                .WithTitle(title)
                .WithDescription(description)
                .WithColor(color)
                .WithCurrentTimestamp()
                .Build()));
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> CreateErrorEmbed(this ISocketMessageChannel channel, string modulError, string error)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($":no_entry: Упс... Ошибка(")
                .WithDescription($"**Модуль с которым произошла ошибка:\n{modulError}**\n\n**Подробности ошибки**: \n{error}")
                .WithColor(new Color(255, 0, 0))
                .WithCurrentTimestamp()
                .Build());
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }

        public static async Task<IMessage> CreateSuccessfullyEmbed(this ISocketMessageChannel channel, string moduluSccessfully, string sccessfully)
        {
            var embed = await Task.Run(() => new EmbedBuilder()
                .WithTitle($":white_check_mark: Успешно!)")
                .WithDescription($"**Сообщение из модуля:\n{moduluSccessfully}**\n\n**Содержание сообщения:** \n\n{sccessfully}")
                .WithColor(new Color(30, 230, 30))
                .WithCurrentTimestamp()
                .Build());
            var message = await channel.SendMessageAsync(embed: embed);
            return message;
        }
    }
}
