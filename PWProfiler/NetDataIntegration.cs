// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         NetDataIntegration.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:37 PM
//    Created Date:     01/30/2023 1:37 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
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

        private DateTimeOffset _previousPacketTime;

        /// <summary>
        /// 
        /// </summary>
        public NetDataIntegration()
        {
            Singleton = this;
            _previousPacketTime = DateTimeOffset.UtcNow;
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
                PreviousPacketEpoch = _previousPacketTime.ToUnixTimeMilliseconds(),
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
                else
                {
                    _previousPacketTime = DateTimeOffset.UtcNow;
                }

                try
                {
                    var results = JsonConvert.DeserializeObject<Dictionary<string, object>>(response);
                    if ((string)results["message"] == "slow refresh time")
                    {
                        float serverRefreshTime = (float)results["server refresh"];
                        Log.Error($"Refresh time is slower than the server. " +
                                  $"The refresh speed of the plugin will be set to the refresh time of the server." +
                                  $" Update this variable in the plugin config to prevent the message. " +
                                  $"(server refresh: {serverRefreshTime}, plugin refresh: {PWProfiler.Singleton.Config.StatsRefreshTime})");
                        Config.StatsRefreshTime = serverRefreshTime;
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Could not deserialize json response message. {e}");
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