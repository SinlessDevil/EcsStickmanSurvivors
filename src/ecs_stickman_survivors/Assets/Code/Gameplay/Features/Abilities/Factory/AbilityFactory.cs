using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Configs;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Identifiers;

namespace Code.Gameplay.Features.Abilities.Factory
{
    public class AbilityFactory : IAbilityFactory
    {
        private readonly IIdentifierService _identifierService;
        private readonly IStaticDataService _staticDataService;

        public AbilityFactory(
            IIdentifierService identifierService, 
            IStaticDataService staticDataService)
        {
            _identifierService = identifierService;
            _staticDataService = staticDataService;
        }

        public GameEntity CreateVegetableBoltAbility(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.VegetableBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.VegetableBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .With(x => x.isVegetableBoltAbility = true)
                .PutOnCooldown();
        }
        
        public GameEntity CreateRadiatingCogBoltAbility(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.RadiatingCogBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.RadiatingCogBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .With(x => x.isRadiatingCogBoltAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true)
                .PutOnCooldown();
        }
        
        public GameEntity CreateBouncingCoinBoltAbility(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.BouncingCoinBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.BouncingCoinBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .AddBounceRate(abilityLevel.ProjectileSetup.MaxBounces)
                .With(x => x.isBouncingCoinAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true)
                .PutOnCooldown();
        }
        
        public GameEntity CreateScatteringRuneStoneBolt(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.ScatteringRuneStoneBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.ScatteringRuneStoneBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .With(x => x.isScatteringRuneStoneAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true)
                .PutOnCooldown();
        }
        
        public GameEntity CreateOrbitingMushroomBolt(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.OrbitingMushroomBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.OrbitingMushroomBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .With(x => x.isOrbitingMushroomBoltAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true)
                .PutOnCooldown();
        }

        public GameEntity CreateBombBolt(int level)
        {
            AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.BombBolt, level);
            
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.BombBolt)
                .AddCooldown(abilityLevel.Cooldown)
                .With(x => x.isBombBoltAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true)
                .PutOnCooldown();
        }
        
        public GameEntity CreateGarlicAuraAbility()
        {
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .AddAbilityId(AbilityId.GarlicAura)
                .With(x => x.isGarlicAuraAbility = true)
                .With(x => x.isRecreatedOnUpgrade = true);
        }
    }
}