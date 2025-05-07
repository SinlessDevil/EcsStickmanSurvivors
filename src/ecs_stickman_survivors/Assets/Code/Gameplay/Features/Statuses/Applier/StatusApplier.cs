using System.Linq;
using Code.Common.EntityIndices;
using Code.Common.Extensions;
using Code.Gameplay.Features.Statuses.Factory;
using Unity.VisualScripting;
using UnityEngine;

namespace Code.Gameplay.Features.Statuses.Applier
{
    public class StatusApplier : IStatusApplier
    {
        private readonly IStatusFactory _statusFactory;
        private readonly GameContext _gameContext;

        public StatusApplier(IStatusFactory statusFactory, GameContext gameContext)
        {
            _statusFactory = statusFactory;
            _gameContext = gameContext;
        }

        public GameEntity ApplyStatusOnTarget(StatusSetup setup, int producerId, int targetId)
        {
            GameEntity status = _gameContext.TargetStatusesOfType(setup.StatusTypeId, targetId).FirstOrDefault();
            
            if (status != null && setup.IsStackable)
                return status.ReplaceTimeLeft(setup.Duration);

            if (status != null)
                return null;
            
            return _statusFactory
                .CreateStatus(setup, producerId, targetId)
                .With(x => x.isApplied = true);
        }

        public GameEntity ApplyStatusOnProducer(StatusSetup setup, int producerId, int targetId)
        {
            GameEntity status = _gameContext.TargetStatusesOfType(setup.StatusTypeId, producerId).FirstOrDefault();
            
            if (status != null && setup.IsStackable)
                return status.ReplaceTimeLeft(setup.Duration);
            
            if (status != null)
                return null;
            
            return _statusFactory
                .CreateStatus(setup, producerId, targetId)
                .With(x => x.isApplied = true);
        }
    }
}