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
                LowTpsWarnCount = lowTps,
                ApiVersion = PWProfiler.ApiVersion,
                PluginVersion = $"{AssemblyInfo.CommitHash}-{AssemblyInfo.CommitBranch}",
                ApiKey = Config.ApiKey
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

                if (!resp.IsSuccessStatusCode)
                {
                    Log.Error("Status code was non successful.");
                }
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