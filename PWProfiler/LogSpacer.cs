// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         LogSpacer.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/31/2023 3:40 PM
//    Created Date:     01/31/2023 3:40 PM
// -----------------------------------------

using System.Collections.Generic;
using PWProfiler.Structs;

namespace PWProfiler
{
    public static class LogSpacer
    {
        public static string Process(LoggingInfo info)
        {
            string content =
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.DateTime)], info.DateTime.ToString("G")) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.Epoch)], info.Epoch.ToString()) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.AverageTps)], info.AverageTps.ToString("F")) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.AverageDeltaTime)], info.AverageDeltaTime.ToString("0.0000000")) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.MemoryUsage)], info.MemoryUsage.ToString()) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.CpuUsage)], info.CpuUsage.ToString("000.0000")) +
                _processLength(LoggingInfoSpacings[nameof(LoggingInfo.Players)], info.Players.ToString(), false) + "\n";
            return content;
        }

        public static string Process(LowTps info)
        {
            string content =
                _processLength(LoggingFpsSpacings[nameof(LowTps.DateTime)], info.DateTime.ToString("G")) +
                _processLength(LoggingFpsSpacings[nameof(LowTps.Epoch)], info.Epoch.ToString()) +
                _processLength(LoggingFpsSpacings[nameof(LowTps.InstanceNumber)], info.InstanceNumber.ToString()) +
                _processLength(LoggingFpsSpacings[nameof(LowTps.Tps)], info.Tps.ToString("F")) +
                _processLength(LoggingFpsSpacings[nameof(LowTps.DeltaTime)], info.DeltaTime.ToString("0.00000000")) +
                _processLength(LoggingFpsSpacings[nameof(LowTps.Players)], info.Players.ToString(), false) + "\n";
            return content;
        }

        static string _processLength(Spacing spacing, string input, bool separator = true)
        {
            if (input.Length > spacing.AllocatedSpaces)
                return input.Substring(0, spacing.AllocatedSpaces) + (separator ? spacing.Separator : "");

            if (input.Length < spacing.AllocatedSpaces)
                return input.PadRight(spacing.AllocatedSpaces) + (separator ? spacing.Separator : "");

            return input + (separator ? spacing.Separator : "");
        }

        public static string Process(string type, List<string> arguments)
        {
            string content;
            if (type == nameof(LoggingInfo))
            {
                    content =
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.DateTime)], arguments[0]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.Epoch)], arguments[1]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.AverageTps)], arguments[2]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.AverageDeltaTime)], arguments[3]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.MemoryUsage)], arguments[4]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.CpuUsage)], arguments[5]) +
                    _processLength(LoggingInfoSpacings[nameof(LoggingInfo.Players)], arguments[6], false);
            }
            else
            {
                content = 
                    _processLength(LoggingFpsSpacings[nameof(LowTps.DateTime)], arguments[0]) +
                    _processLength(LoggingFpsSpacings[nameof(LowTps.Epoch)], arguments[1]) +
                    _processLength(LoggingFpsSpacings[nameof(LowTps.InstanceNumber)], arguments[2]) +
                    _processLength(LoggingFpsSpacings[nameof(LowTps.Tps)], arguments[3]) +
                    _processLength(LoggingFpsSpacings[nameof(LowTps.DeltaTime)], arguments[4]) +
                    _processLength(LoggingFpsSpacings[nameof(LowTps.Players)], arguments[5], false);
            }

            return content;
        }

        private static readonly Dictionary<string, Spacing> LoggingInfoSpacings = new Dictionary<string, Spacing>()
        {
            { nameof(LoggingInfo.DateTime), new Spacing(20) },
            { nameof(LoggingInfo.Epoch), new Spacing(10) },
            { nameof(LoggingInfo.AverageTps), new Spacing(11) },
            { nameof(LoggingInfo.AverageDeltaTime), new Spacing(18) },
            { nameof(LoggingInfo.MemoryUsage), new Spacing(12) },
            { nameof(LoggingInfo.CpuUsage), new Spacing(9) },
            { nameof(LoggingInfo.Players), new Spacing(7) }
        };

        private static readonly Dictionary<string, Spacing> LoggingFpsSpacings = new Dictionary<string, Spacing>()
        {
            { nameof(LowTps.DateTime), new Spacing(20) },
            { nameof(LowTps.Epoch), new Spacing(10) },
            { nameof(LowTps.InstanceNumber), new Spacing(10) },
            { nameof(LowTps.Tps), new Spacing(8) },
            { nameof(LowTps.DeltaTime), new Spacing(10) },
            { nameof(LowTps.Players), new Spacing(7) }

        };

}
    public struct Spacing
    {
        public Spacing( int allocatedSpaces)
        {
            Separator = "  |  ";
            AllocatedSpaces = allocatedSpaces;
        }
        public readonly int AllocatedSpaces;
        public readonly string Separator;
    }
}