using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class MessagesCommand : IBotCommand
    {
        [UserMention]
        public ulong User { get; set; }
        [ChannelMention]
        public ulong Channel { get; set; }
    }
}
