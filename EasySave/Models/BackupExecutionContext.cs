using System;
using System.Threading;

namespace EasySave.Models
{
    /// <summary>
    /// V3/P1 (step 2): Minimal execution context for orchestrated backup runs.
    ///
    /// IMPORTANT:
    /// - This is a placeholder for future P2/P3/P4 rules (priority, pause/stop, business software auto-pause, etc.).
    /// - In P1 we don't enforce any rule here; we only carry data.
    /// </summary>
    public sealed class BackupExecutionContext
    {
        public Guid ExecutionId { get; }
        public DateTimeOffset CreatedAt { get; }
        public CancellationToken CancellationToken { get; }

        public BackupExecutionContext(CancellationToken cancellationToken = default)
        {
            ExecutionId = Guid.NewGuid();
            CreatedAt = DateTimeOffset.Now;
            CancellationToken = cancellationToken;
        }
    }
}

