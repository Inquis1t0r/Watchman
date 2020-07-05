using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;
using Devscord.DiscordFramework.Framework.Commands.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Watchman.DomainModel.Messages.Commands
{
    class MessagesCommand
    {
        [UserMention]
        public string User { get; set; }
        [ChannelMention]
        public string Channel { get; set; }
    }
}
