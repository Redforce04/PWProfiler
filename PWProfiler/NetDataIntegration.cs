// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         NetDataIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:37 PM
//    Created Date:     01/30/2023 1:37 PM
// -----------------------------------------

using System;
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
        
        internal NetDataPacket? LastPacket;

        private DateTime? _lastError;

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
            try
            {
                HttpResponseMessage resp;
                string response;
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
                    Log.Error($"Status code was non successful. {response} {resp.StatusCode}");
                }
            }
            catch (Exception e)
            {
                if (_lastError is null || !_lastError.HasValue && _lastError!.Value.AddMinutes(15) < DateTime.UtcNow)
                {
                    _lastError = DateTime.UtcNow;
                    Log.Error($"An error has occured while trying to send data to the server. Check that it is running properly.");
                }

                Log.Debug($"{e}", Config.Debug);
            }

        }
    }
}