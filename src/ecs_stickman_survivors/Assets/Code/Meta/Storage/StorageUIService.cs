using System;
using System.Collections.Generic;

namespace Code.Meta.UI.GoldHolder.Service
{
    public class StorageUIService : IStorageUIService
    {
        private Dictionary<ResourceTypeId, float> _resources = new();
        private Dictionary<ResourceTypeId, float> _resourceBoosts = new();
        
        public event Action<ResourceTypeId> ResourceChangedEvent;
        public event Action<ResourceTypeId> ResourceBoostChangedEvent;
        
        public float GetResource(ResourceTypeId type) => _resources.TryGetValue(type, out var value) ? value : 0f;
        
        public float GetResourceBoost(ResourceTypeId type) => _resourceBoosts.TryGetValue(type, out var value) ? value : 0f;

        public void UpdateResource(ResourceTypeId type, float amount)
        {
            if (!_resources.ContainsKey(type) || Math.Abs(_resources[type] - amount) > float.Epsilon)
            {
                _resources[type] = amount;
                ResourceChangedEvent?.Invoke(type);
            }
        }
        
        public void UpdateResourceBoost(ResourceTypeId type, float boost)
        {
            _resourceBoosts[type] = boost;
            ResourceBoostChangedEvent?.Invoke(type);
        }
        
        public void Cleanup()
        {
            _resources.Clear();
            _resourceBoosts.Clear();
        }
    }
}