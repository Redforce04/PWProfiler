// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         LowTps.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:35 PM
//    Created Date:     01/30/2023 1:35 PM
// -----------------------------------------

using System;

namespace PWProfiler.Structs
{
    struct LowTps
    {
        internal DateTime DateTime;
        internal long Epoch;
        internal int InstanceNumber;
        internal float Tps;
        internal float DeltaTime;
        internal int Players;
    }
}