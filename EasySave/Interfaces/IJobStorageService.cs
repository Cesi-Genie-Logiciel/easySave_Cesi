using System.Collections.Generic;
using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface IJobStorageService
    {
        List<BackupConfig> LoadJobs();
        void SaveJobs(List<BackupConfig> jobs);
    }
}