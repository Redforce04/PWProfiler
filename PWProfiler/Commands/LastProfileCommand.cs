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
using PWProfiler.Structs;

namespace PWProfiler.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    [CommandHandler(typeof(GameConsoleCommandHandler))]

    public class LastProfileCommand : ICommand
    {
        // ReSharper disable once StringLiteralTypo
        public string Command { get; } = "lastprofile";
        
        public string[] Aliases { get; } = new[] { "profile" };

        public string Description { get; } = $"Gets the last profile taken.";
        
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!PWProfiler.Singleton.Config.NetDataIntegrationEnabled)
            {
                response = $"The NetData integration is disabled. Only the last NetData packet is saved for inspection.";
                return false;
            }

            if (NetDataIntegration.Singleton.LastPacket == null)
            {
                response = $"No saved packet was found.";
                return false;
            }

            NetDataPacket packet = NetDataIntegration.Singleton.LastPacket.Value;
            string a = "<b><color=cyan>";
            string b = "</b></color>";
            string c = "<i>";
            string d = "</i><br>";
            
            response = $"Last Captured NetData Packet: " +
                       $"{a}Server Name:{b}      {c}{packet.ServerName}{d}" +
                       $"{a}Server Port:{b}      {c}{packet.Port}{d}" +
                       $"{a}Time:{b}             {c}{DateTimeOffset.FromUnixTimeSeconds(packet.Epoch).DateTime:G}{d}" +
                       $"{a}Refresh Speed:{b}    {c}{packet.RefreshSpeed}{d}" +
                       $"{a}Players{b}           {c}{packet.Players}{d}" +
                       $"{a}Average Tps{b}       {c}{packet.AverageTps}{d}" +
                       $"{a}Delta Time Avg:{b}   {c}{packet.AverageDeltaTime}{d}" +
                       $"{a}Cpu Usage:{b}        {c}{packet.CpuUsage}{d}" +
                       $"{a}Memory Usage:{b}     {c}{packet.MemoryUsage}{d}" +
                       $"{a}Low Tps Warnings:{b} {c}{packet.LowTpsWarnCount}{d}";
            return true;
        }
    }
}