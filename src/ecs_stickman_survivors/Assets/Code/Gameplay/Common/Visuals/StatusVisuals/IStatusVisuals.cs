namespace Code.Gameplay.Common.Visuals.StatusVisuals
{
    public interface IStatusVisuals
    {
        void ApplyFreeze();
        void UnapplyFreeze();
        
        void ApplyPoison();
        void UnapplyPoison();
        
        void ApplySpeedUp();
        void UnapplySpeedUp();
        
        void ApplyMaxHp();
        void UnapplyMaxHp();
        
        void ApplyInvulnerability();
        void UnapplyInvulnerability();
        
        void ApplyHex();
        void UnapplyHex();
    }
}