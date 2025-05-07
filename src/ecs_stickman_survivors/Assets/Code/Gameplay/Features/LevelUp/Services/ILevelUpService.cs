namespace Code.Gameplay.Features.LevelUp.Services
{
    public interface ILevelUpService
    {
        float CurrentExperience { get; }
        int CurrentLevel { get; set; }
        float ExperienceForLevelUp();
        void AddExperience(float value);
    }
}