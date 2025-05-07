using Code.Gameplay.Common.Visuals;
using Code.Gameplay.Common.Visuals.StatusVisuals;
using UnityEngine;
using DG.Tweening;

namespace Code.Gameplay.Features.Enemies.Behaviours
{
    public class EnemyAnimator : MonoBehaviour, IDamageTakenAnimator
    {
        private static readonly int OverlayIntensityProperty = Shader.PropertyToID("_OverlayIntensity");
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        
        private readonly int _isMovingHash = Animator.StringToHash("isMoving");
        private readonly int _diedHash = Animator.StringToHash("died");

        public Animator Animator;
        public SpriteRenderer SpriteRenderer;
        
        public StatusEffectColor HealEffectColor = new(new Color32(0, 255, 0, 255), 0.6f);
        
        private Material Material => SpriteRenderer.material;

        public void PlayMove() => Animator.SetBool(_isMovingHash, true);
        public void PlayIdle() => Animator.SetBool(_isMovingHash, false);
        
        public void PlayDied() => Animator.SetTrigger(_diedHash);

        public void PlayDamageTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            Material.DOFloat(0.4f, OverlayIntensityProperty, 0.15f)
                .OnComplete(() =>
                {
                    if (SpriteRenderer)
                        Material.DOFloat(0, OverlayIntensityProperty, 0.15f);
                });
        }

        public void PlayHealTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            Material.DOFloat(HealEffectColor.Intensity, OverlayIntensityProperty, 0.15f)
                .OnComplete(() =>
                {
                    if (SpriteRenderer)
                        Material.DOFloat(0, OverlayIntensityProperty, 0.15f);
                });
            
            Material.DOColor(HealEffectColor.Color, ColorProperty, 0.15f)
                .OnComplete(() =>
                {
                    if (SpriteRenderer)
                        Material.DOColor(Color.white, ColorProperty, 0.15f);
                });
        }
        
        public void ResetAll()
        {
            Animator.ResetTrigger(_diedHash);
        }
    }
}