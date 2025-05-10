using Code.Gameplay.Input.Service;
using Entitas;
using UnityEngine;

namespace Code.Gameplay.Input.Systems
{
    public class EmitInputSystem : IExecuteSystem
    {
        private readonly IInputService _inputService;
        private readonly IGroup<InputEntity> _inputs;

        public EmitInputSystem(InputContext input, IInputService inputService)
        {
            _inputService = inputService;
            _inputs = input.GetGroup(InputMatcher.Input);
        }

        public void Execute()
        {
            foreach (InputEntity input in _inputs)
            {
                if (_inputService.HasAxisInput())
                    input.ReplaceAxisInput(new Vector3(_inputService.GetHorizontalAxis(),
                        0, _inputService.GetVerticalAxis()));
                else if (input.hasAxisInput)
                    input.RemoveAxisInput();
            }
        }
    }
}