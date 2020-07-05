﻿using Devscord.DiscordFramework.Framework.Commands.Properties;
using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Devscord.DiscordFramework.Framework.Commands.Services
{
    public class BotCommandsPropertyConversionService
    {
        private readonly Regex _exTime = new Regex(@"(?<Value>\d+)(?<Unit>(ms|d|h|m|s))", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //private readonly Regex _exMention = new Regex(@"<@&?\d+>", RegexOptions.Compiled);
        private readonly Regex _exMention = new Regex(@"<@&?(?<Mention>\d+)>", RegexOptions.Compiled);

        public object ConvertType(string value, BotCommandPropertyType type)
        {
            return type switch
            {
                BotCommandPropertyType.Time => this.ToTimeSpan(value),
                BotCommandPropertyType.Number => int.Parse(value),//TODO add more types
                BotCommandPropertyType.Bool => bool.Parse(value),
                BotCommandPropertyType.Text => ulong.Parse(value), 
                _ => value
            };
        }

        private ulong ToUlong(string value)
        {
            ulong result = 0;
            var match = this._exMention.Match(value);
            if (match.Success)
            {
                var mention = match.Groups["Mention"].Value;
                result = ulong.Parse(mention);
            }
            return result;
        }

        private TimeSpan ToTimeSpan(string value)
        {
            var match = this._exTime.Match(value);
            var unit = match.Groups["Unit"].Value.ToLowerInvariant();
            var timeValue = uint.Parse(match.Groups["Value"].Value);
            return unit switch
            {
                "d" => TimeSpan.FromDays(timeValue),
                "h" => TimeSpan.FromHours(timeValue),
                "m" => TimeSpan.FromMinutes(timeValue),
                "s" => TimeSpan.FromSeconds(timeValue),
                "ms" => TimeSpan.FromMilliseconds(timeValue),
                _ => throw new ArgumentNullException()
            };
        }
    }
}
