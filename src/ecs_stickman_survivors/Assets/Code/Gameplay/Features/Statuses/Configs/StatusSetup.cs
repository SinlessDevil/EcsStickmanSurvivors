using System;

namespace Code.Gameplay.Features.Statuses
{
    [Serializable]
    public class StatusSetup
    {
        public StatusTypeId StatusTypeId;
        public float Value;
        public float Duration;
        public float Period;
        public StatusApplicationTypeId StatusApplicationTypeId;
        public StatusTriggerConditionTypeId StatusTriggerConditionTypeId;
        public bool IsStackable;
    }
}