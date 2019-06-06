//-----------------------------------------------------------------------
// <copyright file="Program.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
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

            AccountManager.CreateCashAccountStream(c.Connection, "EUR", "PaySociety Europe", "001");

            for (int i = 1; i < 1000000; i++)
            {
                var AccountNr = $"00{i + 1}";
                AccountManager.CreateWalletStream(c.Connection, "EUR", $"John_Acc_{i + 1}", AccountNr);
            }


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
            var json = AccountManager.GetLastEvent(c.Connection, "acc-200.002");
            Console.WriteLine(json);
            Console.WriteLine(divider);



            Console.WriteLine("Press Any Key to Add a Wallet Deposit (funding");
            Console.ReadKey();



            var st = new System.Diagnostics.Stopwatch();
            var dm = new DepositManager();

            st.Start();



            // Avg is now 200ms per mmutation. If we turn of all projections except $streams
            for(int i = 1; i < 1000000; i++)
            {
                var AccountNr = $"acc-200.00{i + 1}";
                dm.HandleWalletDeposit(c.Connection, AccountNr, "acc-100.001", 1000.00m, "EUR", "Deposit from City Bank", "Account nr 888888");
                dm.HandleWalletDeposit(c.Connection, AccountNr, "acc-100.001", 1000m, "THB", "Deposit from Bangkok Bank", "Account nr 7777");
            }


            st.Stop();

            Console.WriteLine($"Ms elapsed : {st.ElapsedMilliseconds/1000} Seconds for 99999 mutations = 199998 Events.");
            Console.WriteLine($"Avg : {st.ElapsedMilliseconds/ 199998} Ms per mutation.");
            Console.WriteLine($"Avg : {st.ElapsedMilliseconds /399998} Ms per Event.");

            Console.WriteLine(divider);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


   


    }
}
