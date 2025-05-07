using Entitas;

namespace Code.Gameplay.Features.Armaments.Systems
{
    public class FollowProducerSystem : IExecuteSystem
    {
        private readonly IGroup<GameEntity> _followers;
        private readonly IGroup<GameEntity> _producers;

        public FollowProducerSystem(GameContext game)
        {
            _followers = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.FollowingProducer,
                    GameMatcher.WorldPosition,
                    GameMatcher.ProducerId));

            _producers = game.GetGroup(GameMatcher
                .AllOf(
                    GameMatcher.Id,
                    GameMatcher.WorldPosition));
        }

        public void Execute()
        {
            foreach (GameEntity follower in _followers)
            foreach (GameEntity producer in _producers)
            {
                if(follower.ProducerId ==  producer.Id)
                    follower.ReplaceWorldPosition(producer.WorldPosition);
            }
        }
    }
}