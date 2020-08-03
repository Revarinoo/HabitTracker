using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;
//template only. feel free to edit
namespace Abc.HabitTracker.Api
{
    public class RequestData
    {
        [JsonPropertyName("name")]
        public String Name { get; set; }

        [JsonPropertyName("days")]
        public String[] days { get; set; }
    }

    public class Habits
    {

        [JsonPropertyName("id")]
        public Guid ID { get; set; }

        [JsonPropertyName("name")]
        public String name { get; set; }

        [JsonPropertyName("user_id")]
        public Guid user_id { get; set; }

        [JsonPropertyName("days")]
        public List<String> days { get; set; }

        [JsonPropertyName("Log_count")]
        public int Log_count { get; set; }  

        [JsonPropertyName("current_streak")]
        public int current_streak { get; set; }   

        [JsonPropertyName("longest_streak")]
        public int longest_streak { get; set; }         

    }
}
