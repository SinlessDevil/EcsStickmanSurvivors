using UnityEngine;

namespace Code.Gameplay.Common.Visuals.StatusVisuals
{
    [System.Serializable]
    public struct StatusEffect
    {
        public StatusEffectColor Color;
        public float OutlineSize;
        public float OutlineSmoothness;
        public bool AffectsAnimator;
        public float AnimatorSpeed;
    }
    
    [System.Serializable]
    public struct StatusEffectColor
    {
        public Color Color;
        public float Intensity;

        public StatusEffectColor(Color color, float intensity)
        {
            Color = color;
            Intensity = intensity;
        }
    }
}