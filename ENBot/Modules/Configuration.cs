using Discord;
using Discord.Commands;
using ENBot.Common;
using ENBot.Utilities;
using Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ENBot.Modules
{
    public class Configuration : ModuleBase<SocketCommandContext>
    {
        private readonly RanksHelper _ranksHelper;
        private readonly Servers _servers;
        private readonly Ranks _ranks;
        private readonly AutoRolesHelper _autoRolesHelper;
        private readonly AutoRoles _autoRoles;

        public Configuration(RanksHelper ranksHelper, Servers servers, Ranks ranks, AutoRolesHelper autoRolesHelper, AutoRoles autoRoles)
        {
            _ranksHelper = ranksHelper;
            _servers = servers;
            _ranks = ranks;
            _autoRolesHelper = autoRolesHelper;
            _autoRoles = autoRoles;
        }

        [Command("префикс")]
        [RequireUserPermission(Discord.GuildPermission.Administrator)]
        public async Task Prefix(string prefix = null)
        {
            if (prefix == null)
            {
                var guildPrefix = await _servers.GetGuildPrefix(Context.Guild.Id) ?? "*";
                await ReplyAsync($"Текущий префикс бота `{guildPrefix}`.");
                return;
            }

            if (prefix.Length > 8)
            {
                await ReplyAsync("Новый префикс слишком длинный, укажите префикс меньше 8 символов");
                return;
            }

            await _servers.ModifyGuildPrefix(Context.Guild.Id, prefix);
            var builder = new EmbedBuilder()
                    .WithThumbnailUrl(Context.User.GetAvatarUrl() ?? Context.User.GetDefaultAvatarUrl())// середнє зображення зправа
                    .WithTitle($"Настройки бота **ENBot**")
                    .WithDescription($"Пользователь: **{Context.User.Username}** изменил префикс бота.")// опис, типу текст в середині
                    .WithColor(new Color(32, 255, 0))// колір лінії зліва
                    .AddField($"Новый префикс:", $"{prefix}", true)
                    .WithCurrentTimestamp();// час коли відправлено повідомлення
            var ember = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, ember);
        }

        [Command("ранги", RunMode = RunMode.Async)]
        public async Task Ranks()
        {
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);
            if(ranks.Count == 0)
            {
                await ReplyAsync("На этом сервере ещё нет ни одной роли");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "В этом сообщении перечислены все доступные роли.\n Чтобы добавить роль, вы можете использовать название или ID роли.";
            foreach(var rank in ranks)
            {
                description += $"\n{rank.Mention} ({rank.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("ранги+")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddRank([Remainder]string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if(role == null)
            {
                await ReplyAsync("Такой роли не существует!");
                return;
            }

            if(role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Ранг этой роли выше чем у роли бота!");
                return;
            }

            if(ranks.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Эта роль уже есть среди рангов");
                return;
            }

            await _ranks.AddRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Роль: {role.Mention} была успешно добавлена к рангам");
        }

        [Command("ранги-", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelRank([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var ranks = await _ranksHelper.GetRanksAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Такой роли не существует!");
                return;
            }

            if (ranks.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Такой роли нет среди рангов!");
                return;
            }

            await _ranks.RemoveRankAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Роль: {role.Mention} была удалена из рангов");
        }


        [Command("автороль", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoRoles()
        {
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);
            if (autoRoles.Count == 0)
            {
                await ReplyAsync("На этом сервере ещё нет ни одной автороли");
                return;
            }

            await Context.Channel.TriggerTypingAsync();

            string description = "В этом сообщении перечислены все доступные автороли.\n Чтобы добавить или удалить автороль, вы можете использовать название или ID роли.";
            foreach (var autoRole in autoRoles)
            {
                description += $"\n{autoRole.Mention} ({autoRole.Id})";
            }

            await ReplyAsync(description);
        }

        [Command("автороль+")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task AddAutoRoles([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Такой роли не существует!");
                return;
            }

            if (role.Position > Context.Guild.CurrentUser.Hierarchy)
            {
                await ReplyAsync("Ранг этой роли выше чем у роли бота!");
                return;
            }

            if (autoRoles.Any(x => x.Id == role.Id))
            {
                await ReplyAsync("Эта роль уже есть среди авторолей");
                return;
            }

            await _autoRoles.AddAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Роль: {role.Mention} была успешно добавлена к авторолям");
        }

        [Command("автороль-", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task DelAutoRoles([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var autoRoles = await _autoRolesHelper.GetAutoRolesAsync(Context.Guild);

            var role = Context.Guild.Roles.FirstOrDefault(x => string.Equals(x.Name, name, StringComparison.CurrentCultureIgnoreCase));
            if (role == null)
            {
                await ReplyAsync("Такой роли не существует!");
                return;
            }

            if (autoRoles.Any(x => x.Id != role.Id))
            {
                await ReplyAsync("Такой роли нет среди авторолей!");
                return;
            }

            await _autoRoles.RemoveAutoRoleAsync(Context.Guild.Id, role.Id);
            await ReplyAsync($"Роль: {role.Mention} была удалена из авторолей");
        }

        [Command("велком")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Welcome(string option = null, string value = null)
        {
            if (option == null && value == null)
            {
                var fetchedChannelId = await _servers.GetWelcomeAsync(Context.Guild.Id);
                if (fetchedChannelId == 0)
                {
                    await ReplyAsync("Приветственный канал еще не настроен!");
                    return;
                }

                var fetchedChannel = Context.Guild.GetTextChannel(fetchedChannelId);
                if (fetchedChannel == null)
                {
                    await ReplyAsync("Приветственный канал еще не настроен!");
                    await _servers.ClearWelcomeAsync(Context.Guild.Id);
                    return;
                }

                
                var fetchedBackground = await _servers.GetBackgroundAsync(Context.Guild.Id);

                if (fetchedBackground != null)
                    await ReplyAsync($"Канал, используемый для модуля приветствия: {fetchedChannel.Mention}.\nФон установлен на: {fetchedBackground}.");
                else
                    await ReplyAsync($"Канал, используемый для модуля приветствия: {fetchedChannel.Mention}.");

                return;
            }

            if (option == "канал" && value != null)
            {
                if (!MentionUtils.TryParseChannel(value, out ulong parsedId))
                {
                    await ReplyAsync("Пожалуйста, укажите действующий канал!");
                    return;
                }

                var parsedChannel = Context.Guild.GetTextChannel(parsedId);
                if (parsedChannel == null)
                {
                    await ReplyAsync("Пожалуйста, укажите действующий канал!");
                    return;
                }

                await _servers.ModifyWelcomeAsync(Context.Guild.Id, parsedId);
                await ReplyAsync($"Канал приветствия успешно изменен на {parsedChannel.Mention}.");
                return;
            }

            if (option == "фон" && value != null)
            {
                if (value == "очистить")
                {
                    await _servers.ClearBackgroundAsync(Context.Guild.Id);
                    await ReplyAsync("Фон для этого сервера успешно очищен.");
                    return;
                }

                await _servers.ModifyBackgroundAsync(Context.Guild.Id, value);
                await ReplyAsync($"Фон успешно изменен на {value}.");
                return;
            }

            if (option == "очистить" && value == null)
            {
                await _servers.ClearWelcomeAsync(Context.Guild.Id);
                await ReplyAsync("Модуль приветствия отключен.");
                return;
            }

            await ReplyAsync("Вы неправильно использовали эту команду. Напишите *помощь что бы узнать как использовать эту команду ");
        }

        [Command("инвайт")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AntiInvait(string option = null)
        {
            if (option == null)
            {
                var antiInvait = await _servers.GetInvaitAsync(Context.Guild.Id);
                if(antiInvait == false)
                {
                    await Context.Channel.CreateSuccessfullyEmbed(":shushing_face:Авто-Модерация:shushing_face:", "Блокировка ссылок приглашения Дискорд отключенна!");
                    return;
                }
                else
                {
                    await Context.Channel.CreateSuccessfullyEmbed(":shushing_face:Авто-Модерация:shushing_face:", "Блокировка ссылок приглашения Дискорд включенна!");
                    return;
                }
            }
            if (option == "включить")
            {
                await _servers.ModifyInvaitAsync(Context.Guild.Id, true);
                await Context.Channel.CreateSuccessfullyEmbed(":shushing_face:Авто-Модерация:shushing_face:", "Блокировка ссылок приглашения Дискорд успешно включенна!");
                return;
            }

            if (option == "отключить")
            {
                await _servers.ClearInvaitAsync(Context.Guild.Id);
                await Context.Channel.CreateSuccessfullyEmbed(":shushing_face:Авто-Модерация:shushing_face:", "Блокировка ссылок приглашения Дискорд успешно отключенна!");
                return;
            }

            await ReplyAsync("Вы неправильно использовали эту команду. Напишите *помощь что бы узнать как использовать эту команду ");
        }

        [Command("автокод")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AutoCodeRedact(string option = null)
        {
            if (option == null)
            {
                var autoCodeRedact = await _servers.GetAutoCodeRedactAsync(Context.Guild.Id);
                if (autoCodeRedact == false)
                {
                    await Context.Channel.CreateSuccessfullyEmbed(":man_technologist:Авто-Модерация Кода:man_technologist:", "Функция Авто-формат кода отключенна!");
                    return;
                }
                else
                {
                    await Context.Channel.CreateSuccessfullyEmbed(":man_technologist:Авто-Модерация Кода:man_technologist:", "Функция Авто-формат кода включенна!");
                    return;
                }
            }
            if (option == "включить")
            {
                await _servers.ModifyAutoCodeRedactAsync(Context.Guild.Id, true);
                await Context.Channel.CreateSuccessfullyEmbed(":man_technologist:Авто-Модерация Кода:man_technologist:", "Функция Авто-формат кода успешно включенна!");
                return;
            }

            if (option == "отключить")
            {
                await _servers.ClearAutoCodeRedactAsync(Context.Guild.Id);
                await Context.Channel.CreateSuccessfullyEmbed(":man_technologist:Авто-Модерация Кода:man_technologist:", "Функция Авто-формат кода успешно отключенна!");
                return;
            }

            await Context.Channel.CreateErrorEmbed(":man_technologist:Авто-Модерация Кода:man_technologist:", "Вы неправильно использовали эту команду. Напишите *помощь что бы узнать как использовать эту команду ");
        }
    }
}
