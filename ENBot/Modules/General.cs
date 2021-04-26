using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ENBot;
using ENBot.Services;
using ENBot.Common;

namespace ENBot.Modules
{
    public class General : ModuleBase<SocketCommandContext>
    {
        //info ember
        [Command("карточка")]
        public async Task Info(SocketGuildUser user = null)
        {
            if (user == null)
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())// середнє зображення зправа
                    .WithDescription("Здесь вы можете увидеть свою карточку профиля")// опис, типу текст в середині
                    .WithColor(new Color(33, 52, 250))// колір лінії зліва
                    .AddField("ID участника: ", Context.User.Id, true)// id користувача
                    .AddField("Хэштег участника: ", Context.User.Discriminator, true)// хештег номер користувача
                    .AddField("Дата создания\nаккаунта: ", Context.User.CreatedAt.ToString("dd/MM/yyyy"), true)// коли створений акк користувача
                    .AddField("На сервере с : ", (Context.User as SocketGuildUser).JoinedAt.Value.ToString("dd/MM/yyyy"), true)// коли приєднався на сервер
                    .AddField("Роли: ", string.Join(" ", (Context.User as SocketGuildUser).Roles.Select(x => x.Mention)))// ролі користувача
                    .WithCurrentTimestamp();// час коли відправлено повідомлення
                var ember = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, ember);
            }
            else
            {
                var builder = new EmbedBuilder()
                    .WithThumbnailUrl(user.GetAvatarUrl() ?? user.GetDefaultAvatarUrl())// середнє зображення зправа
                    .WithDescription($"Здесь отображается информация о профиле пользователя: {user.Username}")// опис, типу текст в середині
                    .WithColor(new Color(33, 250, 20))// колір лінії зліва
                    .AddField("ID участника: ", user.Id, true)// id користувача
                    .AddField("Хэштег участника: ", user.Discriminator, true)// хештег номер користувача
                    .AddField("Дата создания\nаккаунта: ", user.CreatedAt.ToString("dd/MM/yyyy"), true)// коли створений акк користувача
                    .AddField("На сервере с : ", user.JoinedAt.Value.ToString("dd/MM/yyyy"), true)// коли приєднався на сервер
                    .AddField("Роли: ", string.Join(" ", user.Roles.Select(x => x.Mention)))// ролі користувача
                    .WithCurrentTimestamp();// час коли відправлено повідомлення
                var ember = builder.Build();
                await Context.Channel.SendMessageAsync(null, false, ember);
            }
        }
        //server info
        [Command("сервер")]
        public async Task Server()
        {
            var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription("Здесь вы увидете всю, или почти всю информацию о сервере!)")// опис
                .WithTitle($"Информация сервера: {Context.Guild.Name}")// назва сервера
                .WithColor(new Color(103, 43, 86))
                .AddField("Дата создания\nсервера: ", Context.Guild.CreatedAt.ToString("dd/MM/yyyy"), true)// коли створений сервер
                .AddField("Количество\nучастников: ", "👽" + (Context.Guild as SocketGuild).MemberCount, true)// кількість участників
                .AddField("Количество\nучастников\nонлайн: ", "👽" + (Context.Guild as SocketGuild).Users.Where(x => x.Status != UserStatus.Offline).Count(), true);// онлайн
            var ember = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, ember);
        }

        [Command("репорт")]
        public async Task Report( string options, [Remainder] string textMessage)
        {
            ulong ownerID = 561975249147527313;
            if (options == "проблема")
            {
                var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription($"Вам новое сообщение: {textMessage}")
                .WithTitle($"Сообщение из сервера: {Context.Guild.Name}")
                .WithColor(new Color(255, 0, 0))
                .AddField("Тип: ", $"{options}", true)
                .AddField("Автор сообщения: ", Context.Message.Author, true);
                var ember = builder.Build();
                await Context.Client.GetUser(ownerID).SendMessageAsync(null, false, ember);
            }
            else if(options == "предложение")
            {
                var builder = new EmbedBuilder()
                .WithThumbnailUrl(Context.Guild.IconUrl)
                .WithDescription($"Вам новое сообщение: {textMessage}")
                .WithTitle($"Сообщение из сервера: {Context.Guild.Name}")
                .WithColor(new Color(0, 255, 0))
                .AddField("Тип: ", $"{options}", true)
                .AddField("Автор сообщения: ", Context.Message.Author, true);
                var ember = builder.Build();
                await Context.Client.GetUser(ownerID).SendMessageAsync(null, false, ember);
            }
            else if(options == null)
            {
                await Context.Channel.CreateErrorEmbed(":loudspeaker:Связь с администрацией:loudspeaker:", "Вы не указали тип обращения, например `проблема` или `предложение`");
            }
        }

    }
}
