using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using SlimeBotC.Handlers.Dialogue;
using SlimeBotC.Handlers.Dialogue.Steps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SlimeBotC.Commands
{
    public class GeneralCommands : BaseCommandModule
    {
        // This tag is what the actual user puts in chat
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("add")]
        [Description("Adds two numbers together")]
        public async Task Add(CommandContext ctx, [Description("First number")] int numOne = 0, [Description("Second number")] int numTwo = 0)
        {

            await ctx.Channel.SendMessageAsync((numOne + numTwo).ToString())
                .ConfigureAwait(false);
        }

        [Command("gm")]
        [Aliases("goodmorning")]
        [Description("Sends a random good morning message")]
        public async Task GoodMorning(CommandContext ctx)
        {
            string[] messages = { "Good morning", "Top of the mornin'", "Coffee time, ammiright", "Morning", "It may or may not be a good morning for me", "Rise and shine" };
            Random index = new Random();
            await ctx.Channel.SendMessageAsync(messages[index.Next(0, messages.Length)] + " " + ctx.Member.DisplayName).ConfigureAwait(false);
        }

        [Command("country")]
        [Description("It might be dated, but it's one hell of a song")]
        public async Task CountryRoads(CommandContext ctx, [Description("The best type of road")] string roads)
        {
            if (roads.ToLower() == "roads")
            {
                await ctx.Message.RespondAsync("Take me home!").ConfigureAwait(false);
                //Add reaction here
                //await ctx.Message.CreateReactionAsync("diamond");
            }
            else
            {
                await ctx.Channel.SendMessageAsync("If only you knew the song").ConfigureAwait(false);
            }
        }

        [Command("pp")]
        [Description("Whats your pp size?")]
        public async Task PP(CommandContext ctx)
        {
            string[] messages = { "Big PP!", "little pp" };
            Random index = new Random();
            await ctx.Channel.SendMessageAsync(ctx.Member.DisplayName + " has a " + messages[index.Next(0, messages.Length)]).ConfigureAwait(false);
        }

        [Command("avatar")]
        [Description("Returns the avatar photo of any mentioned user")]
        public async Task Avatar(CommandContext ctx, DiscordMember? user = null)
        {
            var memberMentioned = (user == null) ? ctx.Member.AvatarUrl : user.AvatarUrl;
            await ctx.Channel.SendMessageAsync(memberMentioned).ConfigureAwait(false);
        }

        [Command("roll")]
        [Description("Roll the dice and see what you get")]
        public async Task Roll(CommandContext ctx)
        {
            Random random = new Random();
            await ctx.Channel.SendMessageAsync("You rolled a : " + random.Next(1, 7)).ConfigureAwait(false);
        }

        [Command("roulette")]
        [Description("The only fun game to come from Russia")]
        public async Task Roulette(CommandContext ctx)
        {
            var random = new Random();
            var msg = (random.Next(0, 8) < 6) ? "You survive this time" : "BANG!";

            await ctx.Channel.SendMessageAsync(msg).ConfigureAwait(false);
        }

        [Command("trump")]
        [Description("Hear some amazing quotes")]
        public async Task Trump(CommandContext ctx)
        {
            var random = new Random();
            string[] quotes = {"\"I have a great relationship with the blacks\"", "\"Nobody builds walls better then me, believe me\"", "\"The beauty of me is that I'm very rich\"", "\"It's freezing and snowing in New York--we need global warming!\"",
                            "\"I beat China all the time. All the time.\"", "\"I just start kissing them. It’s like a magnet. Just kiss. I don’t even wait.\"", "\"You know, it really doesn't matter what the media write, as long as you've got a young and beautiful piece of ass.\"",
                        "\"I've said if Ivanka weren't my daughter, perhaps I'd be dating her.\"", "\"We have a 5 billion dollar website. I have so many websites. I have them all over the place.\"", "\"All of the women on The Apprentice flirted with me--consciously or unconsciously. That's to be expected.\"",
                    "\"My Twitter has become so powerful that I can actually make my enemies tell the truth.\"", "\"I'm their worst nightmare.\"", "\"I fully understand why her former husband left her for a man--he made a good decision.\"", "\"My fingers are long and beautiful, as, it has been well documented, are various other parts of my body.\"",
                "\"I did try and fuck her... I moved on her like a bitch, but I couldn’t get there. And she was married.\"", "\"I Have never seen a thin person drinking Diet Coke.\""};

            var embed = new DiscordEmbedBuilder
            {
                Description = "- Donny Boy",
                Title = quotes[random.Next(0, quotes.Length)],
                Color = DiscordColor.Yellow
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
        }

        [Command("hug")]
        [Description("Send a lovely dm to your pal")]
        public async Task Hug(CommandContext ctx, [Description("Everyone you want to hug")] params DiscordMember[] users)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "You have been sent a hug",
                Description = $"Courtesy of {ctx.Message.Author.Mention}",
                Color = DiscordColor.HotPink,
            };

            foreach (var test in users)
            {
                await test.CreateDmChannelAsync().ConfigureAwait(false);
                await test.SendMessageAsync(embed: embed).ConfigureAwait(false);
            }
        }
        
        /*
        [Command("image")]
        [Description("Provides a random tenor gif")]
        public async Task Image(CommandContext ctx, string query)
        {
            await ctx.Channel.SendMessageAsync("https://cdn.vox-cdn.com/thumbor/x5bPoUvbJhKWBIxX7LV2j_Bpi_8=/0x146:2040x1214/fit-in/1200x630/cdn.vox-cdn.com/uploads/chorus_asset/file/19287010/acastro_191014_1777_google_pixel_0005.0.jpg").ConfigureAwait(false);
        }
        */
        

        [Command("test")]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new StringStep("Enter something interesting", null);
            var rejectStep = new StringStep("Well that's no fun", null);

            string input = string.Empty;
            inputStep.OnValidResult += (result) =>
            {
                input = result;

                if (result == "no")
                {
                    inputStep.SetNextStep(rejectStep);
                }
            };

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(ctx.Client, userChannel, ctx.User, inputStep);

            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded) { return; }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);
        }
    }
}
