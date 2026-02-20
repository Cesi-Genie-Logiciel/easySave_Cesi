using System;
using System.Collections.Generic;
using EasySave.Models;

namespace EasySave.Interfaces
{
    public interface IBackupService
    {
        void CreateBackupJob(string name, string source, string target, string type);
        List<BackupJob> GetAllBackupJobs();
        void ExecuteBackupJob(int index);
        void ExecuteMultipleBackupJobs(List<int> indices);
        void DeleteBackupJob(int index);
        void UpdateBackupJob(int index, string name, string source, string target, string type);
    }
}