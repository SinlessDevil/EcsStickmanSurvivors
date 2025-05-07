namespace Code.Gameplay.Features.Statuses.Applier
{
    public interface IStatusApplier
    {
        GameEntity ApplyStatusOnTarget(StatusSetup setup, int producerId, int targetId);
        GameEntity ApplyStatusOnProducer(StatusSetup setup, int producerId, int targetId);
    }
}