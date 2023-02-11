// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         Config.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 10:54 AM
//    Created Date:     01/30/2023 10:54 AM
// -----------------------------------------

using System.ComponentModel;

namespace PWProfiler.Configs
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class MainConfig
    {
        /// <summary>
        /// Whether the plugin is enabled or not.
        /// </summary>
        [Description("Whether the plugin is enabled or not.")]
        public bool Enabled { get; set; } = true;

        /// <summary>
        /// Whether debug mod is disabled or not.
        /// </summary>
        [Description("Whether debug mod is disabled or not.")]
        public bool Debug { get; set; } = false;
        
        /// <summary>
        /// Whether the plugin will check memory stats.
        /// </summary>
        [Description("Whether the plugin will check memory stats.")]
        public bool CheckMemory { get; set; } = true;
        
        /// <summary>
        /// Whether the plugin will check cpu stats.
        /// </summary>
        [Description("Whether the plugin will check cpu stats.")]
        public bool CheckCpu { get; set; } = true;

        /// <summary>
        /// How often the stats will refresh.
        /// </summary>
        [Description("How often the stats will refresh.")]
        public float StatsRefreshTime { get; set; } = 5f;
        
        /// <summary>
        /// How low the tps has to drop before the low-tps logger will begin triggering.
        /// </summary>
        [Description("How low the tps has to drop before the low-tps logger will begin triggering.")]
        public float LowTps { get; set; } = 30f;
        
        /// <summary>
        /// Whether or not the NetData Integration is enabled.
        /// </summary>
        [Description("Whether or not the NetData Integration is enabled.")]
        public bool NetDataIntegrationEnabled { get; set; }
        
        /// <summary>
        /// The address and port that the NetData integration is hosted on. Ie: localhost:11011
        /// </summary>
        [Description($"The address and port that the NetData integration is hosted on. Ie: localhost:11011")]
        public string NetDataIntegrationAddress { get; set; }
        
        /// <summary>
        /// Whether or not the plugin will log the stats and LowTps to the file.
        /// </summary>
        [Description("Whether or not the plugin will log the stats and LowTps to the file.")]
        public bool FileLoggingEnabled { get; set; } = true;

        /// <summary>
        /// The location of the log files if File Logging is enabled.
        /// </summary>
        [Description("The location of the log files if File Logging is enabled.")]
        public string FileLoggingLocation { get; set; } = "/home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/PWProfiler/";

        /// <summary>
        /// How many frames will be sampled for the average tps.
        /// </summary>
        [Description("How many frames will be sampled for the average tps.")]
        public int AverageFrameSampleAmount { get; set; } = 120;

        // ReSharper disable once CommentTypo
        /// <summary>
        /// The name of the server to show up in netdata.
        /// </summary>
        [Description("The name of the server to show up in netdata.")]
        public string ServerName { get; set; } = "Server";
        
        /// <summary>
        /// Used to communicate with the NetData integration and verify that this is a registered server with permission to update stats.
        /// </summary>
        [Description("Used to communicate with the NetData integration and verify that this is a registered server with permission to update stats.")]
        public string ApiKey { get; set; }

    }
}