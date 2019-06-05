//-----------------------------------------------------------------------
// <copyright file="Posting.cs" company="Paysociety">
//     All rights Reserved
// </copyright>
//-----------------------------------------------------------------------
using Newtonsoft.Json;

namespace ConsoleApp1.Models
{
    public class Posting
    {
        /// <summary>
        /// Gets or sets mutations. Change to the Account and Change to the Counter Account.
        /// </summary>
        [JsonProperty("Mutations")]
        public Mutation[] Mutations {get; set; } = new Mutation[2];
    }
}
