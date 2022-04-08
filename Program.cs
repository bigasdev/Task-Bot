using System;

namespace discordbottemplate
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting the bot!");
            var bot = new Bot();
            bot.RunAsync().GetAwaiter().GetResult();
        }
    }
}
