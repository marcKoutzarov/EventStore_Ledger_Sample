using ConsoleApp1.Enums;
using ConsoleApp1.Models;
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public static class AccountManager
    {
        // The $streams stream contains a link to the first event in every stream. One of the properties on it is the stream name
        // here we get all the streams that have an event. 
        public static List<string> GetAccounts(IEventStoreConnection connection)
        {
            List<String> result = new List<String>();
            var readEvents = connection.ReadStreamEventsForwardAsync("$streams", 0, 100, true).Result;
            var LastEvent = readEvents.NextEventNumber;
            readEvents = connection.ReadStreamEventsForwardAsync("$streams", 0, (int)LastEvent + 1, true).Result;
            foreach (var e in readEvents.Events)
            {
                if (e.Event != null)
                {

                   // if (e.Event.EventStreamId.StartsWith("acc-"))
                   // {
                          result.Add(e.Event.EventStreamId);
                   // }
                }
            }
            return result;
        }

        // Query if a Stream exists 
        public static bool StreamExits(IEventStoreConnection connection, string StreamName)
        {
            var LastEventsStream = connection .ReadStreamEventsBackwardAsync(StreamName.ToLower(), 0, 1, false).Result;
            SliceReadStatus Status = LastEventsStream.Status;
            return (Status == 0);
        }

        // Check if a stream Exists
        public static string GetLastEvent(IEventStoreConnection connection, string accountStream) {

            if (AccountManager.StreamExits(connection,accountStream))
            {
                var EventsStream = connection.ReadStreamEventsBackwardAsync(accountStream,0, 1, true).Result;
                var LastEventNr = EventsStream.LastEventNumber;

                EventsStream = connection.ReadStreamEventsBackwardAsync(accountStream, LastEventNr, 1, true).Result;
                var LastEvent = EventsStream.Events[0].Event;


                var json = Encoding.UTF8.GetString(LastEvent.Data);
                return json;
            }
            else
            {
                return null;
            }
        }

        // create an Account stream
        public static void CreateAccountStream(IEventStoreConnection connection, string currency, string accountHolder, string accountNr, AccountTypes accountType)
        {
           if (AccountManager.StreamExits(connection, $"acc-{accountType.ToString()}_{accountNr}")) { return;};

            var genesisMutation = MutationEventManager.GenesisMutation(currency, accountHolder, accountNr, accountType);

            // convert to json
            var json = Encoding.UTF8.GetBytes(Models.Mutation.ToJson(genesisMutation));
           
            // create an event
            var myEvent = new EventData(Guid.Parse(genesisMutation.MutationId), "posting", true, json, null);
  
            //Append Initial event
            connection.AppendToStreamAsync($"acc-{accountType.ToString().ToLower()}_{accountNr}".ToLower(),-1, myEvent).Wait();
        }
        
    }
}
