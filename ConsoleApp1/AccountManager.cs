//-----------------------------------------------------------------------
// <copyright file="AccountManager.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp1.Enums;
using EventStore.ClientAPI;

namespace ConsoleApp1
{
    /// <summary>
    /// Manages Account Creation and Properties
    /// </summary>
    public static class AccountManager
    {
        /// <summary>
        /// The $streams stream contains a link to the first event in every stream. One of the properties on it is the stream name
        /// here we get all the streams that have an event. 
        /// </summary>
        /// <param name="connection">Connection to ES</param>
        /// <returns>List of Account names</returns>
        public static List<string> GetAccounts(IEventStoreConnection connection)
        {
            List<string> result = new List<string>();
            var readEvents = connection.ReadStreamEventsForwardAsync("$streams", 0, 100, true).Result;
            var lastEvent = readEvents.NextEventNumber;
            readEvents = connection.ReadStreamEventsForwardAsync("$streams", 0, (int)lastEvent + 1, true).Result;
            foreach (var e in readEvents.Events)
            {
                if (e.Event != null)
                {
                    if (e.Event.EventStreamId.StartsWith("acc-"))
                    {
                      result.Add(e.Event.EventStreamId);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Check if stream exists
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="streamName">Name of the stream</param>
        /// <returns>True if stream exists</returns>
        public static bool StreamExits(IEventStoreConnection connection, string streamName)
        {
            var lastEventsStream = connection.ReadStreamEventsBackwardAsync(streamName.ToLower(), 0, 1, false).Result;
            SliceReadStatus status = lastEventsStream.Status;
            return status == 0;
        }

        /// <summary>
        /// Get the Last published event
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="accountStream">Name of the stream</param>
        /// <returns>json string of the last event</returns>
        public static string GetLastEvent(IEventStoreConnection connection, string accountStream)
        {
            if (AccountManager.StreamExits(connection, accountStream))
            {
                var eventsStream = connection.ReadStreamEventsBackwardAsync(accountStream, 0, 1, true).Result;
                var lastEventNr = eventsStream.LastEventNumber;
                eventsStream = connection.ReadStreamEventsBackwardAsync(accountStream, lastEventNr, 1, true).Result;
                var lastEvent = eventsStream.Events[0].Event;
                var json = Encoding.UTF8.GetString(lastEvent.Data);
                return json;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// create an Account stream
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="currency">ISO 3</param>
        /// <param name="accountHolder">Name of the Account owner</param>
        /// <param name="accountNr">Account Number</param>
        /// <param name="accountType">Type of account</param>
        public static void CreateAccountStream(IEventStoreConnection connection, string currency, string accountHolder, string accountNr, AccountTypes accountType)
        {
            if (AccountManager.StreamExits(connection, $"acc-{(int)accountType}.{accountNr}"))
            {
                return;
            }

            var genesisMutation = MutationEventManager.GenesisMutation(currency, accountHolder, accountNr, accountType);
            
            // convert to json
            var json = Encoding.UTF8.GetBytes(Models.Mutation.ToJson(genesisMutation));
            
            // create an event
            var myEvent = new EventData(Guid.Parse(genesisMutation.MutationId), "posting", true, json, null);
            
            // Append Initial event
            connection.AppendToStreamAsync($"acc-{(int)accountType}.{accountNr}".ToLower(), -1, myEvent).Wait();
        }
    }
}
