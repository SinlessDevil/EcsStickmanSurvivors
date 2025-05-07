using Code.Gameplay.Features.LevelUp.Services;
using Entitas;

namespace Code.Gameplay.Features.LevelUp.Systems
{
    public class UpdateExperienceMeterSystem : IExecuteSystem
    {
        private readonly ILevelUpService _levelUpService;
        
        private readonly IGroup<GameEntity> _experienceMeters;
        private readonly IGroup<GameEntity> _heroes;

        public UpdateExperienceMeterSystem(GameContext game, ILevelUpService levelUpService)
        {
            _levelUpService = levelUpService;
            
            _experienceMeters = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.ExperienceMeter));

            _heroes = game.GetGroup(GameMatcher
                .AllOf(GameMatcher.Hero,
                    GameMatcher.Experience));
        }

        public void Execute()
        {
            foreach (GameEntity experienceMeter in _experienceMeters)
            foreach (GameEntity hero in _heroes)
                experienceMeter.ExperienceMeter.SetExperience(hero.Experience, _levelUpService.ExperienceForLevelUp());
        }
    }
}