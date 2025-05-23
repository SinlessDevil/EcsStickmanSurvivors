//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherArmamentBombVisual;

    public static Entitas.IMatcher<GameEntity> ArmamentBombVisual {
        get {
            if (_matcherArmamentBombVisual == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.ArmamentBombVisual);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherArmamentBombVisual = matcher;
            }

            return _matcherArmamentBombVisual;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class GameEntity {

    public Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent armamentBombVisual { get { return (Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent)GetComponent(GameComponentsLookup.ArmamentBombVisual); } }
    public Code.Gameplay.Features.Armaments.Behaviours.IArmamentBombVisuals ArmamentBombVisual { get { return armamentBombVisual.Value; } }
    public bool hasArmamentBombVisual { get { return HasComponent(GameComponentsLookup.ArmamentBombVisual); } }

    public GameEntity AddArmamentBombVisual(Code.Gameplay.Features.Armaments.Behaviours.IArmamentBombVisuals newValue) {
        var index = GameComponentsLookup.ArmamentBombVisual;
        var component = (Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent)CreateComponent(index, typeof(Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent));
        component.Value = newValue;
        AddComponent(index, component);
        return this;
    }

    public GameEntity ReplaceArmamentBombVisual(Code.Gameplay.Features.Armaments.Behaviours.IArmamentBombVisuals newValue) {
        var index = GameComponentsLookup.ArmamentBombVisual;
        var component = (Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent)CreateComponent(index, typeof(Code.Gameplay.Features.Armaments.ArmamentBombVisualComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
        return this;
    }

    public GameEntity RemoveArmamentBombVisual() {
        RemoveComponent(GameComponentsLookup.ArmamentBombVisual);
        return this;
    }
}
