﻿using Devscord.DiscordFramework.Framework.Commands;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Administration.BotCommands
{
    public class UntrustCommand : IBotCommand
    {
        [SingleWord]
        public string RoleName { get; set; }
    }
}
