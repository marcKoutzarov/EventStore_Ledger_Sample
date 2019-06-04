using System;
using System.Collections.Generic;
using System.Text;
using EventStore.ClientAPI;

namespace ConsoleApp1
{
    class Program
    {

   
        static void Main(string[] args)
        {
            const string divider = "\n";
            var c = new ConnectionManager();


            Console.WriteLine("Connecting to the eventstore");
            c.Connect();

            Console.WriteLine(c.Status);

            
            Console.WriteLine(divider);
            Console.WriteLine("Press Any Key to Create an Account");
            Console.ReadKey();
            AccountManager.CreateAccountStream(c.Connection, "EUR", "John", "100.0001", Enums.AccountTypes.Wallet);
            AccountManager.CreateAccountStream(c.Connection, "USD", "Marc", "100.0002", Enums.AccountTypes.Wallet);
            AccountManager.CreateAccountStream(c.Connection, "THB", "Lisa", "100.0003", Enums.AccountTypes.Wallet);
            AccountManager.CreateAccountStream(c.Connection, "VND", "Bert", "100.0004", Enums.AccountTypes.Wallet);
            AccountManager.CreateAccountStream(c.Connection, "THB", "Arm", "100.0006", Enums.AccountTypes.Wallet);
            AccountManager.CreateAccountStream(c.Connection, "EUR", "PaySociety Europe", "200.0005", Enums.AccountTypes.Cash);
            Console.WriteLine("Accounts created");
            Console.WriteLine(divider);


            Console.WriteLine("Press Any Key to get list of account Streams");
            Console.ReadKey();
            var sList = AccountManager.GetAccounts(c.Connection);
            foreach (string stream in sList)
            {
                Console.WriteLine(stream);
            }




            Console.WriteLine(divider);
            Console.WriteLine("Press Any Key to Get Last event of a account");
            Console.ReadKey();
            var json = AccountManager.GetLastEvent(c.Connection, "acc-wallet_100.0001");
            Console.WriteLine(json);
            Console.WriteLine(divider);



            Console.WriteLine("Press Any Key append an event to a stream");
            Console.ReadKey();
            //AddEvent(c.Connection, "Acc_0004", 1.02m);

            var readEvents = c.Connection.ReadStreamEventsForwardAsync("$by_event_type", 0, 100, true).Result;
            var LastEvent = readEvents.NextEventNumber;





            Console.WriteLine(divider);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


   


    }
}
