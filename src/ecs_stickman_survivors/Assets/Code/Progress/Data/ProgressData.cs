using System;
using Newtonsoft.Json;

namespace Code.Progress.Data
{
    public class ProgressData
    {
        [JsonProperty("e")] public EntityData EntityData = new();
        [JsonProperty("at")] public DateTime LastSimulationTickTime;
    }
}