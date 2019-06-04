using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Models
{
    /// <summary>
    /// Extention of the Account Class. Holding the 
    /// MutationId of the Revenue or Receivable.
    /// </summary>
    public class CounterAccount:Account
    {
        [JsonProperty("MutationId")]
        public System.Guid MutationId { get; set; }

        [JsonProperty("EventNumber")]
        public long EventNumber { get; set; }

        [JsonProperty("PreviousEventNumber")]
        public long PreviousEventNumber { get; set; }

    }
}
