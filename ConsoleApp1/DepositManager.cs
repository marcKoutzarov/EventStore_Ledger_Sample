//-----------------------------------------------------------------------
// <copyright file="DepositManager.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using ConsoleApp1.Enums;
using ConsoleApp1.Models;
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
    public class DepositManager
    {
        private Mutation walletAccountLastEvent;
        private Mutation cashAccountLastEvent;
        private Posting posting;


        /// <summary>
        /// This Funds a wallet from a External Bank Deposit. 
        /// </summary>
        /// <param name="connection">ES connection</param>
        /// <param name="walletAccount">Account name of a wallet</param>
        /// <param name="CashAccount">The Cash account</param>
        /// <param name="amount">Transaction amount</param>
        /// <param name="currency">Currency ISO 3</param>
        /// <param name="description">Free test filed</param>
        /// <param name="description2">Free text field</param>
        public void HandleWalletDeposit(IEventStoreConnection connection, string walletAccount, string CashAccount, decimal amount, string currency, string description, string description2)
        {
            // reset the objects
            ResetManager();

          
           
            // get the last mutations of the both Accounts
            walletAccountLastEvent = Mutation.FromJson(AccountManager.GetLastEvent(connection, walletAccount));
            cashAccountLastEvent = Mutation.FromJson(AccountManager.GetLastEvent(connection, CashAccount));

          

            //
            // TODO Must check if the CashAccount is of the type Cash and the Wallet Account of the Type Wallet. If not then do not continue.
            //  


            // Create a Posting Object.
            posting = MutationEventManager.CreatePosting(walletAccountLastEvent, cashAccountLastEvent, Enums.MutationTypes.Deposit, Enums.MutationEntryTypes.Dr, amount, currency, description, description2);
            Mutation walletAccountNewEvent = posting.Mutations[0];
            Mutation CashAccountNewEvent = posting.Mutations[1];


            // create an eventStore Event for the 2 Postings
            var myEvent1 = new EventData(Guid.Parse(walletAccountNewEvent.MutationId), EventTypes.WalletFunded.ToString(), true, Encoding.UTF8.GetBytes(Mutation.ToJson(walletAccountNewEvent)), null);
            var myEvent2 = new EventData(Guid.Parse(CashAccountNewEvent.MutationId), EventTypes.WalletFunded.ToString(), true, Encoding.UTF8.GetBytes(Mutation.ToJson(CashAccountNewEvent)), null);

            // TODO validate the balance the wallet account can not be negative

            // TODO Calculate the fee

            // TODO transaction learn about it

            // this takes to long 165 ms..
            // Events is a Array. One day need to try to experiment with this.
            // First Credit the Cash Account 
            connection.AppendToStreamAsync(CashAccount, CashAccountNewEvent.PreviousEventNumber, myEvent2).Wait();
            //Then Debit the Wallet Account
            connection.AppendToStreamAsync(walletAccount, walletAccountNewEvent.PreviousEventNumber, myEvent1).Wait();

            // var st = new System.Diagnostics.Stopwatch();
            // st.Start();
            // st.Stop();
            // System.Diagnostics.Debug.WriteLine($"Mutation created: {st.ElapsedMilliseconds} ms");
        }

        private void ResetManager()
        {
            walletAccountLastEvent = new Mutation();
            cashAccountLastEvent = new Mutation();
            posting = new Posting();
        }
    }
}
