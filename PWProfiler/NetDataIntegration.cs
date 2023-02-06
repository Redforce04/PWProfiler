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
using System.Net.Http;
using Newtonsoft.Json;
using PluginAPI.Core;
using PWProfiler.Configs;
using PWProfiler.Structs;
using NetDataPacket = PWProfiler.Structs.NetDataPacket;

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
        internal void SendInfoToNetDataIntegration(LoggingInfo log, int lowTps)
        {

            NetDataPacket packet = new NetDataPacket()
            {
                Port = Server.Port,
                ServerName = Config.ServerName,
                RefreshSpeed = Config.StatsRefreshTime,
                Epoch = (long)log.Epoch,
                AverageTps = log.AverageTps,
                AverageDeltaTime = log.AverageDeltaTime,
                CpuUsage = log.CpuUsage,
                MemoryUsage = log.MemoryUsage,
                Players = log.Players,
                LowTpsWarnCount = lowTps
            };
            string json = JsonConvert.SerializeObject(packet);
            LastPacket = packet;
            HttpResponseMessage resp = null;
            string response = string.Empty;
            try
            {

                using (HttpClient client = new HttpClient())
                {
                    //client.DefaultRequestHeaders.Add("ApiKey", Add api key here);
                    HttpContent content = new StringContent(json);
                    resp = client.PostAsync("http://" + NetDataIntegration.Config.NetDataIntegrationAddress + "/packet",
                        content).Result;
                    response = resp.Content.ReadAsStringAsync().Result;
                }

                Log.Debug($"Response: {response} ({resp}) \n {resp.ReasonPhrase}");
            }
            catch (Exception e)
            {
                if (e is AggregateException)
                {
                    Log.Error(
                        $"An error has occured while trying to send data to the server. Check that it is running properly.");
                    Log.Debug($"{e}", Config.Debug);
                }
                else
                {
                    Log.Error($"An error has occured while trying to send data. ");

                }
            }
        }
    }
}