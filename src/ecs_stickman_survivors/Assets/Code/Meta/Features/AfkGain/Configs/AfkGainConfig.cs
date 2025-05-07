using UnityEngine;

namespace Code.Meta.Features.AfkGain.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/AfkGain", fileName = "AfkGainConfig")]
    public class AfkGainConfig : ScriptableObject
    {
        public float GoldPerSecond;
        
        public float GemPerSecond;
        public float GemChance;
    }
}