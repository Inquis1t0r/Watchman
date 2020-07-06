using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MessagesCommand : Attribute, IBotCommand
    {
       public string Command { get; private set; }
        public MessagesCommand(string command)
        {
            this.Command = command;
        }

        [UserMention]
        public ulong User { get; set; }
        [ChannelMention]
        public ulong Channel { get; set; }
    }
}
