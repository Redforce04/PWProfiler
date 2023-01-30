// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         Config.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 10:54 AM
//    Created Date:     01/30/2023 10:54 AM
// -----------------------------------------

namespace PWProfiler.Configs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class MainConfig
    {
        /// <summary>
        /// Whether the plugin is enabled or not.
        /// </summary>
        public bool Enabled { get; set; } = true;
        
        /// <summary>
        /// Whether the plugin will check memory stats.
        /// </summary>
        public bool CheckMemory { get; set; } = true;
        
        /// <summary>
        /// Whether the plugin will check cpu stats.
        /// </summary>
        public bool CheckCpu { get; set; } = true;

        /// <summary>
        /// How often the stats will refresh.
        /// </summary>
        public float StatsRefreshTime { get; set; } = 5f;
        
        /// <summary>
        /// How low the tps has to drop before the low-tps logger will begin triggering.
        /// </summary>
        public float LowTps { get; set; } = 30f;
        
        /// <summary>
        /// Whether or not the NetData Integration is enabled.
        /// </summary>
        public bool NetDataIntegrationEnabled { get; set; }
        
        /// <summary>
        /// Whether or not the plugin will log the stats and LowTps to the file.
        /// </summary>
        public bool FileLoggingEnabled { get; set; } = true;

        /// <summary>
        /// How many frames will be sampled for the average tps.
        /// </summary>
        public int AverageFrameSampleAmount { get; set; } = 120;

        // ReSharper disable once CommentTypo
        /// <summary>
        /// The name of the server to show up in netdata.
        /// </summary>
        public string ServerName { get; set; } = "Server";

    }
}