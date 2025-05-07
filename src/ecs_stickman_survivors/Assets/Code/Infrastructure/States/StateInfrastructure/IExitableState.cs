using RSG;

namespace Code.Infrastructure.States.StateInfrastructure
{
    public interface IExitableState
    {
        IPromise BeginExit();
        void EndExit();
    }
}