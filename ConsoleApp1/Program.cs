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

            AccountManager.CreateAccountStream(c.Connection, "EUR", "PaySociety Europe", "01", Enums.AccountTypes.Cash);

            for (int i = 1; i < 11; i++)
            {
                var AccountNR = $"0{i + 1}";
                AccountManager.CreateAccountStream(c.Connection, "EUR", $"John_Acc_{i}", AccountNR, Enums.AccountTypes.Wallet);
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
            var json = AccountManager.GetLastEvent(c.Connection, "acc-wallet_100.0003");
            Console.WriteLine(json);
            Console.WriteLine(divider);



            Console.WriteLine("Press Any Key to Add a Wallet Deposit (funding");
            Console.ReadKey();



            var st = new System.Diagnostics.Stopwatch();
            var dm = new DepositManager();

            st.Start();



            // Avg is now 200ms per mmutation. If we turn of all projections except $streams
            for(int i = 0; i < 30; i++)
            {
                dm.HandleWalletDeposit(c.Connection, "acc-20.02", "acc-10.01", 25.25m, "EUR", "Deposit from City Bank", "Account nr 888888");
                dm.HandleWalletDeposit(c.Connection, "acc-20.02", "acc-10.01", 25.25m, "USD", "Deposit from HSBC", "Account nr 66666");
                dm.HandleWalletDeposit(c.Connection, "acc-20.02", "acc-10.01", 25.25m, "THB", "Deposit from Bangkok Bank", "Account nr 4565.4544.43");
                dm.HandleWalletDeposit(c.Connection, "acc-20.02", "acc-10.01", 25.25m, "SGD", "Deposit from ING Bank", "Account nr 4444");
            }


            st.Stop();

            Console.WriteLine($"Ms elapsed : {st.ElapsedMilliseconds} Ms for 150 mutations = 300 Events.");
            Console.WriteLine($"Avg : {st.ElapsedMilliseconds/150} Ms per mutation.");
            Console.WriteLine($"Avg : {st.ElapsedMilliseconds / 300} Ms per Event.");

            Console.WriteLine(divider);
            Console.WriteLine("Press any key to exit");
            Console.ReadKey();
        }


   


    }
}
