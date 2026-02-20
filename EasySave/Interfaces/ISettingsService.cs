using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface ISettingsService
    {
        AppSettings Load();
        void Save(AppSettings settings);
        AppSettings GetCurrent();
        void Reload();
    }
}