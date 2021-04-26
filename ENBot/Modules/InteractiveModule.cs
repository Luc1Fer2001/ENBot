using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Addons.Interactive;
using Discord.Commands;

namespace ENBot.Modules
{
    public class InteractiveModule : InteractiveBase
    {
        // DeleteAfterAsync will send a message and asynchronously delete it after the timeout has popped
        // This method will not block.
        [Command("delete")]
        public async Task<RuntimeResult> Test_DeleteAfterAsync()
        {
            await ReplyAndDeleteAsync("I will be delete in 10 seconds", timeout: new TimeSpan(0, 0, 10));
            return Ok();
        }

        /*// NextMessageAsync will wait for the next message to come in over the gateway, given certain criteria
        // By default, this will be limited to messages from the source user in the source channel
        // This method will block the gateway, so it should be ran in async mode.
        [Command("next", RunMode = RunMode.Async)]
        public async Task Test_NextMessageAsync()
        {
            await ReplyAsync("What is 2+2?");
            var response = await NextMessageAsync(true, true, new TimeSpan(0, 0, 10));
            if (response != null)
            {
                if(response.Content == "4")
                {
                    await ReplyAsync($"Correct! The anwer was 4.");
                }
                else
                {
                    await ReplyAsync("Wrong! The anwer was 4.");
                }
            }
            else
                await ReplyAsync("You did not reply before the timeout");
        }*/

        // PagedReplyAsync will send a paginated message to the channel
        // You can customize the paginator by creating a PaginatedMessage object
        // You can customize the criteria for the paginator as well, which defaults to restricting to the source user
        // This method will not block.
        [Command("помощь")]
        public async Task Test_Paginator()
        {
            var pages = new[]
            {
                "**Основные команды**\n\n" + 
                "`*помощь` - Посмотреть все команды бота\n" +
                "`*префикс` - Посмотреть префикс бота\n" +
                "`*ранги` - Посмотреть доступные ранги\n" +
                "`*ранг` `назва роли` - Взять роль\n" +
                "`*карточка` - Узнать информацию о себе\n" +
                "`*карточка` `@Пользователь` - Узнать информацию о `Пользователь`",

                "**Полезное Программистам**\n\n" +
                "`*автокод`  - Узнать статус функции Авто-формата кода\n" +
                "`*автокод` `включить` - Включить функцию Авто-формата кода\n" +
                "`*автокод` `отключить` - Отключить функцию Авто-формата кода\n" +
                "Авто-формат кода опридиляет является ли ваше сообщение частью кода, если да, то бот автоматически поместит его в синтаксис для удобства чтения кода",

                "**Прикольчики**\n\n" +
                "`*вопрос` `ваш вопрос` - Можно задать вопрос боту, на который он рандомно ответит да или нет)(попробуй)\n" +
                "`*мемчик` - скинет мемчик из редита, жаль на инглише(",


                "**Команды для музыки**\n\n" +
                "`*играть` `название/ссылка из ютуба` - найти и играть песню\n" +
                "`*скип` - пропустить песню\n" +
                "`*пауза` - поставить/снять с паузи\n" +
                "`*плейлист` - посмотреть плейлист песен(очередь)\n" +
                "`*стоп` - остановить музыку, и очистить плейлист\n" +
                "`*дисконект` - отключить бота от голосового чата\n",


                "**Модерация**\n\n" +
                "`*убрать` `количество` - очистить чат от мусора\n",


                "**Админ команды**\n\n" +
                "`*префикс` `желаемый префикс` - изменить префикс бота\n" +
                "`*ранги+` `назва роли` - добавить роль в список ролей\n" +
                "`*ранги-` `назва роли` - убрать роль из списка ролей\n" +
                "`*автороль+` `назва роли` - добавить роль в список авторолей\n" +
                "`*автороль-` `назва роли` - добавить роль в список авторолей\n",


                "**Команды приветствия**\n\n" +
                "`*велком` - посмотреть статус модуля приветствия\n" +
                "`*велком` `канал` `#канал` - установить канал для приветствия\n" +
                "`*велком` `очистить` - отключить модуль приветствия\n" +
                "`*велком` `фон` `ссылка на изображение` - установить фон для приветственного сообщения\n" +
                "`*велком` `фон` `очистить` - востанновить стандартный фон\n" +
                "`*велком` - посмотреть модуль приветствия\n" +
                "`*изображение` `@участник` - посмотреть пример приветственного сообщения\n",


                "**Команды разработчика бота**\n\n" + 
                "`*статусбота` `тип` `статус` - изменить статус бота\n" +
                "`*бот` - посмотреть инфу о обте\n" +
                "`*создать` - пока-что в разработке(\n" +
                "`*живой?` - проверить живой ли бот(если бот ответил, значит он работает)\n"
            };

            PaginatedMessage paginatedMessage = new PaginatedMessage()
            {
                Pages = pages,
                Options = new PaginatedAppearanceOptions()
                {
                    InformationText = "Здесь ты можешь посмотреть команды бота) и красиво переключатся между страницами",
                    //First = new Emoji("❓"),
                    //Back = new Emoji("❓"),
                    //Next = new Emoji("❓"),
                    //Last = new Emoji("❓"),
                    Stop = new Emoji("🚫"),
                    //Jump = new Emoji("❓"),
                    Info = new Emoji("❓")
                },
                Color = new Color(0, 125, 69),
                Title = $"Команды {Context.Client.CurrentUser.Username}"
            };
            await PagedReplyAsync(paginatedMessage);
        }
    }
}
