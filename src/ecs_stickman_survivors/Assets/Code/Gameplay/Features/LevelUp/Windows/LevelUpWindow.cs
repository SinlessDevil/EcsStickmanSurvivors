using System.Collections;
using System.Collections.Generic;
using Code.Common.Entity;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Configs;
using Code.Gameplay.Features.Abilities.Upgrade;
using Code.Gameplay.Features.LevelUp.Behaviours;
using Code.Gameplay.Features.LevelUp.UIFactory;
using Code.Gameplay.StaticData;
using Code.Gameplay.Windows;
using Code.Gameplay.Windows.Services;
using UnityEngine;
using Zenject;
using DG.Tweening;

namespace Code.Gameplay.Features.LevelUp.Windows
{
    public class LevelUpWindow : BaseWindow
    {
        public Transform AbilityLayout;

        private IAbilityUIFactory _abilityFactory;
        private IAbilityUpgradeService _abilityUpgradeService;
        private IStaticDataService _staticDataService;
        private IWindowService _windowService;
        
        [Inject]
        private void Construct(
            IAbilityUIFactory abilityUIFactory, 
            IAbilityUpgradeService abilityUpgradeService,
            IStaticDataService staticDataService,
            IWindowService windowService)
        {
            Id = WindowId.LevelUpWindow;

            _abilityFactory = abilityUIFactory;
            _abilityUpgradeService = abilityUpgradeService;
            _staticDataService = staticDataService;
            _windowService = windowService;
        }

        protected override void Initialize()
        {
            List<AbilityCard> abilityCards = new List<AbilityCard>();

            foreach (AbilityUpgradeOption variableOption in _abilityUpgradeService.GetUpgradeOptions())
            {
                AbilityLevel abilityLevel = _staticDataService.GetAbilityLevel(variableOption.Id, variableOption.Level);

                AbilityCard abilityCard = _abilityFactory.CreateAbilityCard(AbilityLayout);
                abilityCard.Setup(variableOption.Id, abilityLevel, OnSelected);
                abilityCards.Add(abilityCard);
            }

            StartCoroutine(AnimateAbilityCards(abilityCards));
        }

        private IEnumerator AnimateAbilityCards(List<AbilityCard> cards)
        {
            float delay = 0.1f;

            foreach (var card in cards)
            {
                card.transform.localScale = Vector3.zero;
            }
            
            foreach (var card in cards)
            {
                card.transform.DOScale(Vector3.one, 0.5f)
                    .SetEase(Ease.OutBack);
                yield return new WaitForSeconds(delay);
            }
        }
        
        private void OnSelected(AbilityId id)
        {
            CreateEntity.Empty()
                .AddAbilityId(id)
                .isUpgradeRequest = true;
            
            _windowService.Close(Id);
        }
    }
}