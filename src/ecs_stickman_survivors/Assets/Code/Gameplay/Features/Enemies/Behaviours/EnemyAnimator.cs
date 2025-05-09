using Code.Gameplay.Common.Visuals;
using Code.Gameplay.Common.Visuals.StatusVisuals;
using UnityEngine;
using DG.Tweening;

namespace Code.Gameplay.Features.Enemies.Behaviours
{
    public class EnemyAnimator : MonoBehaviour, IDamageTakenAnimator
    {
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthProperty = Shader.PropertyToID("_OutlineWidth");
        private static readonly int RimColorProperty = Shader.PropertyToID("_RimColor");

        private readonly int _isMovingHash = Animator.StringToHash("isMoving");
        private readonly int _diedHash = Animator.StringToHash("died");

        public Animator Animator;
        public SkinnedMeshRenderer Renderer;

        private Material Material => Renderer.materials[1];

        public Color DamageColor = Color.red;
        public Color HealColor = Color.green;

        public void PlayMove() => Animator.SetBool(_isMovingHash, true);
        public void PlayIdle() => Animator.SetBool(_isMovingHash, false);
        public void PlayDied() => Animator.SetTrigger(_diedHash);

        public void PlayDamageTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            Material.DOColor(DamageColor, OutlineColorProperty, 0.1f)
                .OnComplete(() =>
                {
                    Material.DOColor(Color.black, OutlineColorProperty, 0.1f);
                });

            Material.DOFloat(0.4f, OutlineWidthProperty, 0.1f)
                .OnComplete(() =>
                {
                    Material.DOFloat(0.32f, OutlineWidthProperty, 0.1f);
                });
        }

        public void PlayHealTaken()
        {
            if (DOTween.IsTweening(Material))
                return;

            Material.DOColor(HealColor, RimColorProperty, 0.15f)
                .OnComplete(() =>
                {
                    Material.DOColor(Color.white, RimColorProperty, 0.15f);
                });

            Material.DOFloat(0.45f, OutlineWidthProperty, 0.15f)
                .OnComplete(() =>
                {
                    Material.DOFloat(0.32f, OutlineWidthProperty, 0.15f);
                });
        }

        public void ResetAll()
        {
            Animator.ResetTrigger(_diedHash);
        }
    }
}