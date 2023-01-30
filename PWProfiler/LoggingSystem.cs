// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         LoggingSystem.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 1:56 PM
//    Created Date:     01/30/2023 1:56 PM
// -----------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using PluginAPI.Core;
using PWProfiler.Configs;
using PWProfiler.Enums;
using PWProfiler.Structs;

namespace PWProfiler
{
    public class LoggingSystem
    {
        public static LoggingSystem Singleton;

        private static MainConfig Config => PWProfiler.Singleton.Config;

        private static Dictionary<LoggingFile, string> _logLocations;
        
        public LoggingSystem()
        {
            Singleton = this;
            // Initialize the paths for the NetData logging.
            _setNetDataLogLocation();
            // Initialize the files and directories for file logging if they don't exist.
            _initFileLoggingDirectory();
        }
        /// <summary>
        /// Initialize the paths for the log files.
        /// </summary>
        private void _setNetDataLogLocation()
        {
            var handler = PluginHandler.Get(this);
            string path = handler.MainConfigPath.Replace("config.yml", "");
            _logLocations = new Dictionary<LoggingFile, string>()
            {
                { LoggingFile.Stats, $"{path}Stats/" },
                { LoggingFile.LowTps, $"{path}LowTPS/" }
            };
            Log.Debug($"Logging Stats to \'{_logLocations[LoggingFile.Stats]}\'");
            Log.Debug($"Logging LowTps to \'{_logLocations[LoggingFile.LowTps]}\'");
        }
        
        /// <summary>
        /// Initialize the files and directories for file logging if they don't exist.
        /// </summary>
        private void _initFileLoggingDirectory()
        {
            try
            {
                foreach (string directory in _logLocations.Values)
                {
                    if (!Directory.Exists(directory))
                        Directory.CreateDirectory(directory);
                }

            }
            catch (Exception e)
            {
                Log.Error($"Could not load File Logging Paths or Directories. File Logging will be disabled. \n{e}");
                Config.FileLoggingEnabled = false;
            }
        }
        
        /// <summary>
        /// The method that logs stats to a file.
        /// </summary>
        /// <param name="log">The <see cref="LoggingInfo"/> to log.</param>
        internal void LogStatsToFile(LoggingInfo log)
        {
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.Stats] + $"Stats-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.AppendAllText(path, $"# Local Time = Epoch Time = Average TPS = Average Delta Time = Memory Usage = Cpu Usage\n");
            }

            string stats = $"stats = {log.DateTime.ToLocalTime()} = {log.Epoch} = {log.AverageTps} = {log.AverageDeltaTime} = {log.MemoryUsage} = {log.CpuUsage} = {log.Players}\n";
            File.AppendAllText(path, stats);
        }
        
        /// <summary>
        /// The method that logs low <see cref="LowTps"/> instances to files.
        /// </summary>
        /// <param name="tpsInstances"></param>
        internal static void LogLowTpsInstances(List<LowTps> tpsInstances)
        {
            string combined = "";
            foreach (LowTps tps in tpsInstances)
                combined += $"lowTps = {tps.DateTime} = {tps.Epoch} = {tps.InstanceNumber} = {tps.Tps} = {tps.DeltaTime} = {tps.Players}\n";
            combined += $"# Refresh, server caught back up. \n";
            
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.LowTps] + $"LowTPS-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                File.AppendAllText(path, $"# Local Time = Epoch Time = Instance Number = TPS = Delta Time\n");
            }
            File.AppendAllText(path, combined);
            
        }

    }
}