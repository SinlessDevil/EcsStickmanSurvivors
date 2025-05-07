using Code.Gameplay.Features.Abilities;
using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Factory
{
    public interface IArmamentFactory
    {
        GameEntity CreateVegetableBolt(int level, Vector3 at);
        GameEntity CreateRadiatingCogBolt(int level, Vector3 at);
        GameEntity CreateBouncingCoinBolt(int level, Vector3 at);
        GameEntity CreateScatteringRuneStoneBolt(int level, Vector3 at);
        GameEntity CreateOrbitingMushroomBolt(int level, Vector3 at, float phase);
        GameEntity CreateBombBolt(int level, Vector3 at);
        
        GameEntity CreateEffectAura(AbilityId parentAbilityId, int producerId, int level);
        
        GameEntity CreateExplosionEnchant(int producerId, Vector3 at);
    }
}