using System;
using System.Collections.Generic;

namespace Code.Gameplay.Features.Statuses.Indexing
{
    public class StatusKeyEqualityComparer : IEqualityComparer<StatusKey>
    {
        public bool Equals(StatusKey x, StatusKey y)
        {
            return x.TargetId == y.TargetId && x.TypeId == y.TypeId;
        }

        public int GetHashCode(StatusKey obj)
        {
            return HashCode.Combine(obj.TargetId, (int)obj.TypeId);
        }
    }
}