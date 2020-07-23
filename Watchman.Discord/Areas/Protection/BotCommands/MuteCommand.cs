﻿using Devscord.DiscordFramework.Framework.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using Devscord.DiscordFramework.Framework.Commands.PropertyAttributes;

namespace Watchman.Discord.Areas.Protection.BotCommands
{
    public class MuteCommand : IBotCommand
    {
        [Text]
        public string Mention { get; set; }
        [Text]
        public string Time { get; set; }
        [Text]
        public string Reason { get; set; }
    }
}
