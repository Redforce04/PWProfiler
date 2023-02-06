// <copyright file="Log.cs" company="Redforce04#4091">
// Copyright (c) Redforce04. All rights reserved.
// </copyright>
// -----------------------------------------
//    Solution:         NetDataSL
//    Project:          NetDataSL
//    FileName:         ServerConfig.cs
//    Author:           Redforce04#4091
//    Revision Date:    02/05/2023 12:38 PM
//    Created Date:     02/05/2023 12:38 PM
// -----------------------------------------

namespace PWProfiler.Structs;

using System.Text.Json.Serialization;

#pragma warning disable SA1313,SA1611

/// <summary>
/// The config for allowing an individual sl server to connect.
/// </summary>
public struct ServerConfig
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ServerConfig"/> class.
    /// </summary>
    [Newtonsoft.Json.JsonConstructor]

    // ReSharper disable InconsistentNaming
    public ServerConfig(int Port, string ServerName, string Key)
    {
        this.Port = Port;
        this.ServerName = ServerName;
        this.Key = Key;
    }

    /// <summary>
    /// Gets the port of the server.
    /// </summary>
    [JsonPropertyName("Port")]
    public int Port { get; private set; }

    /// <summary>
    /// Gets the name of the server.
    /// </summary>
    [JsonPropertyName("ServerName")]
    public string ServerName { get; private set; }

    /// <summary>
    /// Gets the key of the server.
    /// </summary>
    [JsonPropertyName("Key")]
    public string Key { get; private set; }
}
