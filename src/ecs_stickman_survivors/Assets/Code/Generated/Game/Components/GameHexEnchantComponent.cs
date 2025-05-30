//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class GameMatcher {

    static Entitas.IMatcher<GameEntity> _matcherHexEnchant;

    public static Entitas.IMatcher<GameEntity> HexEnchant {
        get {
            if (_matcherHexEnchant == null) {
                var matcher = (Entitas.Matcher<GameEntity>)Entitas.Matcher<GameEntity>.AllOf(GameComponentsLookup.HexEnchant);
                matcher.componentNames = GameComponentsLookup.componentNames;
                _matcherHexEnchant = matcher;
            }

            return _matcherHexEnchant;
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

    static readonly Code.Gameplay.Features.Enchants.HexEnchant hexEnchantComponent = new Code.Gameplay.Features.Enchants.HexEnchant();

    public bool isHexEnchant {
        get { return HasComponent(GameComponentsLookup.HexEnchant); }
        set {
            if (value != isHexEnchant) {
                var index = GameComponentsLookup.HexEnchant;
                if (value) {
                    var componentPool = GetComponentPool(index);
                    var component = componentPool.Count > 0
                            ? componentPool.Pop()
                            : hexEnchantComponent;

                    AddComponent(index, component);
                } else {
                    RemoveComponent(index);
                }
            }
        }
    }
}
