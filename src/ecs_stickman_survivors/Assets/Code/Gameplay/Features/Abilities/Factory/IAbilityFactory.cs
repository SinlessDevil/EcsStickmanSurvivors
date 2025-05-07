namespace Code.Gameplay.Features.Abilities.Factory
{
    public interface IAbilityFactory
    {
        GameEntity CreateVegetableBoltAbility(int level);
        GameEntity CreateRadiatingCogBoltAbility(int level);
        GameEntity CreateBouncingCoinBoltAbility(int level);
        GameEntity CreateScatteringRuneStoneBolt(int level);
        GameEntity CreateOrbitingMushroomBolt(int level);
        
        GameEntity CreateGarlicAuraAbility();
        GameEntity CreateBombBolt(int level);
    }
}