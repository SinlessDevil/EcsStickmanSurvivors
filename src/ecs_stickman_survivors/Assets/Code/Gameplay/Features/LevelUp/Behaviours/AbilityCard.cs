using System;
using System.Collections;
using Code.Gameplay.Features.Abilities;
using Code.Gameplay.Features.Abilities.Configs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Gameplay.Features.LevelUp.Behaviours
{
    public class AbilityCard : MonoBehaviour
    {
        public AbilityId AbilityId;
        public Image Icon;
        public TextMeshProUGUI Description;
        public Button Button;
        public GameObject Stamp;
        
        private Action<AbilityId> _onSelected;

        public void Setup(AbilityId abilityId, AbilityLevel abilityLevel, Action<AbilityId> onSelected)
        {
            AbilityId = abilityId;
            Icon.sprite = abilityLevel.Icon;
            Description.text = abilityLevel.Description;
            
            _onSelected = onSelected;
            
            Button.onClick.AddListener(OnSelected);
        }

        private void OnDestroy()
        {
            Button.onClick.RemoveListener(OnSelected);
        }

        private void OnSelected()
        {
            StartCoroutine(StampAndReportRoutine());
        }

        private IEnumerator StampAndReportRoutine()
        {
            Stamp.SetActive(true);
            yield return new WaitForSeconds(1f);
            Stamp.SetActive(false);
            
            _onSelected?.Invoke(AbilityId);
        }
    }
}