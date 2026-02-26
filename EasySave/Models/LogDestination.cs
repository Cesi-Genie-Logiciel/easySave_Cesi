using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasySave.Models
{
    // Determines where log files should be sent
    public enum LogDestination
    {
        Local,
        Centralized,
        Both
    }
}
