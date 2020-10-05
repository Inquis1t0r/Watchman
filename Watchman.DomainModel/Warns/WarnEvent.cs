﻿using System;
using Watchman.Integrations.MongoDB;

namespace Watchman.DomainModel.Warns
{
    public class WarnEvent : Entity, IAggregateRoot
    {
        public ulong GrantorId { get; private set; }
        public ulong ReceiverId { get; private set; }
        public string Reason { get; private set; }
        public ulong ServerId { get; private set; }

        public WarnEvent(ulong grantorId, ulong receiverId, string reason, ulong serverId)
        {
            this.GrantorId = grantorId;
            this.ReceiverId = receiverId;
            this.Reason = reason;
            this.ServerId = serverId;
            this.CreatedAt = DateTime.Now;
        }
    }
}
