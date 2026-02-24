using System;
using System.Collections.Generic;
using System.Threading;

namespace EasySave.Models
{
    // Carries the runtime information needed during a backup execution.
    // The cancellation token handles Stop, the pause event handles Pause/Resume.
    // ManualResetEventSlim starts signaled (true = not paused).
    // Calling Reset() blocks threads that call Wait() = pause.
    // Calling Set() unblocks them = resume.
    public sealed class BackupExecutionContext
    {
        public Guid ExecutionId { get; }
        public CancellationToken Token { get; }
        public ManualResetEventSlim PauseEvent { get; }
        public List<string> ExtensionsToEncrypt { get; }

        public BackupExecutionContext(
            CancellationToken token,
            ManualResetEventSlim pauseEvent,
            List<string> extensionsToEncrypt)
        {
            ExecutionId = Guid.NewGuid();
            Token = token;
            PauseEvent = pauseEvent;
            ExtensionsToEncrypt = extensionsToEncrypt ?? new List<string>();
        }

        // Simplified constructor for CLI sequential mode where pause/stop is not needed
        public BackupExecutionContext(CancellationToken token = default)
        {
            ExecutionId = Guid.NewGuid();
            Token = token;
            PauseEvent = new ManualResetEventSlim(true);
            ExtensionsToEncrypt = new List<string>();
        }
    }
}

