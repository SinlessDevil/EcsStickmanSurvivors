using System.Collections.Generic;
using Code.Common.Extensions;
using Code.Gameplay.Cameras.Provider;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.Armaments.Factory;
using Code.Gameplay.Features.Cooldowns;
using Code.Gameplay.StaticData;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Abilities.Systems
{
    public class BombBoltAbilitySystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IArmamentFactory _armamentFactory;
        private readonly IAbilityUpgradeService _abilityUpgradeService;
        private readonly ICameraProvider _cameraProvider;

        private readonly IGroup<GameEntity> _abilities;
        private readonly IGroup<GameEntity> _heroes;
        
        private readonly List<GameEntity> _buffer = new(1);

        public BombBoltAbilitySystem(GameContext game, 
            IStaticDataService staticDataService, 
            IArmamentFactory armamentFactory,
            IAbilityUpgradeService abilityUpgradeService,
            ICameraProvider cameraProvider)
        {
            _staticDataService = staticDataService;
            _armamentFactory = armamentFactory;
            _abilityUpgradeService = abilityUpgradeService;
            _cameraProvider = cameraProvider;

            _abilities = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.BombBoltAbility, 
                    GameMatcher.CooldownUp));

            _heroes = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Hero,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity ability in _abilities.GetEntities(_buffer))
            foreach (GameEntity hero in _heroes)
            {
                int level = _abilityUpgradeService.GetAbilityLevel(AbilityId.BombBolt);

                _armamentFactory.CreateBombBolt(level, hero.WorldPosition)
                    .AddProducerId(hero.Id)
                    .AddTargetPosition(GetRandomScreenPositionRelativeTo(hero.WorldPosition))
                    .With(x => x.isMoving = true)
                    .With(x => x.isBomb = true);
                
                ability.PutOnCooldown(_staticDataService.GetAbilityLevel(AbilityId.BombBolt, level).Cooldown);
            }
        }
        
        
        private Vector2 GetRandomScreenPositionRelativeTo(Vector2 heroPosition)
        {
            Camera cam =_cameraProvider.MainCamera;
            
            if (cam == null) 
                return heroPosition + new Vector2(2f, 0f);

            float screenHalfWidth = cam.orthographicSize * cam.aspect;
            float screenHalfHeight = cam.orthographicSize;
            
            float randomX = Random.Range(-screenHalfWidth, screenHalfWidth);
            float randomY = Random.Range(-screenHalfHeight, screenHalfHeight);
    
            return heroPosition + new Vector2(randomX, randomY);
        }

    }
}