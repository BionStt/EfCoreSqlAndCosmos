﻿// Copyright (c) 2019 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataLayerEvents.DomainEventCode;
using Infrastructure.EventRunnerCode.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Infrastructure.EventRunnerCode
{
    public class EventsRunner : IEventsRunner
    {
        private readonly FindRunHandlers _findRunHandlers;

        public EventsRunner(IServiceProvider serviceProvider)
        {
            _findRunHandlers = new FindRunHandlers(serviceProvider);
        }

        public int RunEventsBeforeAfterSaveChanges(Func<IEnumerable<EntityEntry<EventsHolder>>> getTrackedEntities,  
            Func<int> callBaseSaveChanges)
        {
            RunBeforeSaveChangesEvents(getTrackedEntities);
            var numChanges = callBaseSaveChanges.Invoke();
            RunAfterSaveChangesEvents(getTrackedEntities);
            return numChanges;
        }

        public async Task<int> RunEventsBeforeAfterSaveChangesAsync(Func<IEnumerable<EntityEntry<EventsHolder>>> getTrackedEntities, 
            Func<Task<int>> callBaseSaveChangesAsync)
        {
            RunBeforeSaveChangesEvents(getTrackedEntities);
            var numChanges = await callBaseSaveChangesAsync.Invoke();
            RunAfterSaveChangesEvents(getTrackedEntities);
            return numChanges;
        }


        private void RunBeforeSaveChangesEvents(Func<IEnumerable<EntityEntry<EventsHolder>>> getTrackedEntities)
        {
            //This has to run until there are no new events, because one event might trigger another event
            bool shouldRunAgain;
            do
            {
                var eventsToRun = getTrackedEntities.Invoke()
                    .SelectMany(x => x.Entity.GetBeforeSaveEventsThenClear())
                    .ToList();

                shouldRunAgain = false;
                foreach (var domainEvent in eventsToRun)
                {
                    shouldRunAgain = true;
                    _findRunHandlers.DispatchBeforeSave(domainEvent);
                }
            } while (shouldRunAgain);
        }

        private void RunAfterSaveChangesEvents(Func<IEnumerable<EntityEntry<EventsHolder>>> getTrackedEntities)
        {
            var eventsToRun = getTrackedEntities.Invoke()
                .SelectMany(x => x.Entity.GetBeforeSaveEventsThenClear());
            foreach (var domainEvent in eventsToRun)
            { 
                _findRunHandlers.DispatchAfterSave(domainEvent);
            }
        }

    }
}