using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace ENBot.Modules
{
    public class Fun : ModuleBase<SocketCommandContext>
    {
        [Command("вопрос")]
        public async Task EchoAsync([Remainder] string text)
        {
            string[] textik = new string[] { "Возможно да", "Естественно!", "Я устал отвечать на твои вопросы", "Да!💪", "Надеюсь да🤞", "Думаю ты сам знаешь ответ на этот вопрос😏😏😏", "Сам подумай😛", "Не хочу отвечать😝😝😝", "Нет!", "С такими вопросами обращайся к гуглу", "Ох, даже незнаю отвечать тебе или нет", "Сам незнаю🤔", "50/50🤏", "+-", "Спроси когото другого", "Возможно", "Возможно нет", "Да", "Нет", "Я лучше промолчу", "На этот вопрос тебе может ответить любой в чате)", "Однозначно!", "Спроси когото другого, не видишь я занят", "Я хз🤔", "Это тебя надо спросить!" };
            Random random = new Random();
            int count = random.Next(0, textik.Length);
            await Context.Channel.SendMessageAsync($"{textik[count]}" + "\n **Мой ответ не вправе изменить / влиять на правила сервера**");
        }

        [Command("мемчик")]
        [Alias("reddit")]
        public async Task Mem(string subreddit = null)
        {
            var client = new HttpClient();
            var result = await client.GetStringAsync($"https://reddit.com/r/{subreddit ?? "memes"}/random.json?limit=1");
            if (!result.StartsWith("["))
            {
                await Context.Channel.SendMessageAsync("is reddit not find");
                return;
            }
            JArray arr = JArray.Parse(result);
            JObject post = JObject.Parse(arr[0]["data"]["children"][0]["data"].ToString());

            if(post["over_18"].ToString() == "True" && !(Context.Channel as ITextChannel).IsNsfw)
            {
                await ReplyAsync("The subreddit contains NSFW content, while this is a SFW channel.");
                return;
            }

            var builder = new EmbedBuilder()
                .WithImageUrl(post["url"].ToString())
                .WithColor(new Color(12, 125, 68))
                .WithTitle(post["title"].ToString())
                .WithUrl("https://reddit.com" + post["permalink"].ToString())
                .WithFooter($"🗨 {post["num_comments"]} ⬆️ {post["ups"]}");
            var ember = builder.Build();
            await Context.Channel.SendMessageAsync(null, false, ember);
        }
    }
}
