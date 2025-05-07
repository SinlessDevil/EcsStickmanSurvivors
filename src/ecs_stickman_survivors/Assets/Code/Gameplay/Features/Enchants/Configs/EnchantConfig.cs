using System.Collections.Generic;
using Code.Gameplay.Features.Effects;
using Code.Gameplay.Features.Statuses;
using Code.Infrastructure.View;
using UnityEngine;

namespace Code.Gameplay.Features.Enchants.Configs
{
    [CreateAssetMenu(menuName = "ECS Survivors/Enchant", fileName = "EnchantConfig")]
    public class EnchantConfig : ScriptableObject
    {
        public EnchantTypeId EnchantTypeId;
        public Sprite Icon;
        
        public List<EffectSetup> EffectSetups;
        public List<StatusSetup> StatusSetups;
        
        public float Radius;
        public EntityBehaviour ViewPrefab;
    }
}