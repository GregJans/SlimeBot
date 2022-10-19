using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlimeBotC.Commands
{
    class InteractiveCommands : BaseCommandModule
    {
        [Command("response")]
        [Description("Sends the next reaction to the channel (With a two minute time out)")]
        public async Task Response(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();

            var message = await interactivity.WaitForReactionAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Emoji);
        }

        [Command("join")]
        [Aliases("team")]
        [Description("Joins or leaves a team")]
        public async Task Join(CommandContext ctx)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Which team would you like to join?",
                Color = DiscordColor.Green,
                //ImageUrl = ctx.Client.CurrentUser.AvatarUrl
            };

            var joinMessage = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            // Place all emojis here
            var redSquare = DiscordEmoji.FromName(ctx.Client, ":red_square:");
            var blueSquare = DiscordEmoji.FromName(ctx.Client, ":blue_square:");
            var xAnswer = DiscordEmoji.FromName(ctx.Client, ":x:");



            //var greenSquare = DiscordEmoji.FromName(ctx.Client, ":green_square:");
            //var purpleSquare = DiscordEmoji.FromName(ctx.Client, ":purple_square:");
            //var orangeSquare = DiscordEmoji.FromName(ctx.Client, ":orange_square:");



            await joinMessage.CreateReactionAsync(redSquare).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(blueSquare).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(xAnswer).ConfigureAwait(false);

            //Extra teams if you ever want to add
            //await joinMessage.CreateReactionAsync(greenSquare).ConfigureAwait(false);
            //await joinMessage.CreateReactionAsync(purpleSquare).ConfigureAwait(false);
            //await joinMessage.CreateReactionAsync(orangeSquare).ConfigureAwait(false);


            var interactivity = ctx.Client.GetInteractivity();

            var result = await interactivity.WaitForReactionAsync(x => x.Message == joinMessage &&
                x.User == ctx.User &&
                (x.Emoji == redSquare || x.Emoji == blueSquare || x.Emoji == xAnswer)).ConfigureAwait(false);

            if (result.Result.Emoji == redSquare)
            {
                var role = ctx.Guild.GetRole(817527406448738334);

                foreach (var r in ctx.Member.Roles)
                {
                    if (r == ctx.Guild.GetRole(817527490830008330))
                    {
                        await ctx.Member.RevokeRoleAsync(r);
                    }
                }
                await ctx.Member.GrantRoleAsync(role);
            }
            else if (result.Result.Emoji == blueSquare)
            {
                var role = ctx.Guild.GetRole(817527490830008330);

                foreach (var r in ctx.Member.Roles)
                {
                    if (r == ctx.Guild.GetRole(817527406448738334))
                    {
                        await ctx.Member.RevokeRoleAsync(r);
                    }
                }

                await ctx.Member.GrantRoleAsync(role);
            }




            await joinMessage.DeleteAsync().ConfigureAwait(false);
        }

        [Command("poll")]
        [Description("Polls the server for one minute")]
        public async Task Poll(CommandContext ctx, [Description("What you would like to poll the users about")] string name = "Poll", [Description("All of the poll options as emojis")] params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());


            var embed = new DiscordEmbedBuilder
            {
                Title = name,
                Color = DiscordColor.Green,
                Description = string.Join(" or ", options)
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, new TimeSpan(0, 1, 0)).ConfigureAwait(false);
            var distinctResult = result.Distinct();

            var finalResult = 0;
            foreach (var test in distinctResult)
            {
                finalResult = Math.Max(finalResult, test.Total);
            }
            //distinctResult.Select(x => $"{x.Emoji}: {x.Total}");



            await ctx.Channel.SendMessageAsync("Looks like the winner has: \n" + finalResult + " vote(s)").ConfigureAwait(false);
        }

        [Command("suggest")]
        [Aliases("recommend", "suggestion")]
        [Description("Send a message telling me how you think the bot can be improved")]
        public async Task Suggest(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("What is your suggestion?").ConfigureAwait(false);
            var suggestion = await ctx.Channel.GetNextMessageAsync(x => x.Author == ctx.Message.Author, new TimeSpan(0, 2, 0)).ConfigureAwait(false);

            var owner = ctx.Guild.Owner;

            var embed = new DiscordEmbedBuilder
            {
                Title = $"New suggestion from {ctx.Message.Author.Username}",
                Description = suggestion.Result.Content
            };

            await owner.SendMessageAsync(embed: embed).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"Thank you {ctx.Member.Mention} for your valuable suggestion").ConfigureAwait(false);
        }

        [Command("rps")]
        [Aliases("rockpaperscissors")]
        [Description("Begins a game of rock, paper, scissors")]
        public async Task Rock(CommandContext ctx, [Description("Which of the three you choose")] string choice)
        {
            string[] options = { "rock", "paper", "scissors" };
            string[] emojis = { ":rock:", ":roll_of_paper:", ":scissors:" };
            Random botOpt = new Random();
            int index = botOpt.Next(0, options.Length);
            var botChoice = options[index];

            await ctx.Channel.SendMessageAsync($"{botChoice} {emojis[index]}").ConfigureAwait(false);

            if (!options.Contains<string>(choice))
            {
                await ctx.Channel.SendMessageAsync("Hey wait a minute, that wasnt an option. Try that again");
                var test = await ctx.Channel.GetNextMessageAsync(x => x.Author == ctx.Message.Author).ConfigureAwait(false);
                choice = test.Result.Content;
                await Rock(ctx, choice);
            }
            else if (choice == botChoice)
            {
                await ctx.Channel.SendMessageAsync("I guess its a tie, try again");
                var test = await ctx.Channel.GetNextMessageAsync(x => x.Author == ctx.Message.Author).ConfigureAwait(false);
                choice = test.Result.Content;
                await Rock(ctx, choice);
            }
            else
            {
                string message = string.Empty;

                if (choice == "rock")
                {
                    message = (botChoice == "scissors") ? "You win this time" : "You loose";
                }
                else if (choice == "paper")
                {
                    message = (botChoice == "rock") ? "Not bad. You win" : "You loose";
                }
                else if (choice == "scissors")
                {
                    message = (botChoice == "paper") ? "Cheater. I knew you'd win" : "You loose";
                }

                await ctx.Channel.SendMessageAsync(message).ConfigureAwait(false);
            }
        }


    }
}
