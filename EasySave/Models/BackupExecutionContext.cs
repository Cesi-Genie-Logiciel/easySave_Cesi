using System;
using System.Collections.Generic;
using System.Threading;
using EasySave.Coordination;

namespace EasySave.Models
{
    /// <summary>
    /// V3/P1 (step 2): Execution context for orchestrated backup runs.
    /// PauseEvent, PriorityManager, BandwidthGuard, ExtensionsToEncrypt are optional (conformité schéma).
    /// </summary>
    public sealed class BackupExecutionContext
    {
        public Guid ExecutionId { get; }
        public DateTimeOffset CreatedAt { get; }
        public CancellationToken CancellationToken { get; }
        public ManualResetEventSlim? PauseEvent { get; set; }
        public PriorityFileManager? PriorityManager { get; set; }
        public LargeFileTransferGuard? BandwidthGuard { get; set; }
        public List<string>? ExtensionsToEncrypt { get; set; }

        public BackupExecutionContext(CancellationToken cancellationToken = default)
        {
            ExecutionId = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
            CancellationToken = cancellationToken;
        }
    }
}

