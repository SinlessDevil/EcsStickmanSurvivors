using System.Collections.Generic;
using Code.Gameplay.Features.Statuses;
using Code.Gameplay.StaticData;
using Entitas;

namespace Code.Gameplay.Features.Enchants.Systems
{
    public class HexEnchantSystem : IExecuteSystem
    {
        private readonly IStaticDataService _staticDataService;
        
        private readonly IGroup<GameEntity> _enchants;
        private readonly IGroup<GameEntity> _armamets;
        
        private readonly List<GameEntity> _buffer = new(32);

        public HexEnchantSystem(GameContext game, IStaticDataService staticDataService)
        {
            _staticDataService = staticDataService;
            
            _enchants = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.EnchantTypeId,
                    GameMatcher.ProducerId,
                    GameMatcher.HexEnchant));
            
            _armamets = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Armament,
                    GameMatcher.ProducerId)
                .NoneOf(GameMatcher.HexEnchant));
        }

        public void Execute()
        {
            foreach (GameEntity enchant in _enchants)
            foreach (GameEntity armament in _armamets.GetEntities(_buffer))
            {
                if (enchant.ProducerId == armament.ProducerId)
                {
                    var newStatusSetups = new List<StatusSetup>();
                    newStatusSetups.AddRange(_staticDataService.GetEnchantConfig(EnchantTypeId.HexArmaments).StatusSetups);
                    
                    GetOrAddStatusSetups(armament).AddRange(newStatusSetups);
                    armament.isHexEnchant = true;
                }
            }
        }
        
        private List<StatusSetup> GetOrAddStatusSetups(GameEntity armament)
        {
            if (!armament.hasStatusSetups)
                armament.AddStatusSetups(new List<StatusSetup>());

            return armament.StatusSetups;
        }
    }
}