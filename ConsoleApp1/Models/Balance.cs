//-----------------------------------------------------------------------
// <copyright file="Balance.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Models
{
    public class Balance
    {
        [JsonProperty("Currency")]
        public string Currency { get; set; }

        [JsonProperty("OldBalance")]
        public decimal OldBalance { get; set; }

        [JsonProperty("NewBalance")]
        public decimal NewBalance { get; set; }
    }
}
