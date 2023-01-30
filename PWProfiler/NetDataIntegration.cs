// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         NetDataIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:37 PM
//    Created Date:     01/30/2023 1:37 PM
// -----------------------------------------

using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using PluginAPI.Core;
using PWProfiler.Configs;
using PWProfiler.Structs;

namespace PWProfiler
{
    public class NetDataIntegration
    {
        public static NetDataIntegration Singleton;
        private static MainConfig Config => PWProfiler.Singleton.Config;
        
        /// <summary>
        /// The directory containing the NetData Integration connector file
        /// </summary>
        private string NetDataLogLocation { get; set; }

        /// <summary>
        /// The Path to the NetData Connector File.
        /// </summary>
        private string NetDataConnectorLog { get; set; }
        
        internal NetDataPacket? LastPacket;

        /// <summary>
        /// 
        /// </summary>
        public NetDataIntegration()
        {
            Singleton = this;
            // Initialize the paths for the NetData integration.
            _setNetDataIntegrationFileLocation();
            // Initialize the files and directories for the net data integration they don't already exist.
            _initNetDataIntegrationDirectory();
        }
        
        /// <summary>
        /// Initialize the paths for the NetData integration.
        /// </summary>
        private void _setNetDataIntegrationFileLocation()
        {
            NetDataLogLocation = Path.GetTempPath() + "PwProfiler/";
            NetDataConnectorLog = NetDataLogLocation + $"Server-{ServerStatic.ServerPort}";
        }
        
        /// <summary>
        /// Initialize the files and directories for the net data integration they don't already exist.
        /// </summary>
        private void _initNetDataIntegrationDirectory()
        {
            try
            {
                if (!Directory.Exists(NetDataLogLocation))
                    Directory.CreateDirectory(NetDataLogLocation);
                if (!File.Exists(NetDataConnectorLog))
                    File.Create(NetDataConnectorLog);

                Log.Debug($"NetData Integration Enabled. NetData Integration Location: \'{NetDataLogLocation}\'");
            }
            catch (Exception e)
            {
                Log.Error(
                    $"Could not create or load the NetData Integration Files. NetData Integration will be disabled.\n{e}");
                Config.NetDataIntegrationEnabled = false;
            }

        }
        
        /// <summary>
        /// Sends information to the NetData integration.
        /// </summary>
        /// <param name="log"></param>
        /// <param name="lowTps"></param>
        internal void SendLowTpsToNetDataIntegration(LoggingInfo log, int lowTps)
        {
            NetDataPacket packet = new NetDataPacket()
            {
                Port = Server.Port,
                ServerName = Config.ServerName,
                RefreshSpeed = Config.StatsRefreshTime,
                Epoch = log.Epoch,
                AverageTps = log.AverageTps,
                AverageDeltaTime = log.AverageDeltaTime,
                CpuUsage = log.CpuUsage,
                MemoryUsage = log.MemoryUsage,
                Players = log.Players,
                LowTpsWarnCount = lowTps
            };
            LastPacket = packet;
            string json = JsonConvert.SerializeObject(packet);
            using (FileStream fs = new FileStream(NetDataConnectorLog, FileMode.Append, FileAccess.Write, FileShare.Write))
            {
                StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                sw.Write(json);
                sw.Close();
                fs.Close();
            }
        }
    }
}