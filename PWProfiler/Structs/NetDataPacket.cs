// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         NetDataPacket.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:34 PM
//    Created Date:     01/30/2023 1:34 PM
// -----------------------------------------

namespace PWProfiler.Structs
{
    internal struct NetDataPacket
    {
        internal int Port;
        internal string ServerName;
        internal float RefreshSpeed;
        internal long Epoch;
        internal float AverageTps;
        internal float AverageDeltaTime;
        internal long MemoryUsage;
        internal float CpuUsage;
        internal int Players;
        internal int LowTpsWarnCount;
    }
}