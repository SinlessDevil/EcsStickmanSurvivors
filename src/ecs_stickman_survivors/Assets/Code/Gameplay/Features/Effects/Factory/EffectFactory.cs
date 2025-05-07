using System;
using Code.Common.Entity;
using Code.Common.Extensions;
using Code.Infrastructure.Identifiers;

namespace Code.Gameplay.Features.Effects.Factory
{
    public class EffectFactory : IEffectFactory
    {
        private readonly IIdentifierService _identifierService;

        public EffectFactory(IIdentifierService identifierService)
        {
            _identifierService = identifierService;
        }

        public GameEntity CreateEffect(EffectSetup setup, int producerId, int targetId)
        {
            switch (setup.EffectTypeId)
            {
                case EffectTypeId.Unknown:
                    break;
                case EffectTypeId.Damage:
                    return CreateDamage(producerId, targetId, setup.Value);
                case EffectTypeId.Heal:
                    return CreateHeal(producerId, targetId, setup.Value);
                case EffectTypeId.Speed:
                    return CreateSpeed(producerId, targetId, setup.Value);
            }
            
            throw new Exception($"Effect with type id {setup.EffectTypeId} does not exist");
        }

        private GameEntity CreateHeal(int producerId, int targetId, float heal)
        {
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .With(x => x.isEffect = true)
                .With(x => x.isHealEffect = true)
                .AddEffectValue(heal)
                .AddProducerId(producerId)
                .AddTargetId(targetId);
        }

        private GameEntity CreateSpeed(int producerId, int targetId, float heal)
        {
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .With(x => x.isEffect = true)
                .With(x => x.isSpeedEffect = true)
                .AddEffectValue(heal)
                .AddProducerId(producerId)
                .AddTargetId(targetId);
        }
        
        private GameEntity CreateDamage(int producerId, int targetId, float damage)
        {
            return CreateEntity.Empty()
                .AddId(_identifierService.Next())
                .With(x => x.isEffect = true)
                .With(x => x.isDamageEffect = true)
                .AddEffectValue(damage)
                .AddProducerId(producerId)
                .AddTargetId(targetId);
        }
    }
}