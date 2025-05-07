using Code.Meta.UI.GoldHolder.Service;
using Entitas;

namespace Code.Meta.UI.ResourceHolder.Systems
{
    public class RefreshGemSystem : IExecuteSystem
    {
        private readonly IStorageUIService _storageUIService;
        private readonly IGroup<MetaEntity> _storages;

        public RefreshGemSystem(MetaContext meta, IStorageUIService storageUIService)
        {
            _storageUIService = storageUIService;
            
            _storages = meta.GetGroup(MetaMatcher
                .AllOf(MetaMatcher.Storage, MetaMatcher.Gem));
        }

        public void Execute()
        {
            foreach (MetaEntity storage in _storages)
                _storageUIService.UpdateResource(ResourceTypeId.Gem, storage.Gem);
        }
    }
}