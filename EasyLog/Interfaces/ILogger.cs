using System.Collections.Generic;

namespace EasyLog.Interfaces
{
	public interface ILogger
	{
		void LogFileTransfer(string backupName, string sourceFile, string destFile, long fileSize, double duration);
		void UpdateState(object state);
		void UpdateState(List<object> states);
	}
}