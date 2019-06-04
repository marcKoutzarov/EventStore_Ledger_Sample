using ConsoleApp1.Models;
using EventStore.ClientAPI;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1
{
   public static class PostingManager
    {

        /// <summary>
        /// This Funds a wallet from a External Bank Deposit. 
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="walletAccount"></param>
        /// <param name="CashAccount"></param>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <param name="description"></param>
        /// <param name="description2"></param>
        public static void HandleWalletDeposit(IEventStoreConnection connection, string walletAccount, string CashAccount,decimal amount, string currency, string description, string description2)
        {
            // get the last mutations of the both Accounts
            Mutation walletAccountLastEvent = Mutation.FromJson(AccountManager.GetLastEvent(connection, walletAccount));
            Mutation CashAccountLastEvent = Mutation.FromJson(AccountManager.GetLastEvent(connection, CashAccount)); ;

            // TODO Must check if the CashAccount is of the type Cash and the Wallet Account of the Type Wallet
            // If not then do not continue. 


            //Create a Posting Object.
            Posting posting = MutationEventManager.CreatePosting(walletAccountLastEvent, CashAccountLastEvent, Enums.MutationTypes.Deposit, Enums.MutationEntryType.Dr, amount, currency, description, description2);
            Mutation walletAccountNewEvent = posting.Mutations[0];
            Mutation CashAccountNewEvent = posting.Mutations[1];

            
            // create an eventStore Event for the 2 Postings
            var myEvent1 = new EventData(Guid.Parse(walletAccountNewEvent.MutationId), "posting", true, Encoding.UTF8.GetBytes(Mutation.ToJson(walletAccountNewEvent)), null);
            var myEvent2 = new EventData(Guid.Parse(CashAccountNewEvent.MutationId), "posting", true, Encoding.UTF8.GetBytes(Mutation.ToJson(CashAccountNewEvent)), null);


            // First Credit the Cash Account
            //Append Event CashAccount
            connection.AppendToStreamAsync(CashAccount, CashAccountNewEvent.PreviousEventNumber, myEvent2).Wait();

            // Then Debit the Wallet Account
            // Append Event Wallet
            connection.AppendToStreamAsync(walletAccount, walletAccountNewEvent.PreviousEventNumber, myEvent1).Wait();

        }
    }
}
