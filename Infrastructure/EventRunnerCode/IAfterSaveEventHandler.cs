﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using DataLayerEvents.DomainEventCode;

namespace Infrastructure.EventRunnerCode
{
    public interface IAfterSaveEventHandler<in T> where T : IDomainEvent
    {
        void Handle(T domainEvent);
    }
}