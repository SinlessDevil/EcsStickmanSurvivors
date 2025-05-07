using System;
using Code.Common.Entity;
using Code.Gameplay.StaticData;
using Code.Infrastructure.Identifiers;
using Code.Meta.UI.Shop.Items;

namespace Code.Meta.UI.Shop.UIFactory
{
    public class ShopItemFactory : IShopItemFactory
    {
        private readonly IStaticDataService _staticDataService;
        private readonly IIdentifierService _identifierService;

        public ShopItemFactory(IStaticDataService staticDataService, IIdentifierService identifierService)
        {
            _staticDataService = staticDataService;
            _identifierService = identifierService;
        }

        public MetaEntity CreateShopItem(ShopItemId shopItemId)
        {
            var config = _staticDataService.GetShopItemConfig(shopItemId);

            switch (config.Kind)
            {
                case ShopItemKind.Booster:
                   return CreateMetaEntity.Empty()
                        .AddId(_identifierService.Next())
                        .AddGoldGainBoost(config.Boost)
                        .AddDuration(config.Duration);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}