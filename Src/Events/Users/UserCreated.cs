﻿using Events.Infrastructure.Event;
using System;

namespace Events.Users
{
    public class UserCreated : IEvent
    {
        public Guid UserId { get; }

        public UserCreated(Guid userId)
        {
            UserId = userId;
        }
    }
}
