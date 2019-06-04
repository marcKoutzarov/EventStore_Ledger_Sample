using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Text;
using ConsoleApp1.Enums;

namespace ConsoleApp1
{
   public class MutationEventManager
    {
        /// <summary>
        /// Creates a Single Mutation
        /// </summary>
        /// <param name="account">Account affected by this mutation</param>
        /// <param name="previousEventNr">The Event Number of the previeous Event.</param>
        /// <param name="counterAccount">The Counter Party Account for this mutation</param>
        /// <param name="mutationType">Type of mutation example Deposit of Withdrawal</param>
        /// <param name="entryType">Credit of Debit</param>
        /// <param name="prevBalance">Previous balance of the Account</param>
        /// <param name="amount">The Ammount mutated</param>
        /// <param name="currency">Asset type ex USD</param>
        /// <param name="descr2"></param>
        /// <returns></returns>
        private static Mutation CreateMutation(Mutation lastAccountEvent,
                                             Mutation LastCounterAccountEvent,
                                             MutationTypes mutationType,
                                             MutationEntryType entryType,
                                             decimal amount,
                                             string currency,
                                             string descr2 = "", string descr3 = "")
        {

            var MutationId = Guid.NewGuid();

            //the amount credited will be calculated by the MutationEntryType
            var CreditAmount = 0m;

            //the amount credited will be calculated by the MutationEntryType
            var DebitAmount = 0m;

            //New balance will be calculated by the MutationEntryType Dr + and Cr -
            var newBalance = 0m;

            //this old balance is the new Balance of the previous mutation
            var oldBalance = 0m;

            // Find the Old Currency balance. If cussency is not there the balance is 0.00
            foreach (Balance b in lastAccountEvent.AccountBalances)
            {
                if (b.Currency == currency)
                {
                    oldBalance = b.NewBalance;
                }
            }


            // generate the default description (goes to descr1)
            var descr1 = ($"{mutationType.ToString().Replace('_', ' ')}  {entryType.ToString()} {amount} {currency}."); ;


            //Calucate the balance and Debit and Credit amount
            switch (entryType)
            {
                case MutationEntryType.Cr:
                    CreditAmount = amount;
                    newBalance = oldBalance - amount;

                    break;
                case MutationEntryType.Dr:
                    DebitAmount = amount;
                    newBalance = oldBalance + amount;
                    break;
            }

            //Create counterAccount Information
            CounterAccount counterAccount = new CounterAccount
            {
                AccountHolder = LastCounterAccountEvent.Account.AccountHolder,
                AccountNumber = LastCounterAccountEvent.Account.AccountNumber,
                AccountType = LastCounterAccountEvent.Account.AccountType,
                PreviousEventNumber = LastCounterAccountEvent.EventNumber,
                EventNumber = LastCounterAccountEvent.EventNumber + 1,
            };

            var mutation = new Mutation
            {
                MutationId = MutationId,
                Account = lastAccountEvent.Account,
                CounterAccount = counterAccount,
                PreviousEventNumber = lastAccountEvent.EventNumber,
                EventNumber = lastAccountEvent.EventNumber + 1,
                MutationType = mutationType.ToString(),
                Currency = currency.ToUpper(),
                AccountBalances = UpdateBalance(lastAccountEvent.AccountBalances, oldBalance, newBalance, currency.ToUpper()),
                Cr = CreditAmount,
                Dr = DebitAmount,
                Description1 = descr1,
                Description2 = descr2,
                Description3 = descr3
            };

            return mutation;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="balances"></param>
        /// <param name="oldBalance"></param>
        /// <param name="NewBalance"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private static List<Balance> UpdateBalance(List<Balance> balances, decimal oldBalance, decimal NewBalance, string currency)
        {

            var addBalanceItem = true;

            foreach (Balance b in balances)
            {
                if (b.Currency == currency)
                {
                    b.NewBalance = NewBalance;
                    b.OldBalance = oldBalance;
                    addBalanceItem = false;
                }
            }

            if (addBalanceItem)
            {
                balances.Add(new Balance
                {
                    Currency = currency,
                    NewBalance = NewBalance,
                    OldBalance = oldBalance
                });
            }

            return balances;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastAccountEvent"></param>
        /// <param name="LastCounterAccountEvent"></param>
        /// <param name="mutationType"></param>
        /// <param name="entryType"></param>
        /// <param name="amount"></param>
        /// <param name="currency"></param>
        /// <param name="descr2"></param>
        /// <param name="descr3"></param>
        /// <returns></returns>
        public static Posting CreatePosting(Mutation lastAccountEvent,
                                             Mutation LastCounterAccountEvent,
                                             MutationTypes mutationType,
                                             MutationEntryType entryType,
                                             decimal amount,
                                             string currency,
                                             string descr2 = "", string descr3 = "")
        {



            Posting result = new Posting();


            Mutation accountEvent = CreateMutation(lastAccountEvent, LastCounterAccountEvent, mutationType, entryType, amount, currency, descr2, descr3);


            MutationEntryType CounterEntryType;
            if (entryType == MutationEntryType.Cr)
            {
                CounterEntryType = MutationEntryType.Dr;
            }
            else
            {
                CounterEntryType = MutationEntryType.Cr;
            }


            Mutation CounteraccountEvent = CreateMutation(LastCounterAccountEvent, lastAccountEvent, mutationType, CounterEntryType, amount, currency, descr2, descr3);

            CounteraccountEvent.CounterAccount.MutationId = accountEvent.MutationId;

            result.Mutations[0] = accountEvent;

            result.Mutations[1] = CounteraccountEvent;

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="currency"></param>
        /// <param name="accountHolder"></param>
        /// <param name="accountNr"></param>
        /// <param name="accountType"></param>
        public static Mutation GenesisMutation(string currency, string accountHolder, string accountNr, AccountTypes accountType)
        {
            var MutationId = Guid.NewGuid();
            Mutation Result = new Mutation
            {
                AccountBalances = new List<Balance>() { new Balance { Currency = currency.ToUpper(), NewBalance = 0m, OldBalance = 0m } },
                Currency = currency,
                Cr = 0m,
                Dr = 0m,
                MutationId = MutationId,
                Account = new Account { AccountHolder = accountHolder, AccountNumber = accountNr, AccountType = accountType.ToString() },
                CounterAccount = new CounterAccount { AccountHolder = accountHolder, AccountNumber = accountNr, AccountType = accountType.ToString(), EventNumber = 0, PreviousEventNumber = 0, MutationId = MutationId },
                MutationType = MutationTypes.Deposit.ToString(),
                EventNumber = 0,
                PreviousEventNumber = 0,
                Description1 = "Account activation."
            };

            return Result;
        }


        public Posting CreatePosting(string Account,string CounterAccount, MutationTypes mutationType, MutationEntryType entryType, decimal amount,  string descr2 = "", string descr3 = ""){
            Posting result = new Posting();

            return result;
        }
    }
}
