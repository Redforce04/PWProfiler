// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         LoggingInfo.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:34 PM
//    Created Date:     01/30/2023 1:34 PM
// -----------------------------------------

using System;

namespace PWProfiler.Structs
{
    struct LoggingInfo
    {
        internal DateTime DateTime;
        internal long Epoch;
        internal float AverageTps;
        internal float AverageDeltaTime;
        internal long MemoryUsage;
        internal float CpuUsage;
        internal int Players;
    }
}