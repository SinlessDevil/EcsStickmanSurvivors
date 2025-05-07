using Code.Meta.UI.GoldHolder.Service;
using Entitas;

namespace Code.Meta.UI.ResourceHolder.Systems
{
    public class RefreshGoldSystem : IExecuteSystem
    {
        private readonly IStorageUIService _storageUIService;
        private readonly IGroup<MetaEntity> _storages;

        public RefreshGoldSystem(MetaContext meta, IStorageUIService storageUIService)
        {
            _storageUIService = storageUIService;
            
            _storages = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage, MetaMatcher.Gold));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
                _storageUIService.UpdateResource(ResourceTypeId.Gold, storage.Gold);
        }
    }
}