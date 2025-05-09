using UnityEngine;

namespace Code.Gameplay.Common.Visuals.StatusVisuals
{
    [System.Serializable]
    public class StatusEffect
    {
        public Color OutlineColor = Color.black;
        public float OutlineWidth = 0f;
        public Color RimColor = Color.black;
        public float RimStep = 0.5f;
        public float RimStepSmooth = 0.2f;
        public bool AffectsAnimator = false;
        public float AnimatorSpeed = 1f;
    }
}