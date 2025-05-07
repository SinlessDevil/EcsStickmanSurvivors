using System.Collections.Generic;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Configs;
using Code.Gameplay.Features.Effects;
using Code.Gameplay.Features.Enchants;
using Code.Gameplay.Features.Enchants.Configs;
using Code.Gameplay.Features.Statuses;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Identifiers;
using UnityEngine;

namespace Code.Gameplay.Features.Armaments.Factory
{
    public class ArmamentFactory : IArmamentFactory
    {
        private const int TargetBufferSize = 16;
        
        private readonly IIdentifierService _identifierService;
        private readonly IStaticDataService _staticDataService;

        public ArmamentFactory(IIdentifierService identifierService, IStaticDataService staticDataService)
        {
            _identifierService = identifierService;
            _staticDataService = staticDataService;
        }
        
        public GameEntity CreateVegetableBolt(int level, Vector3 at)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.VegetableBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;

            return CreateProjectileEntity(at, abilityLevel, setup)
                .AddParentAbility(AbilityId.VegetableBolt)
                .With(x => x.isRotationAlignedByDirection = true);
        }
        
        public GameEntity CreateRadiatingCogBolt(int level, Vector3 at)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.RadiatingCogBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;

            return CreateProjectileEntity(at, abilityLevel, setup)
                .AddParentAbility(AbilityId.RadiatingCogBolt)
                .With(x => x.isRotationAlignedByDirection = true);
        }
        
        public GameEntity CreateBouncingCoinBolt(int level, Vector3 at)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.BouncingCoinBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;

            return CreateProjectileEntity(at, abilityLevel, setup)
                .AddParentAbility(AbilityId.BouncingCoinBolt)
                .AddBounceRate(abilityLevel.ProjectileSetup.MaxBounces)
                .With(x => x.isRotationAlignedByDirection = true);
        }
        
        public GameEntity CreateScatteringRuneStoneBolt(int level, Vector3 at)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.ScatteringRuneStoneBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;

            return CreateProjectileEntity(at, abilityLevel, setup)
                .AddParentAbility(AbilityId.ScatteringRuneStoneBolt)
                .With(x => x.isRotationAlignedByDirection = true);
        }
        
        public GameEntity CreateOrbitingMushroomBolt(int level, Vector3 at, float phase)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.OrbitingMushroomBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;

            return CreateProjectileEntity(at, abilityLevel, setup)
                .AddParentAbility(AbilityId.OrbitingMushroomBolt)
                .AddOrbitPhase(phase)
                .AddOrbitRadius(setup.OrbitRadius);
        }

        public GameEntity CreateBombBolt(int level, Vector3 at)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.BombBolt, level);
            ProjectileSetup setup = abilityLevel.ProjectileSetup;
            
            return CreateEntity.Empty()
                    .AddId(_identifierService.Next())
                    .With(x => x.isArmament = true)
                    .AddViewPrefab(abilityLevel.ViewPrefab)
                    .AddWorldPosition(at)
                    .AddSpeed(setup.Speed)
                    .With(x => x.isMovementAvailable = true)
                    .AddParentAbility(AbilityId.BombBolt)
                    .With(x => x.isRotationAlignedByDirection = true);
        }
        
        public GameEntity CreateEffectAura(AbilityId parentAbilityId, int producerId, int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.GarlicAura, level);
            AuraSetup setup = abilityLevel.AuraSetup;

            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddParentAbility(parentAbilityId)
                .AddViewPrefab(abilityLevel.ViewPrefab)
                .AddLayerMask(CollisionLayer.Enemy.AsMask())
                .AddRadius(setup.Radius)
                .AddCollectTargetsInterval(setup.Interval)
                .AddCollectTargetsTimer(0)
                .AddTargetsBuffer(new List<int>(TargetBufferSize))
                .With(x => x.AddEffectSetups(abilityLevel.EffectSetups),
                    when: !abilityLevel.EffectSetups.IsNullOrEmpty())
                .With(x => x.AddStatusSetups(abilityLevel.StatusSetups),
                    when: !abilityLevel.StatusSetups.IsNullOrEmpty())
                .AddProducerId(producerId)
                .AddWorldPosition(Vector3.zero)
                .With(x => x.isFollowingProducer = true);
        }
        
        public GameEntity CreateExplosionEnchant(int producerId, Vector3 at)
        {
            EnchantConfig explosionEnchantConfig = _staticDataService.GetEnchantConfig(EnchantTypeId.ExplosiveArmaments);

            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddLayerMask(CollisionLayer.Enemy.AsMask())
                .AddRadius(explosionEnchantConfig.Radius)
                .AddTargetsBuffer(new List<int>(TargetBufferSize))
                .With(x => x.AddEffectSetups(explosionEnchantConfig.EffectSetups), when: !explosionEnchantConfig.EffectSetups.IsNullOrEmpty())
                .With(x => x.AddStatusSetups(explosionEnchantConfig.StatusSetups), when: !explosionEnchantConfig.StatusSetups.IsNullOrEmpty())
                .AddViewPrefab(explosionEnchantConfig.ViewPrefab)
                .AddProducerId(producerId)
                .AddWorldPosition(at)
                .With(x => x.isReadyToCollectTargets = true)
                .AddSelfDestructTimer(1);
        }
        
        private GameEntity CreateProjectileEntity(Vector3 at, AbilityLevel abilityLevel, ProjectileSetup setup)
        {
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .With(x => x.isArmament = true)
                .AddViewPrefab(abilityLevel.ViewPrefab)
                .AddWorldPosition(at)
                .AddSpeed(setup.Speed)
                .With(x => x.AddEffectSetups(new List<EffectSetup>(abilityLevel.EffectSetups)), 
                    when: !abilityLevel.EffectSetups.IsNullOrEmpty())
                .With(x => x.AddStatusSetups(new List<StatusSetup>(abilityLevel.StatusSetups)), 
                    when: !abilityLevel.StatusSetups.IsNullOrEmpty())
                .With(x => x.AddTargetLimit(setup.Pierce), when: setup.Pierce > 0)
                .AddRadius(setup.ContactRadius)
                .AddTargetsBuffer(new List<int>(TargetBufferSize))
                .AddProcessedTargets(new List<int>(TargetBufferSize))
                .AddLayerMask(CollisionLayer.Enemy.AsMask())
                .With(x => x.isMovementAvailable = true)
                .With(x => x.isReadyToCollectTargets = true)
                .With(x => x.isCollectingTargetsContinuously = true)
                .AddSelfDestructTimer(setup.LifeTime);
        }
    }
}