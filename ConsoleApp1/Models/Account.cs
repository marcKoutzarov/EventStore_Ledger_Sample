using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1.Models
{
    public class Account
    {
        [JsonProperty("AccountNumber")]
        public string AccountNumber { get; set; }

        [JsonProperty("AccountHolder")]
        public string AccountHolder { get; set; }

        [JsonProperty("AccountType")]
        public string AccountType { get; set; }

    }
}
