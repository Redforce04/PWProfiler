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
    public struct LoggingInfo
    {
        internal DateTime DateTime;
        internal ulong Epoch;
        internal float AverageTps;
        internal float AverageDeltaTime;
        internal ulong MemoryUsage;
        internal float CpuUsage;
        internal int Players;
    }
}