using System.Collections.Generic;

namespace Code.Gameplay.Features.CharacterStats.Indexing
{
    public class StatKeyEqualityComparer : IEqualityComparer<StatKey>
    {
        public bool Equals(StatKey x, StatKey y)
        {
            return x.TargetId == y.TargetId && x.Stats == y.Stats;
        }

        public int GetHashCode(StatKey obj)
        {
            return obj.TargetId.GetHashCode() ^ obj.Stats.GetHashCode();
        }
    }
}