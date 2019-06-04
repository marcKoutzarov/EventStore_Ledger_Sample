using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace ConsoleApp1.Models
{
   public class Mutation
   {
        [JsonProperty("MutationId")]
        public System.Guid MutationId { get; set; }

        [JsonProperty("EventNumber")]
        public long EventNumber { get; set; }

        [JsonProperty("PreviousEventNumber")]
        public long PreviousEventNumber { get; set; }

        [JsonProperty("MutationType")]
        public string MutationType { get; set; }

        [JsonProperty("Account")]
        public Account Account { get; set; }

        [JsonProperty("CounterParty")]
        public CounterAccount CounterAccount { get; set; }
     
        [JsonProperty("AccountBalances")]
        public List<Balance> AccountBalances { get; set; }

        [JsonProperty("Cr")]
        public decimal Cr { get; set; }

        [JsonProperty("Dr")]
        public decimal Dr { get; set; }

        [JsonProperty("Currency")]
        public string Currency { get; set; }
    
        [JsonProperty("Decription1")]
        public string Description1 { get; set; }

        [JsonProperty("Decription2")]
        public string Description2 { get; set; }

        [JsonProperty("Decription3")]
        public string Description3 { get; set; }

        public static Mutation FromJson(string json) => JsonConvert.DeserializeObject<Mutation>(json, Converter.Settings);

        public static string ToJson(Mutation self) => JsonConvert.SerializeObject(self, Formatting.Indented, Converter.Settings);

                           


    }



    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }




}
