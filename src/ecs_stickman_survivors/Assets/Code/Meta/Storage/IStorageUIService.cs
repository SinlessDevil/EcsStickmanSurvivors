using System;

namespace Code.Meta.UI.GoldHolder.Service
{
    public interface IStorageUIService
    {
        event Action<ResourceTypeId> ResourceChangedEvent;
        event Action<ResourceTypeId> ResourceBoostChangedEvent;
        
        float GetResource(ResourceTypeId type);
        float GetResourceBoost(ResourceTypeId type);
        
        void UpdateResource(ResourceTypeId type, float amount);
        void UpdateResourceBoost(ResourceTypeId type, float boost);
        
        void Cleanup();
    }
}