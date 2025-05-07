using Code.Gameplay.Common.Visuals.StatusVisuals;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Code.Editor
{
    [CustomEditor(typeof(StatusVisuals))]
    public class StatusVisualsEditor : UnityEditor.Editor
    {
        private readonly Dictionary<string, bool> _effectStates = new();

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            StatusVisuals statusVisuals = (StatusVisuals)target;

            var effects = new Dictionary<string, Action>
            {
                { "Freeze", () => ToggleEffect(statusVisuals.ApplyFreeze, statusVisuals.UnapplyFreeze, "Freeze") },
                { "Poison", () => ToggleEffect(statusVisuals.ApplyPoison, statusVisuals.UnapplyPoison, "Poison") },
                { "Speed", () => ToggleEffect(statusVisuals.ApplySpeedUp, statusVisuals.UnapplySpeedUp, "Speed") },
                { "Max Hp", () => ToggleEffect(statusVisuals.ApplyMaxHp, statusVisuals.UnapplyMaxHp, "Max Hp") },
                { "Invulnerability", () => ToggleEffect(statusVisuals.ApplyInvulnerability, statusVisuals.UnapplyInvulnerability, "Invulnerability") }
            };

            foreach (var effect in effects)
            {
                if (GUILayout.Button(_effectStates.ContainsKey(effect.Key) && _effectStates[effect.Key] ? $"Unapply {effect.Key}" : $"Apply {effect.Key}"))
                {
                    effect.Value.Invoke();
                }
            }
        }

        private void ToggleEffect(Action apply, Action unapply, string effectName)
        {
            if (!_effectStates.ContainsKey(effectName))
            {
                _effectStates[effectName] = false;
            }

            if (_effectStates[effectName])
            {
                unapply();
            }
            else
            {
                apply();
            }

            _effectStates[effectName] = !_effectStates[effectName];
        }
    }
}