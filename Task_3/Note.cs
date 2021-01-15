using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft;
using Newtonsoft.Json;

namespace Task_3
{
    class Note : INote
    {
        [JsonProperty("id")]
        public  int Id { get;  set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("text")]
        public string Text { get; set; }
        [JsonProperty("createdOn")]
        public DateTime CreatedOn { get; set; }

        public override string ToString()
        {
            return $"[Creation date: {CreatedOn}]\n" +
                $"[Id: {Id}] Title: {Title}\n" +
                $"Note body:\n" +
                $"{Text}";
        }
    }
}
