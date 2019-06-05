//-----------------------------------------------------------------------
// <copyright file="Account.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using Newtonsoft.Json;

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
