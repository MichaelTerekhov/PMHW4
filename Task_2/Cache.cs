using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Task_2
{
    class Cache
    {
        [JsonProperty("r030")]
        public int R030 { get; set; }

        [JsonProperty("txt")]
        public string Name { get; set; }

        [JsonProperty("rate")]
        public decimal Rate { get; set; }

        [JsonProperty("cc")]
        public string Currency { get; set; }

        [JsonProperty("exchangedate")]
        public string Exchangedate { get; set; }

        public override string ToString()
        {
            return $"Date: {Exchangedate} Currency: {Name} Rate(ToUah):{Rate} Code: {Currency}";
        }
    }
}
