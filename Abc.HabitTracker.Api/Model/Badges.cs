using System;
using System.Text.Json.Serialization;
using System.Collections.Generic;


namespace Abc.HabitTracker.Api
{
  public class Badges
  {
    [JsonPropertyName("id")]
    public Guid ID { get; set; }

    [JsonPropertyName("name")]
    public String name { get; set; }

    [JsonPropertyName("description")]
    public String description { get; set; }

    [JsonPropertyName("user_id")]
    public Guid user_id { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime created_at { get; set; }
  }
}
