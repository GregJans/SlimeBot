using System;
//using Discord;

namespace SlimeBotC
{

    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
            
    }
}
