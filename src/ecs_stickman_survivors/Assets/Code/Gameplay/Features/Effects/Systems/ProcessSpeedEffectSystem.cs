using Code.Gameplay.Features.Effects.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Effects.Systems
{
    public class ProcessSpeedEffectSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _effects;
        
        public ProcessSpeedEffectSystem(GameContext game)
        {
            _effects = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.SpeedEffect,
                GameMatcher.EffectValue,
                GameMatcher.TargetId));
        }
        
        public void Execute()
        {
            foreach (GameEntity effect in _effects)
            {
                GameEntity target = effect.Target();

                effect.isProcessed = true;
                
                target.ReplaceSpeed(CalculatedSpeed(target, effect));
                
                // if(target.hasDamageTakenAnimator)
                //     target.DamageTakenAnimator.PlayHealTaken();
            }
        }

        private float CalculatedSpeed(GameEntity target, GameEntity effect)
        {
            var newSpeed = Mathf.Min(target.Speed + effect.EffectValue, 8);
            return newSpeed;
        }
    }
}