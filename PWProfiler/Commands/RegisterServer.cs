using System;
using System.Security.Cryptography;
using CommandSystem;
using Newtonsoft.Json;
using PluginAPI.Core;
using PWProfiler.Structs;

namespace PWProfiler.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]
public class RegisterServer
{
    public class RegisterServerCommand : ICommand
    {
        // ReSharper disable once StringLiteralTypo
        public string Command { get; } = "RegisterServer";

        public string[] Aliases { get; } = Array.Empty<string>();

        public string Description { get; } = $"Creates an api key for the netdata integration.";

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (PWProfiler.Singleton.Config.ApiKey != "")
            {
                ServerConfig config = new ServerConfig(Server.Port, PWProfiler.Singleton.Config.ServerName, PWProfiler.Singleton.Config.ApiKey);
                string existingJson = JsonConvert.SerializeObject(config);

                response = $"ApiKey was found already. To refresh this key, delete the api key from your config. Otherwise add this to the server instances section of your NetData Plugin config: \n\n{existingJson}";
                return false;
            }
            ServerConfig conf = new ServerConfig(Server.Port, PWProfiler.Singleton.Config.ServerName, GenerateApiKey());
            string json = JsonConvert.SerializeObject(conf);
            response = $"Modify your NetData Plugin config and add this to the server instances section: \n\n{json}";
            return true;
        }
        public string GenerateApiKey()
        {
            using var provider = new RNGCryptoServiceProvider();
            var bytes = new byte[32];
            provider.GetBytes(bytes);

            return "PWP-" + Convert.ToBase64String(bytes)
                .Replace("/", "")
                .Replace("+", "")
                .Replace("=", "")
                .Replace("\"", "")
                .Substring(0, 33);
        }

    }
}