using System;
using System.Collections.Generic;
using System.Linq;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Configs;
using Code.Gameplay.Features.Enchants;
using Code.Gameplay.Features.Enchants.Configs;
using Code.Gameplay.Features.Enemies;
using Code.Gameplay.Features.Enemies.Configs;
using Code.Gameplay.Features.Hero.Configs;
using Code.Gameplay.Features.LevelUp.Configs;
using Code.Gameplay.Features.Loot;
using Code.Gameplay.Features.Loot.Configs;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Configs;
using Code.Meta.Features.AfkGain.Configs;
using Code.Meta.UI.Shop.Items;
using UnityEngine;

namespace Code.Gameplay.StaticData
{
    public class StaticDataService : IStaticDataService
    {
        private Dictionary<AbilityId, AbilityConfig> _abilityById;
        private Dictionary<EnemyTypeId, EnemyConfig> _enemyById;
        private Dictionary<EnchantTypeId, EnchantConfig> _enchantById;
        private Dictionary<LootTypeId, LootConfig> _lootById;
        private Dictionary<WindowId, GameObject> _windowPrefabsById;
        private List<ShopItemConfig> _shopItemConfigs;

        private LevelupConfig _levelupRules;
        private HeroConfig _heroConfig;
        private EnemySpawnConfig _enemySpawnConfig;
        private EnemyDropConfig _enemyDropConfig;
        private AfkGainConfig _afkGainConfig;

        public void LoadAll()
        {
            LoadAbilities();
            LoadEnemies();
            LoadHeroConfig();
            LoadEnchants();
            LoadLoots();
            LoadWindows();
            LoadLevelUpRules();
            LoadEnemySpawnConfig();
            LoadEnemyDropConfig();
            LoadAfkGainConfig();
            LoadShopItems();
        }
        
        public AfkGainConfig AfkGainConfig => _afkGainConfig;
        
        public EnemySpawnConfig EnemySpawnConfig => _enemySpawnConfig;
        
        public EnemyDropConfig EnemyDropConfig => _enemyDropConfig;
        
        public HeroConfig HeroConfig => _heroConfig;
        
        public int MaxLevel => _levelupRules.MaxLevel;
        
        public float ExperienceForLevel(int level) => _levelupRules.ExperienceForLevel[level];
        
        public EnemyWave GetCurrentWave(int level)
        {
            for (int i = _enemySpawnConfig.Waves.Count - 1; i >= 0; i--)
            {
                if (level >= _enemySpawnConfig.Waves[i].LevelRequirement)
                    return _enemySpawnConfig.Waves[i];
            }
            return null;
        }

        public ShopItemConfig GetShopItemConfig(ShopItemId shopItemId)
        {
            return _shopItemConfigs.FirstOrDefault(x => x.ShopItemId == shopItemId);
        }
        
        public List<ShopItemConfig> GetShopItemConfigs()
        {
            return _shopItemConfigs;
        }
        
        public AbilityConfig GetAbilityConfig(AbilityId abilityId)
        {
            if(_abilityById.TryGetValue(abilityId, out AbilityConfig config)) 
                return config;

            throw new Exception($"Ability config for {abilityId} not found");
        }

        public AbilityLevel GetAbilityLevel(AbilityId abilityId, int level)
        {
            AbilityConfig config = GetAbilityConfig(abilityId);
            
            if (level <= 0)
            {
                level = 1;
            }
            else if (level > config.Levels.Count)
            {
                level = config.Levels.Count;
            }

            return config.Levels[level - 1];
        }

        
        public EnemyConfig GetEnemyConfig(EnemyTypeId enemyTypeId)
        {
            if(_enemyById.TryGetValue(enemyTypeId, out EnemyConfig config)) 
                return config;

            throw new Exception($"Enemy config for {enemyTypeId} not found");
        }
        
        public EnchantConfig GetEnchantConfig(EnchantTypeId enchantTypeId)
        {
            if(_enchantById.TryGetValue(enchantTypeId, out EnchantConfig config)) 
                return config;

            throw new Exception($"Enchant config for {enchantTypeId} not found");
        }

        public LootConfig GetLootConfig(LootTypeId lootTypeId)
        {
            if(_lootById.TryGetValue(lootTypeId, out LootConfig config)) 
                return config;

            throw new Exception($"Loot config config for {lootTypeId} not found");
        }
        
        public EnemyLevel GetEnemyLevel(EnemyTypeId enemyTypeId, int level)
        {
            EnemyConfig config = GetEnemyConfig(enemyTypeId);
            
            if (level > config.Levels.Count)
                level = config.Levels.Count;
            
            EnemyLevel closestLevel = null;
            foreach (var enemyLevel in config.Levels)
            {
                if (config.Levels.IndexOf(enemyLevel) <= level) 
                {
                    closestLevel = enemyLevel;
                }
                else
                {
                    break;
                }
            }
            return closestLevel;
        }
        
        public GameObject GetWindowPrefab(WindowId windowId)
        {
           return _windowPrefabsById.TryGetValue(windowId, out GameObject windowPrefab)
                ? windowPrefab
                : throw new Exception($"Prefab config for window {windowId} was not found");
        }
        
        private void LoadAbilities()
        {
            _abilityById = Resources
                .LoadAll<AbilityConfig>("Configs/Abilities")
                .ToDictionary(x => x.AbilityId, x => x);
        }
        
        private void LoadEnemies()
        {
            _enemyById = Resources
                .LoadAll<EnemyConfig>("Configs/Enemies")
                .ToDictionary(x => x.EnemyTypeId, x => x);
        }

        private void LoadEnchants()
        {
            _enchantById = Resources
                .LoadAll<EnchantConfig>("Configs/Enchants")
                .ToDictionary(x => x.EnchantTypeId, x => x);
        }

        private void LoadLoots()
        {
            _lootById = Resources
                .LoadAll<LootConfig>("Configs/Loots")
                .ToDictionary(x => x.LootTypeId, x => x);
        }
        
        private void LoadHeroConfig()
        {
            _heroConfig = Resources.Load<HeroConfig>("Configs/Heroes/HeroConfig");
        }
        
        private void LoadWindows()
        {
            _windowPrefabsById = Resources
                .Load<WindowsConfig>("Configs/Windows/WindowConfig")
                .WindowConfigs
                .ToDictionary(x => x.Id, x => x.Prefab);
        }
        
        private void LoadLevelUpRules()
        {
            _levelupRules = Resources.Load<LevelupConfig>("Configs/Levelup/LevelupConfig");
        }

        private void LoadEnemySpawnConfig()
        {
            _enemySpawnConfig = Resources.Load<EnemySpawnConfig>("Configs/EnemySpawn/EnemySpawnConfig");
        }
        
        private void LoadEnemyDropConfig()
        {
            _enemyDropConfig = Resources.Load<EnemyDropConfig>("Configs/EnemyDrop/EnemyDropConfig");
        }
        
        private void LoadAfkGainConfig()
        {
            _afkGainConfig = Resources.Load<AfkGainConfig>("Configs/AfkGain/AfkGainConfig");
        }

        private void LoadShopItems()
        {
            _shopItemConfigs = Resources.LoadAll<ShopItemConfig>("Configs/ShopItems").ToList();
        }
    }
}
