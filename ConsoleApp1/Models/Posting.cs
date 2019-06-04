using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp1.Models
{
    public class Posting
    {
        [JsonProperty("Mutations")]
        public Mutation[] Mutations {get; set; } = new Mutation[2];
    }
}
