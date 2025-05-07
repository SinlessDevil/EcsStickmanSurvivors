using Code.Gameplay.Features.Visuals.Behaviors;
using Code.Gameplay.Features.Visuals.Factory;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Code.Gameplay.Common.Visuals.StatusVisuals
{
    public class StatusVisuals : MonoBehaviour, IStatusVisuals
    {
        private static readonly int ColorProperty = Shader.PropertyToID("_Color");
        private static readonly int ColorIntensityProperty = Shader.PropertyToID("_Intensity");
        private static readonly int OutlineSizeProperty = Shader.PropertyToID("_OutlineSize");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineSmoothnessProperty = Shader.PropertyToID("_OutlineSmoothness");

        public SpriteRenderer Renderer;
        public Animator Animator;
        public Transform ParentVisual;

        public StatusEffect FreezeEffect = new()
        {
            Color = new StatusEffectColor(new Color32(56, 163, 190, 255), 1f),
            OutlineSize = 3,
            OutlineSmoothness = 8,
            AffectsAnimator = true,
            AnimatorSpeed = 0
        };

        public StatusEffectColor PoisonEffect = new(new Color32(0, 255, 0, 255), 0.6f);
        public StatusEffectColor SpeedEffect = new(new Color32(255, 255, 0, 255), 0.6f);
        public StatusEffectColor MaxHpEffect = new(new Color32(255, 0, 0, 255), 0.6f);

        public StatusEffect InvulnerabilityEffect = new()
        {
            Color = new StatusEffectColor(new Color32(255, 255, 255, 255), 1f),
            OutlineSize = 3,
            OutlineSmoothness = 8,
            AffectsAnimator = true,
            AnimatorSpeed = 0
        };

        private Sheep _sheep;
        private IVisualFactory _visualFactory;
        
        [Inject]
        private void Construct(IVisualFactory visualFactory)
        {
            _visualFactory = visualFactory;
        }
        
        private void ApplyEffect(StatusEffect effect)
        {
            Renderer.material.SetColor(OutlineColorProperty, effect.Color.Color);
            Renderer.material.SetFloat(OutlineSizeProperty, effect.OutlineSize);
            Renderer.material.SetFloat(OutlineSmoothnessProperty, effect.OutlineSmoothness);

            if (effect.AffectsAnimator)
            {
                Animator.speed = effect.AnimatorSpeed;
            }
        }

        private void ApplyEffect(StatusEffectColor effectColor)
        {
            Renderer.material.SetColor(ColorProperty, effectColor.Color);
            Renderer.material.SetFloat(ColorIntensityProperty, effectColor.Intensity);
        }

        private void UnapplyEffect()
        {
            Renderer.material.SetColor(OutlineColorProperty, Color.white);
            Renderer.material.SetFloat(OutlineSizeProperty, 0f);
            Renderer.material.SetFloat(OutlineSmoothnessProperty, 0f);
            Renderer.material.SetFloat(ColorIntensityProperty, 0f);
            Animator.speed = 1;
        }

        public void ApplyFreeze() => ApplyEffect(FreezeEffect);
        public void UnapplyFreeze() => UnapplyEffect();

        public void ApplyPoison() => ApplyEffect(PoisonEffect);
        public void UnapplyPoison() => UnapplyEffect();

        public void ApplySpeedUp() => ApplyEffect(SpeedEffect);

        public void UnapplySpeedUp() => UnapplyEffect();

        public void ApplyMaxHp()
        {
            ApplyEffect(MaxHpEffect);

            transform.parent.DOScale(1.5f, 0.5f)
                .SetEase(Ease.Linear);
        }

        public void UnapplyMaxHp()
        {
            UnapplyEffect();
            
            transform.parent.DOScale(1f, 0.5f)
                .SetEase(Ease.Linear);
        }

        public void ApplyInvulnerability() => ApplyEffect(InvulnerabilityEffect);
        public void UnapplyInvulnerability() => UnapplyEffect();

        public void ApplyHex()
        {
            if(_sheep != null)
                return;
            
            var sheep = _visualFactory.CreateSheep(Vector3.zero, ParentVisual);
            _sheep = sheep;
            
            _sheep.PlayBouncing();
            
            Renderer.enabled = false;
        }

        public void UnapplyHex()
        {
            Destroy(_sheep.gameObject);
            _sheep = null;
            
            Renderer.enabled = true;
        }
    }
}
