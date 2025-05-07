namespace Code.Progress.SaveLoad
{
    public interface ISaveLoadService
    {
        void SaveProgress();
        void LoadProgress();
        void CreateProgress();
        bool HasSaveProgress { get; }
    }
}