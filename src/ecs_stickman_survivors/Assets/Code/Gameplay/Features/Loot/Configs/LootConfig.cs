using System.Collections.Generic;
using Code.Gameplay.Features.Effects;
using Code.Gameplay.Features.Statuses;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Loot.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/Loot", fileName = "LootConfig")]
    public class LootConfig : ScriptableObject
    {
        public LootTypeId LootTypeId;
        public float Experience;
        public EntityBehaviour Prefab;
        
        public List<EffectSetup> EffectSetups;
        public List<StatusSetup> StatusSetups;
    }
}