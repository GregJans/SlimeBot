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
    [RequireRoles(RoleCheckMode.All, "Overlord")]
    class AdminCommands : BaseCommandModule
    {
        [Command("clear")]
        [Description("Clears the desired number of messages")]
        public async Task Clear(CommandContext ctx, [Description("How many messages to clear")] int amount = 5)
        {
            IEnumerable<DiscordMessage> messagesList = await ctx.Channel.GetMessagesBeforeAsync(ctx.Message.Id, amount);
            await ctx.Channel.DeleteMessagesAsync(messagesList);

        }

        [Command("removechannel")]
        [Aliases("remove")]
        [Description("Removes any channel type by name or ID")]
        public async Task RemoveChannel(CommandContext ctx, [Description("Name or ID of channel")] DiscordChannel channel)
        {
            await channel.DeleteAsync().ConfigureAwait(false);
        }

        [Command("newchannel")]
        [Aliases("new")]
        [Description("Adds a channel of any type")]
        public async Task NewChannel(CommandContext ctx, [Description("Channel type")] string type, [Description("Name of channel")] string channel, [Description("Optional : is it nsfw (true / false)")] bool? nsfw=null)
        {
            if (type == "text")
            {
                await ctx.Guild.CreateChannelAsync(channel, DSharpPlus.ChannelType.Text, nsfw: nsfw, parent: ctx.Channel.Parent).ConfigureAwait(false);
            }
            else if (type == "voice")
            {
                await ctx.Guild.CreateVoiceChannelAsync(channel, parent: ctx.Guild.GetChannel(700158385642799157).Parent).ConfigureAwait(false);
            }
        }

        [Command("mute")]
        [Aliases("m")]
        [Description("Mutes a user in a voice chat")]
        public async Task Mute(CommandContext ctx, [Description("The user to mute")] DiscordMember member)
        {
            await member.SetMuteAsync(!member.IsMuted);
        }

    }
}
