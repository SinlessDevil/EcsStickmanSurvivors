using Code.Progress.Data;

namespace Code.Progress.Provider
{
    public class ProgressProvider : IProgressProvider
    {
        public ProgressData ProgressData { get; private set; }
        public EntityData EntityData => ProgressData.EntityData;

        public void SetProgressData(ProgressData data)
        {
            ProgressData = data;
        }
    }
}