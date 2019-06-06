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
        /// create stream for a Wallet
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="currency">ISO 3</param>
        /// <param name="accountHolder">Name of the Account owner</param>
        /// <param name="accountNr">Account Number</param>
        public static void CreateWalletStream(IEventStoreConnection connection, string currency, string accountHolder, string accountNr)
        {
            var streamName = $"acc-{(int)AccountTypes.Wallet}.{accountNr}".ToLower();

            if (AccountManager.StreamExits(connection, streamName))
            {
                return;
            }
                       
            var genesisMutation = MutationEventManager.GenesisMutation(currency, accountHolder, streamName, AccountTypes.Wallet);
            
            // convert to json
            var json = Encoding.UTF8.GetBytes(Models.Mutation.ToJson(genesisMutation));
            
            // create an event
            var myEvent = new EventData(Guid.Parse(genesisMutation.MutationId), EventTypes.CreatedWallet.ToString(), true, json, null);
            
            // Append Initial event
            connection.AppendToStreamAsync(streamName, -1, myEvent).Wait();
        }

        /// <summary>
        /// create stream for a Cash Account
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="currency">ISO 3</param>
        /// <param name="accountHolder">Name of the Account owner</param>
        /// <param name="accountNr">Account Number</param>
        public static void CreateCashAccountStream(IEventStoreConnection connection, string currency, string accountHolder, string accountNr)
        {
            var streamName = $"acc-{(int)AccountTypes.Cash}.{accountNr}".ToLower();

            if (AccountManager.StreamExits(connection, streamName))
            {
                return;
            }

            var genesisMutation = MutationEventManager.GenesisMutation(currency, accountHolder, streamName, AccountTypes.Cash);

            // convert to json
            var json = Encoding.UTF8.GetBytes(Models.Mutation.ToJson(genesisMutation));

            // create an event
            var myEvent = new EventData(Guid.Parse(genesisMutation.MutationId), EventTypes.CreatedCashAccount.ToString(), true, json, null);

            // Append Initial event
            connection.AppendToStreamAsync(streamName, -1, myEvent).Wait();
        }

        /// <summary>
        /// create stream for a Fees Account
        /// </summary>
        /// <param name="connection">connection to ES</param>
        /// <param name="currency">ISO 3</param>
        /// <param name="accountHolder">Name of the Account owner</param>
        /// <param name="accountNr">Account Number</param>
        public static void CreateFeeAccountStream(IEventStoreConnection connection, string currency, string accountHolder, string accountNr)
        {
            var streamName = $"acc-{(int)AccountTypes.Fee}.{accountNr}".ToLower();

            if (AccountManager.StreamExits(connection, streamName))
            {
                return;
            }

            var genesisMutation = MutationEventManager.GenesisMutation(currency, accountHolder, streamName, AccountTypes.Fee);

            // convert to json
            var json = Encoding.UTF8.GetBytes(Models.Mutation.ToJson(genesisMutation));

            // create an event
            var myEvent = new EventData(Guid.Parse(genesisMutation.MutationId), EventTypes.CreatedFeeAccount.ToString(), true, json, null);

            // Append Initial event
            connection.AppendToStreamAsync(streamName, -1, myEvent).Wait();
        }



    }
}
