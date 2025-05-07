namespace Code.Gameplay.Features.CharacterStats.Indexing
{
    public struct StatKey
    {
        public readonly int TargetId;
        public readonly Stats Stats;

        public StatKey(int targetId, Stats stats)
        {
            Stats = stats;
            TargetId = targetId;
        }
    }
}