using System.Collections;
using UnityEngine;
using System;
using Newtonsoft.Json;

namespace NotReaper.Modifier
{
    [Serializable]
    public class Modifier
    {
        [JsonProperty(Order = -4)] public string type;
        [JsonProperty(Order = -3)] public float startTick;
        [JsonProperty(Order = -2)] public float endTick;
        [JsonProperty(Order = -1)] public float amount;
        [JsonProperty(Order = 1)] public float startPosX;
        [JsonProperty(Order = 2)] public float endPosX;

    }
}
