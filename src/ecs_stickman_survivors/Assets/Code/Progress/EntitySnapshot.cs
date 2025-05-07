using System.Collections.Generic;
using Newtonsoft.Json;

namespace Code.Progress
{
    public class EntitySnapshot
    {
        [JsonProperty("c")] public List<ISavedComponent> Components;
    }
}