using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Configs;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.Effects;
using Code.Gameplay.Features.Statuses;
using Code.Gameplay.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Systems
{
    public class HandleBombExplosionSystem: IExecuteSystem
    {
        private const int TargetBufferSize = 16;
        
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _armaments;
        
        private readonly List<GameEntity> _bufferArmaments = new(32);
        
        private readonly IStaticDataService _staticDataService;
        private readonly IAbilityUpgradeService _abilityUpgradeService;

        public HandleBombExplosionSystem (GameContext game, 
            IStaticDataService staticDataService,
            IAbilityUpgradeService abilityUpgradeService)
        {
            _staticDataService = staticDataService;
            _abilityUpgradeService = abilityUpgradeService;

            _armaments = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Armament,
                    GameMatcher.Bomb).NoneOf(GameMatcher.Active));
        }

        public void Execute()
        {
            foreach (GameEntity armament in _armaments.GetEntities(_bufferArmaments))
            {
                if(armament.isMoving)
                    return;
                
                int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.BombBolt);
                AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.BombBolt, level);
                ProjectileSetup setup = abilityLevel.ProjectileSetup;
                AuraSetup auraSetup = abilityLevel.AuraSetup;
                
                armament
                    .AddRadius(auraSetup.Radius)
                    .AddCollectTargetsInterval(auraSetup.Interval)
                    .AddCollectTargetsTimer(0)
                    .AddTargetsBuffer(new List<int>(TargetBufferSize))
                    .With(x => x.AddEffectSetups(new List<EffectSetup>(abilityLevel.EffectSetups)), 
                        when: !abilityLevel.EffectSetups.IsNullOrEmpty())
                    .With(x => x.AddStatusSetups(new List<StatusSetup>(abilityLevel.StatusSetups)), 
                        when: !abilityLevel.StatusSetups.IsNullOrEmpty())
                    .AddLayerMask(CollisionLayer.Enemy.AsMask())
                    .AddSelfDestructTimer(setup.LifeTime);
                
                if(armament.hasArmamentBombVisual)
                    armament.ArmamentBombVisual.PlayExplosionArmament();
                
                armament.isActive = true;
            }
        }
    }
}