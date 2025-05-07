using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.Armaments.Extensions;
using Code.Gameplay.Features.Armaments.Factory;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class RadiatingCogBoltAbilitySystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IArmamentFactory _armamentFactory;
        private readonly IAbilityUpgradeService _abilityUpgradeService;

        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _heroes;
        private readonly IGroup<GameEntity> _enemies;

        private readonly List<GameEntity> _buffer = new(64);

        public RadiatingCogBoltAbilitySystem(GameContext game,
            IStaticDataService staticDataService,
            IArmamentFactory armamentFactory,
            IAbilityUpgradeService abilityUpgradeService)
        {
            _staticDataService = staticDataService;
            _armamentFactory = armamentFactory;
            _abilityUpgradeService = abilityUpgradeService;

            _abilities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.RadiatingCogBoltAbility,
                    GameMatcher.CooldownUp));

            _heroes = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.WorldPosition));

            _enemies = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Enemy,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity ability in _abilities.GetEntities(_buffer))
            foreach (GameEntity hero in _heroes)
            {
                if (_enemies.count <= 0)
                    continue;

                int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.RadiatingCogBolt);
                var abilityLevel = _staticDataService.GetAbilityLevel(AbilityId.RadiatingCogBolt, level);
                
                for (int i = 0; i < abilityLevel.ProjectileSetup.ProjectileCount; i++)
                {
                    _armamentFactory
                        .CreateRadiatingCogBolt(level, hero.WorldPosition)
                        .AddProducerId(hero.Id)
                        .ReplaceDirection(i.GetDirectionByRadian(abilityLevel.ProjectileSetup.SpreadAngle, abilityLevel.ProjectileSetup.ProjectileCount))
                        .With(x => x.isMoving = true);

                    ability
                        .PutOnCooldown(abilityLevel.Cooldown);
                }
            }
        }
    }
}