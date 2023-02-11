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
    public struct NetDataPacket
    {
        public int Port;
        public string ServerName;
        public float RefreshSpeed;
        public long Epoch;
        public float AverageTps;
        public float AverageDeltaTime;
        public ulong MemoryUsage;
        public float CpuUsage;
        public int Players;
        public int LowTpsWarnCount;
        public string ApiKey;
        public string ApiVersion;
        public string PluginVersion;
        
    }
}