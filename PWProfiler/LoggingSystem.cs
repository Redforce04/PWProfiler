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
using JetBrains.Annotations;
using PluginAPI.Core;
using PWProfiler.Configs;
using PWProfiler.Enums;
using PWProfiler.Structs;

namespace PWProfiler
{
    public class LoggingSystem
    {
        public static LoggingSystem Singleton;
        private static string MainConfigPath => PWProfiler.Singleton.Config.FileLoggingLocation;
        private static MainConfig Config => PWProfiler.Singleton.Config;

        private static Dictionary<LoggingFile, string> _logLocations;
        
        public LoggingSystem()
        {
            if (MainConfigPath == null)
            {
                Log.Error($"The file logging path config is not present. File logging will not occur.");
                return;
            }
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
            Log.Debug($"Main Config Path: {MainConfigPath}", Config.Debug);
            string path = MainConfigPath.Replace("config.yml", "");
            _logLocations = new Dictionary<LoggingFile, string>()
            {
                { LoggingFile.Stats, $"{path}Stats/" },
                { LoggingFile.LowTps, $"{path}LowTPS/" }
            };
            Log.Debug($"Logging Stats to \'{_logLocations[LoggingFile.Stats]}\'", Config.Debug);
            Log.Debug($"Logging LowTps to \'{_logLocations[LoggingFile.LowTps]}\'", Config.Debug);
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

                LogStatsToFile(null, true);
                LogLowTpsInstances(null, true);
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
        /// <param name="firstTime">Whether or not this is the first time loading these files.</param>
        internal static void LogStatsToFile(LoggingInfo? log, bool firstTime = false)
        {
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.Stats] + $"Stats-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                string context = LogSpacer.Process(nameof(LoggingInfo), new List<string>()
                {
                    "# Local Time",
                    "Epoch Time",
                    "Average TPS",
                    "Average Delta Time",
                    "Memory Usage",
                    "Cpu Usage",
                    "Players"
                });
                File.AppendAllText(path, context + "\n");
            }
            
            if(firstTime || log == null)
                return;
            
            File.AppendAllText(path, LogSpacer.Process(log.Value));
        }

        /// <summary>
        /// The method that logs low <see cref="LowTps"/> instances to files.
        /// </summary>
        /// <param name="tpsInstances">The instances of <see cref="LowTps"/> to log.</param>
        /// <param name="firstTime">Whether or not this is the first time loading these files.</param>
        internal static void LogLowTpsInstances([CanBeNull] List<LowTps> tpsInstances, bool firstTime = false)
        {
            DateTime date = DateTime.Now;
            string path = _logLocations[LoggingFile.LowTps] + $"LowTPS-{date.Month}-{date.Day}-{date.Year}.txt";
            if (!File.Exists(path))
            {
                File.Create(path).Close();
                string context = LogSpacer.Process(nameof(LowTps), new List<string>()
                {
                    "# Local Time",
                    "Epoch Time",
                    "Instance #",
                    "TPS",
                    "Delta Time",
                    "Players"
                    
                });
                File.AppendAllText(path, context + "\n");
            }
            
            if(firstTime || tpsInstances is null)
                return;
            
            string combined = "";
            foreach (LowTps tps in tpsInstances)
                combined += LogSpacer.Process(tps);
            combined += $"# Refresh, server caught back up. \n";

            File.AppendAllText(path, combined);
            
        }

    }
}