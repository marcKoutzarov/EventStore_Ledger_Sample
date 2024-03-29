﻿//-----------------------------------------------------------------------
// <copyright file="CounterAccount.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using Newtonsoft.Json;

namespace ConsoleApp1.Models
{
    /// <summary>
    /// CounterAccount Class. Holding the MutationId of the Revenue or Receivable.
    /// </summary>
    public class CounterAccount: Account
    {
        [JsonProperty("MutationId")]
        public string MutationId { get; set; }

        [JsonProperty("EventNumber")]
        public long EventNumber { get; set; }

        [JsonProperty("PreviousEventNumber")]
        public long PreviousEventNumber { get; set; }
    }
}
