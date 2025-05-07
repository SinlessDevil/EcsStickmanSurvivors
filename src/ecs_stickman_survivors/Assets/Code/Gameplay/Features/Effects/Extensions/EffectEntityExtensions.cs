namespace Code.Gameplay.Features.Effects.Extensions
{
    public static class EffectEntityExtensions
    {
        private static GameContext GameContext => Contexts.sharedInstance.game;

        public static GameEntity Producer(this GameEntity effect)
        {
            return effect.hasProducerId
                ? GameContext.GetEntityWithId(effect.ProducerId)
                : null;
        }
        
        public static GameEntity Target(this GameEntity effect)
        {
            return effect.hasTargetId
                ? GameContext.GetEntityWithId(effect.TargetId)
                : null;
        }
    }
}