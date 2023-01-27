using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using JetBrains.dotMemoryUnit;
using JetBrains.dotMemoryUnit.Kernel;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using MEC;
using Mirror.LiteNetLib4Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PWProfiler
{
    
    public class PwProfiler 
    {
        private const float StatsMeasuringDelay = 5f;
        private const bool CompareToLastMemoryCheckpoint = false;
        internal float ProblematicFPS = 30;
        private bool CheckMemory = false;
        private bool CheckCPU = false;
        private bool NetdataLogging = true;
        private string NetDataLogLocation = Path.GetTempPath() + "PwProfiler/";
        private string NetDataLog => NetDataLogLocation + $"Server-{ServerStatic.ServerPort}";
        
        public static PwProfiler Singleton { get; private set; }
        private TimingMonoBehaviour _timingMonoBehaviour;
        private static Dictionary<LoggingFile, string> _logLocations;
        private Process CurrentProcess;
        private PerformanceCounter cpuCounter;
        [PluginPriority(LoadPriority.Highest)]
        [PluginEntryPoint("PWProfiler", "1.0.0", "A plugin to log profiling information.", "Redforce04#4091")]
        void LoadPlugin()
        {
            ProblematicFPS = 1 / ProblematicFPS;

            var handler = PluginHandler.Get(this);

            string path = handler.MainConfigPath.Replace("config.yml", "");
            _logLocations= new Dictionary<LoggingFile, string>()
            {
                { LoggingFile.Stats, $"{path}Stats/" },
                { LoggingFile.LowFPS, $"{path}LowFPS/" }
            };
            foreach (string directory in _logLocations.Values)
            {
                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }
                }
                catch(Exception e)
                {
                    Log.Error($"Could not create directory '{directory}'\n{e}");
                }
            }

            if (NetdataLogging && !Directory.Exists(NetDataLogLocation))
                Directory.CreateDirectory(NetDataLogLocation);
            if (NetdataLogging && !File.Exists(NetDataLog))
                File.Create(NetDataLog);
            
            Log.Info($"Logging Stats to {_logLocations[LoggingFile.Stats]}");
            if(NetdataLogging)
                Log.Info($"NetData Logging. NetData Log Location: \'{NetDataLogLocation}\'");
            foreach (string directory in Directory.GetDirectories(Path.GetTempPath()))
            {
                Log.Debug($"Temp Directory: {directory}");
            }
            foreach (string file in Directory.GetFiles(Path.GetTempPath()))
            {
                Log.Debug($"Temp File: {file}");
            }
            CurrentProcess = Process.GetCurrentProcess();
            cpuCounter = new PerformanceCounter();
            cpuCounter.CategoryName = "Processor";
            cpuCounter.CounterName = "% Processor Time";
            cpuCounter.InstanceName = "_Total";
            
            if (!dotMemoryApi.IsEnabled)
            {
                CheckMemory = false;   
                Log.Error($"Dot Memory is not Enabled! Memory stats will not be available.");
            }
            
            Singleton = this;
            _timingMonoBehaviour = Object.FindObjectOfType<TimingMonoBehaviour>();
            if(_timingMonoBehaviour is null)
                _timingMonoBehaviour =  LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<TimingMonoBehaviour>();

                //obj.AddComponent<TimingMonoBehaviour>();
            Timing.RunCoroutine(PerformanceMeasuringCoroutine());
            if(CompareToLastMemoryCheckpoint && CheckMemory)
                _checkpoint = dotMemory.Check();

        }
        private IEnumerator<float> PerformanceMeasuringCoroutine()
        {
            for (;;) //repeat the following infinitely
            {
                yield return Timing.WaitForSeconds(StatsMeasuringDelay);
                try
                {
                    if (_timingMonoBehaviour == null) 
                        _timingMonoBehaviour =  LiteNetLib4MirrorNetworkManager.singleton.gameObject.AddComponent<TimingMonoBehaviour>(); 
                    LogStats();
                }
                catch (Exception e)
                {
                    Log.Error($"LogStats has caught an exception: {e}");
                }
                
            }
        }

        private MemoryCheckPoint _checkpoint;
        private void LogStats()
        {
            long allocatedMemory = -1;
            allocatedMemory = CurrentProcess.WorkingSet64;
            if (CheckMemory)
            {

                _checkpoint = dotMemory.Check(memory =>
                {
                    if (CompareToLastMemoryCheckpoint)
                    {
                        var newObjects = memory.GetDifference(_checkpoint).GetNewObjects();
                        var createdObjectsCount = newObjects.ObjectsCount;
                        allocatedMemory = newObjects.SizeInBytes;
                    }
                    else
                        allocatedMemory = memory.SizeInBytes;
                });
            }

            float cpuUsage = cpuCounter.NextValue();
            LoggingInfo loggingInfo = new LoggingInfo()
            {
                DateTime = DateTime.UtcNow,
                Epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                AverageFPS = 1/_timingMonoBehaviour.AverageDeltaTime,
                AverageDeltaTime = _timingMonoBehaviour.AverageDeltaTime,
                MemoryUsage = allocatedMemory/1000000,
                CpuUsage = cpuUsage,
                Players = Server.PlayerCount
            };
            LogStatsToFile(loggingInfo);
            if (_timingMonoBehaviour.SlowDeltaTime.Count > 0)
            {
                List<LowFPS> fpsInstances = new List<LowFPS>();
                for (int i = 0; i < _timingMonoBehaviour.SlowDeltaTime.Count; i++)
                {
                    var fps = _timingMonoBehaviour.SlowDeltaTime[i];
                    LowFPS lowFPS = new LowFPS()
                    {
                        DateTime = DateTime.Now,
                        Epoch = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                        FPS = 1/fps,
                        DeltaTime = fps,
                        InstanceNumber = i,
                        Players = Server.PlayerCount
                    };
                    fpsInstances.Add(lowFPS);
                }

                LOGLowFPSInstances(fpsInstances);
                _timingMonoBehaviour.SlowDeltaTime.Clear();
            }
        }

        private void LOGLowFPSInstances(List<LowFPS> fpsInstances)
        {
            string combined = "";
            foreach (LowFPS fps in fpsInstances)
                combined += $"lowfps = {fps.DateTime} = {fps.Epoch} = {fps.InstanceNumber} = {fps.FPS} = {fps.DeltaTime} = {fps.Players}\n";
            combined += $"# Refresh, server caught back up. \n";
            
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.LowFPS] + $"LowFPS-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.AppendAllText(path, $"# Local Time = Epoch Time = Instance Number = FPS = Delta Time\n");
            }
            File.AppendAllText(path, combined);


            if (Singleton.NetdataLogging)
            {
                if (NetdataLogging)
                {
                    using (FileStream fs = new FileStream(NetDataLog, FileMode.Append, FileAccess.Write, FileShare.Write))
                    {
                        StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                        sw.Write($"refresh = {StatsMeasuringDelay}\n");
                        sw.Write(combined);
                        sw.Close();
                        fs.Close();
                    }
                }
            }
        }

        private void LogStatsToFile(LoggingInfo log)
        {
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.Stats] + $"Stats-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.AppendAllText(path, $"# Local Time = Epoch Time = Average FPS = Average Delta Time = Memory Usage = Cpu Usage\n");
            }

            string stats =
                $"stats = {log.DateTime.ToLocalTime()} = {log.Epoch} = {log.AverageFPS} = {log.AverageDeltaTime} = {log.MemoryUsage} = {log.CpuUsage} = {log.Players}\n";
            File.AppendAllText(path, stats);
            if (NetdataLogging)
            {
                using (FileStream fs = new FileStream(NetDataLog, FileMode.Truncate, FileAccess.Write, FileShare.Write))
                {
                    StreamWriter sw = new StreamWriter(fs, Encoding.UTF8);
                    sw.Write(stats);
                    sw.Close();
                    fs.Close();
                }
            }
        }

        enum LoggingFile
        {
            Stats,
            LowFPS
        }

        struct LowFPS
        {
            internal DateTime DateTime;
            internal long Epoch;
            internal int InstanceNumber;
            internal float FPS;
            internal float DeltaTime;
            internal int Players;
        }
        struct LoggingInfo
        {
            internal DateTime DateTime;
            internal long Epoch;
            internal float AverageFPS;
            internal float AverageDeltaTime;
            internal long MemoryUsage;
            internal float CpuUsage;
            internal int Players;
        }
    }
}