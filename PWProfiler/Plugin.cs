﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using MEC;
using Mirror.LiteNetLib4Mirror;
using PWProfiler.Configs;
using PWProfiler.Structs;
using Object = UnityEngine.Object;

namespace PWProfiler
{
    
    // ReSharper disable once ClassNeverInstantiated.Global
    // ReSharper disable once InconsistentNaming
    public class PWProfiler
    {
        /// <summary>
        /// Main plugin instance.
        /// </summary>
        public static PWProfiler Singleton { get; private set; }

        public const string ApiVersion = "1.1.2";
        /// <summary>
        /// Main Plugin Config
        /// </summary>
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once UnassignedField.Global
        [PluginConfig] public MainConfig Config;
        
        /// <summary>
        /// Gets the Epoch
        /// </summary>
        private long Epoch => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        /// <summary>
        /// The instance of TimingMonoBehaviour
        /// </summary>
        internal TimingMonoBehaviour TimingMonoBehaviour;
        
        /// <summary>
        /// Process information for cpu stats.
        /// </summary>
        private Process _currentProcess;
        
        /// <summary>
        /// Performance Counter for cpu stats.
        /// </summary>
        private PerformanceCounter _cpuCounter;
        
        
        /// <summary>
        /// Main Plugin Load Point
        /// </summary>
        //[PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("PWProfiler", "1.0.0", "A plugin to log profiling information.", "Redforce04#4091")]
        // ReSharper disable once ArrangeTypeMemberModifiers
        // ReSharper disable once UnusedMember.Local
        void LoadPlugin()
        {
            _getVersionInstances();
            
                // App code goes here. Dispose the SDK before exiting to flush events.

                Log.Info($"PWProfiler Loading ({GitCommitHash}, {VersionIdentifier}).");
                if (Config is null)
                {
                    var handler = PluginHandler.Get(this);
                    handler.LoadConfig(this, nameof(MainConfig));
                }

                if (Config == null || !Config.Enabled)
                {
                    Log.Warning($"PWProfiler is not enabled by config. It will not load.");
                    return;
                }
                CosturaUtility.Initialize();

                Singleton = this;
                // Plugin checks delta time so we need to convert the tps to a delta time.
                Config.LowTps /= 1;

                // Start the NetDataIntegration
                if (Config.NetDataIntegrationEnabled)
                {
                    Log.Debug($"Starting Netdata Integration", Config.Debug);
                    var unused = new NetDataIntegration();
                }

                // Start the Logging System
                if (Config.FileLoggingEnabled)
                {
                    Log.Debug($"Starting Logging System", Config.Debug);
                    var unused = new LoggingSystem();
                }

                // Initializes the information to track cpu usage.
                if (Config.CheckCpu)
                {
                    Log.Debug($"Initializing Cpu Stats", Config.Debug);
                    _initCpuInfo();
                }

                Timing.CallDelayed(5f, () =>
                {

                    // Creates the TimingMonoBehaviour by attaching the component to the network manager singleton.
                    Log.Debug($"Creating Timing MonoBehaviour", Config.Debug);
                    TimingMonoBehaviour = Object.FindObjectOfType<TimingMonoBehaviour>() ??
                                          LiteNetLib4MirrorNetworkManager
                                              .singleton.gameObject.AddComponent<TimingMonoBehaviour>();

                    // Starts the main coroutine for checking stats.
                    Log.Debug($"Beginning Performance Measuring Coroutine", Config.Debug);
                    Timing.RunCoroutine(PerformanceMeasuringCoroutine());
                });
        }

        /// <summary>
        /// Gets information about this build.
        /// </summary>
        private static void _getVersionInstances()
        {
            try
            {
                GitCommitHash = AssemblyInfo.CommitHash;
                VersionIdentifier = AssemblyInfo.CommitBranch;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

        }
        
        /// <summary>
        /// The shortened hash of the commit
        /// </summary>
        public static string GitCommitHash = String.Empty;
        
        /// <summary>
        /// The build method of this build.
        /// </summary>
        public static string VersionIdentifier = String.Empty;


        /// <summary>
        /// Initializes the information to track cpu usage.
        /// </summary>
        private void _initCpuInfo()
        {
            _currentProcess = Process.GetCurrentProcess();
            _cpuCounter = new PerformanceCounter();
            _cpuCounter.CategoryName = "Processor";
            _cpuCounter.CounterName = "% Processor Time";
            _cpuCounter.InstanceName = "_Total";

        }
        
        
        /// <summary>
        /// The coroutine that collects data at the supplied refresh <see cref="Configs"/>.<see cref="Configs.MainConfig.StatsRefreshTime"/>
        /// </summary>
        private IEnumerator<float> PerformanceMeasuringCoroutine()
        {
            for (;;) //repeat the following infinitely
            {
                yield return Timing.WaitForSeconds(Config.StatsRefreshTime);
                try
                {
                    if (TimingMonoBehaviour == null) 
                        TimingMonoBehaviour =  LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<TimingMonoBehaviour>(); 
                    _logStats();
                }
                catch (Exception e)
                {
                    Log.Error($"LogStats has caught an exception: {e}");
                }
                
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        /// <summary>
        /// The method that collects the stats and logs them.
        /// </summary>
        private void _logStats()
        {
            // Check memory usage if enabled.
            long allocatedMemory  = Config.CheckMemory ? _currentProcess.WorkingSet64 : 0;
            
            // Check CPU Usage if enabled.
            float cpuUsage = Config.CheckCpu ? _cpuCounter.NextValue() : 0;
            
            // Create the struct object.
            LoggingInfo loggingInfo = new LoggingInfo()
            {
                DateTime = DateTime.UtcNow,
                Epoch = (ulong)Epoch,
                AverageTps = 1/TimingMonoBehaviour.AverageDeltaTime,
                AverageDeltaTime = TimingMonoBehaviour.AverageDeltaTime,
                MemoryUsage = (ulong)allocatedMemory/1000000,
                CpuUsage = cpuUsage,
                Players = Server.PlayerCount
            };
            
            // if file logging is enabled, log stats to file.
            if(Config.FileLoggingEnabled)
                LoggingSystem.LogStatsToFile(loggingInfo);
            
            // if there have been instances of low tps, log them
            if (TimingMonoBehaviour.SlowDeltaTime.Count > 0)
            {
                // get instances of low tps and log them.
                List<LowTps> tpsInstances = new List<LowTps>();
                for (int i = 0; i < TimingMonoBehaviour.SlowDeltaTime.Count; i++)
                {
                    var tps = TimingMonoBehaviour.SlowDeltaTime[i];
                    LowTps lowTps = new LowTps()
                    {
                        DateTime = DateTime.Now,
                        Epoch = Epoch,
                        Tps = 1/tps,
                        DeltaTime = tps,
                        InstanceNumber = i,
                        Players = Server.PlayerCount
                    };
                    tpsInstances.Add(lowTps);
                }
                
                // if file logging is enabled, log the low tps instances
                if(Config.FileLoggingEnabled)
                    LoggingSystem.LogLowTpsInstances(tpsInstances);
                
                // if the NetData integration is enabled send the stats to it to process
                if(Config.NetDataIntegrationEnabled)
                    NetDataIntegration.Singleton.SendInfoToNetDataIntegration(loggingInfo, tpsInstances.Count);
                
                // make sure to clear the instances of low tps
                TimingMonoBehaviour.SlowDeltaTime.Clear();
            }
            // in case there were no stats, trigger NetData integration again
            else if (Config.NetDataIntegrationEnabled)
                NetDataIntegration.Singleton.SendInfoToNetDataIntegration(loggingInfo, 0);


        }

    }
}