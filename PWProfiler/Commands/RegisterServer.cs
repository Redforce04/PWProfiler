using System;
using System.Security.Cryptography;
using CommandSystem;
using Newtonsoft.Json;
using PluginAPI.Core;
using PWProfiler.Structs;

namespace PWProfiler.Commands;

[CommandHandler(typeof(GameConsoleCommandHandler))]

public class RegisterServerCommand : ICommand
{
    // ReSharper disable once StringLiteralTypo
    public string Command { get; } = "RegisterServer";

    public string[] Aliases { get; } = Array.Empty<string>();

    public string Description { get; } = $"Creates an api key for the netdata integration.";

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (PWProfiler.Singleton == null || PWProfiler.Singleton.Config == null)
        {
            response = $"Plugin or config is null. This is an error.";
            return false;
        }
        if (!string.IsNullOrEmpty(PWProfiler.Singleton.Config.ApiKey))
        {
            ServerConfig config = new ServerConfig(Server.Port, PWProfiler.Singleton.Config.ServerName,
                PWProfiler.Singleton.Config.ApiKey);
            string existingJson = JsonConvert.SerializeObject(config);
            response =
                $"ApiKey was found already. To refresh this key, delete the api key from your config. Otherwise add this to the server instances section of your NetData Plugin config: \n\n{existingJson}";
            return false;
        }

        string key = GenerateApiKey();
        ServerConfig conf = new ServerConfig(Server.Port, PWProfiler.Singleton.Config.ServerName, key);
        string json = JsonConvert.SerializeObject(conf);
        PWProfiler.Singleton.Config.ApiKey = key;
        
        var handler = PluginAPI.Core.PluginHandler.Get(this);
        string x = "";
        if (handler is null)
        {
            x = $"\nCould not save the api key to the config. Add the key \'{key}\' to the sl plugin config as well.";
        }
        else
            handler.SaveConfig(PWProfiler.Singleton, "ApiKey");
        response = $"Modify your NetData Plugin config and add this to the server instances section: \n\n{json}.{x}";
        return true;
    }

    public static string GenerateApiKey()
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