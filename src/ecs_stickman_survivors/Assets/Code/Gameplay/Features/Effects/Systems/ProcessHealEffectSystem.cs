using Code.Gameplay.Features.Effects.Extensions;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Features.Effects.Systems
{
    public class ProcessHealEffectSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _effects;
        
        public ProcessHealEffectSystem(GameContext game)
        {
            _effects = game.GetGroup(GameMatcher.AllOf(
                GameMatcher.HealEffect,
                GameMatcher.EffectValue,
                GameMatcher.TargetId));
        }
        
        public void Execute()
        {
            foreach (GameEntity effect in _effects)
            {
                GameEntity target = effect.Target();

                effect.isProcessed = true;
                
                if(target.isDead)
                    continue;
                
                if(!target.hasCurrentHp || !target.hasMaxHp)
                    return;
                
                target.ReplaceCurrentHp(CalculatedCurrentHp(target, effect));
                
                if(target.hasDamageTakenAnimator)
                    target.DamageTakenAnimator.PlayHealTaken();
            }
        }

        private float CalculatedCurrentHp(GameEntity target, GameEntity effect)
        {
            var newCurrentHp = Mathf.Min(target.CurrentHp + effect.EffectValue,target.MaxHp);
            return newCurrentHp;
        }
    }
}