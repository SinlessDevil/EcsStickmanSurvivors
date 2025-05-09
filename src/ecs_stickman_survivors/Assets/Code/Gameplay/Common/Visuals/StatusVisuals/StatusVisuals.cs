using Code.Gameplay.Features.Visuals.Behaviors;
using Code.Gameplay.Features.Visuals.Factory;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Code.Gameplay.Common.Visuals.StatusVisuals
{
    public class StatusVisuals : MonoBehaviour, IStatusVisuals
    {
        private static readonly int OutlineColorID = Shader.PropertyToID("_OutlineColor");
        private static readonly int OutlineWidthID = Shader.PropertyToID("_OutlineWidth");
        private static readonly int RimColorID = Shader.PropertyToID("_RimColor");
        private static readonly int RimStepID = Shader.PropertyToID("_RimStep");
        private static readonly int RimStepSmoothID = Shader.PropertyToID("_RimStepSmooth");

        public SkinnedMeshRenderer Renderer;
        public Animator Animator;
        public Transform ParentVisual;

        public StatusEffect FreezeEffect = new()
        {
            OutlineColor = new Color32(56, 163, 190, 255),
            OutlineWidth = 0.35f,
            RimColor = Color.cyan,
            RimStep = 0.7f,
            RimStepSmooth = 0.3f,
            AffectsAnimator = true,
            AnimatorSpeed = 0
        };

        public StatusEffect SpeedEffect = new()
        {
            OutlineColor = new Color32(255, 255, 0, 255),
            OutlineWidth = 0.35f,
            RimColor = Color.yellow,
            RimStep = 0.65f,
            RimStepSmooth = 0.25f,
            AffectsAnimator = false
        };

        public StatusEffect InvulnerabilityEffect = new()
        {
            OutlineColor = Color.white,
            OutlineWidth = 0.35f,
            RimColor = Color.white,
            RimStep = 0.8f,
            RimStepSmooth = 0.35f,
            AffectsAnimator = true,
            AnimatorSpeed = 0
        };

        public StatusEffect PoisonEffect = new()
        {
            OutlineColor = new Color32(0, 255, 0, 255),
            OutlineWidth = 0.35f,
            RimColor = new Color32(0, 200, 0, 255),
            RimStep = 0.6f,
            RimStepSmooth = 0.2f,
            AffectsAnimator = false
        };
        
        public StatusEffect MaxHpEffect = new()
        {
            OutlineColor = new Color32(255, 0, 0, 255),
            OutlineWidth = 0.4f,
            RimColor = new Color32(255, 80, 80, 255),
            RimStep = 0.75f,
            RimStepSmooth = 0.3f,
            AffectsAnimator = false
        };
        
        private Material _material;
        private Sheep _sheep;
        private IVisualFactory _visualFactory;

        [Inject]
        private void Construct(IVisualFactory visualFactory)
        {
            _visualFactory = visualFactory;
        }

        private void Awake()
        {
            SetUpMaterial();
        }

        private void SetUpMaterial()
        {
            _material = Renderer.materials[1];
        }
        
        private void ApplyEffect(StatusEffect effect)
        {
            if(_material == null)
                SetUpMaterial();
            
            _material.SetColor(OutlineColorID, effect.OutlineColor);
            _material.SetFloat(OutlineWidthID, effect.OutlineWidth);
            _material.SetColor(RimColorID, effect.RimColor);
            _material.SetFloat(RimStepID, effect.RimStep);
            _material.SetFloat(RimStepSmoothID, effect.RimStepSmooth);

            if (effect.AffectsAnimator)
                Animator.speed = effect.AnimatorSpeed;
        }

        private void UnapplyEffect()
        {
            if(_material == null)
                SetUpMaterial();
            
            _material.SetFloat(OutlineWidthID, 0f);
            _material.SetColor(OutlineColorID, Color.black);
            _material.SetColor(RimColorID, Color.black);
            _material.SetFloat(RimStepID, 0.672f);
            _material.SetFloat(RimStepSmoothID, 0.29f);
            
            Animator.speed = 1f;
        }

        public void ApplyFreeze() => ApplyEffect(FreezeEffect);
        public void UnapplyFreeze() => UnapplyEffect();
        
        public void ApplyPoison() => ApplyEffect(PoisonEffect);

        public void UnapplyPoison() => UnapplyEffect();

        public void ApplySpeedUp() => ApplyEffect(SpeedEffect);
        public void UnapplySpeedUp() => UnapplyEffect();

        public void ApplyInvulnerability() => ApplyEffect(InvulnerabilityEffect);
        public void UnapplyInvulnerability() => UnapplyEffect();

        public void ApplyMaxHp()
        {
            ApplyEffect(MaxHpEffect);
            ParentVisual.DOScale(1.5f, 0.5f).SetEase(Ease.Linear);
        }

        public void UnapplyMaxHp()
        {
            UnapplyEffect();
            ParentVisual.DOScale(1f, 0.5f).SetEase(Ease.Linear);
        }

        public void ApplyHex()
        {
            if (_sheep != null) 
                return;
            
            _sheep = _visualFactory.CreateSheep(Vector3.zero, ParentVisual);
            _sheep.PlayBouncing();
            Renderer.enabled = false;
        }

        public void UnapplyHex()
        {
            if (_sheep == null) 
                return;
            
            Destroy(_sheep.gameObject);
            _sheep = null;
            Renderer.enabled = true;
        }
    }
}
