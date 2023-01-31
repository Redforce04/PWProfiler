// -----------------------------------------
//    Solution:         PWProfiler
//    Project:          PWProfiler
//    FileName:         TpsCommand.cs
//    Author:           Redforce04#4091
//    Revision Date:    01/30/2023 11:40 AM
//    Created Date:     01/30/2023 11:40 AM
// -----------------------------------------

using System;
using CommandSystem;

namespace PWProfiler.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]

    public class TpsCommand : ICommand
    {
        // ReSharper disable once StringLiteralTypo
        public string Command { get; } = "tickspeed";
        
        // ReSharper disable once StringLiteralTypo
        public string[] Aliases { get; } = new[] { "tps", "deltatime", "fps" };

        public string Description
        {
            get
            {
                if (PWProfiler.Singleton is null || PWProfiler.Singleton.Config is null)
                    return $"Gets the average tps over the last 120 frames.";
                else
                    return $"Gets the average tps over the last {PWProfiler.Singleton.Config.AverageFrameSampleAmount} frames.";
            }
        }
        
        private float GetAverageDeltaTime => PWProfiler.Singleton.TimingMonoBehaviour.AverageDeltaTime;
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = $"Tick Speed / Delta Time Speed: ({GetAverageDeltaTime})s ~ {1f/GetAverageDeltaTime}FPS";
            return true;
        }
    }
}