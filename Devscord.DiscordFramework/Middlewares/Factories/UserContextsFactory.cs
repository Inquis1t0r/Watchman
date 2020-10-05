﻿using Autofac;
using Devscord.DiscordFramework.Middlewares.Contexts;
using Devscord.DiscordFramework.Services;
using Discord;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Devscord.DiscordFramework.Middlewares.Factories
{
    internal class UserContextsFactory : IContextFactory<IUser, UserContext>
    {
        private readonly UsersRolesService _usersRolesService;
        private readonly DiscordServersService _discordServersService;
        private readonly UsersService _usersService;

        public UserContextsFactory(IComponentContext context, UsersService usersService)
        {
            this._usersRolesService = context.Resolve<UsersRolesService>();
            this._discordServersService = context.Resolve<DiscordServersService>();
            this._usersService = usersService;
        }

        public UserContext Create(IUser user)
        {
            var avatarUrl = user.GetAvatarUrl(ImageFormat.Png, 2048);
            if (!(user is IGuildUser guildUser))
            {
                return new UserContext(user.Id, user.ToString(), new List<UserRole>(), avatarUrl, user.Mention, getIsOwner: _ => false, getJoinedServerAt: _ => null);
            }
            var roles = guildUser.RoleIds.Select(x => this._usersRolesService.GetRole(x, guildUser.GuildId));
            bool getIsOwner(UserContext userContext) => userContext.Id == this._discordServersService.GetDiscordServerAsync(guildUser.GuildId).Result.GetOwner().Id;
            DateTime? getJoinedServerAt(UserContext userContext) => this._usersService.GetUserJoinedServerAt(guildUser);

            return new UserContext(user.Id, user.ToString(), roles.ToList(), avatarUrl, user.Mention, getIsOwner, getJoinedServerAt);
        }
    }
}
