
# PW Profiler

An SCP-SL Server Profiler build on the Northwood Plugin-API. An integration is available to report this to netdata, which can be found [here](https://git.peanutworshipers.net/redforce04/NetDataSL).
<br/>


## Authors

- [Redforce04#4091](https://git.peanutworshipers.net/Redforce04)
<br/>

## **Features:**
#### -  Records a file log of these stats:
- Checks **TPS / delta time** from an **average** of the past 120 frames (configurable)
    - Most commands only take the TPS from the very last frame, which can miss short lag spikes.
- Checks the **Memory allocation size** 
    - This is a rough memory usage estimate, that is accurate to around 50 - 100 mb.
- Checks the rough **CPU usage** 
    - Currently this is pretty inaccurate but I'm working on it.
- Checks the **player count**
    - Per server player count
- Checks how many times server **falls below a certain tps**
    - Configurable option for what qualifies as "low tps" (default is 30 ticks per second)
    - Logs how many times the server fell below that tps in the past 5 seconds.
- Works inside of  **Docker / Pterodactyl**
    - Should work fine on windows and linux
- Stats log every 5 seconds.
    - Tps is still accurately measured over 120 frames, however it is only logged every 5 seconds

<br/>

## **Wip:**
- Adding a feature to pull cpu and memory stats from docker api
<br/>

## FAQ

#### When will the netdata integration come out?

It is out now! It can be found [Here](https://git.peanutworshipers.net/redforce04/NetDataSL).

#### I have a suggestion. How can I suggest it?

Reach out to me over discord or leave an issue and I will tag it with the suggestion tag.

#### How do I leave a bug report?

Create an issue and I will look at it.


<br/>

## Commands

### - Last Profile:
Usage: 
- `lastprofile` or
- `profile`

Returns the last  profile taken including all stats that are logged normally. This command requires the netdata integration to be enabled.

### - Tickspeed:
Usage: 
- `tickspeed` or
- `deltatime` or
- `tps` or
- `fps`

Returns the last average fps and deltatime from the last 120 frames.
<br/>
<br/>

## Installation

Add the profiler plugin to your plugin directory

```bash
Installation:
  | - .config/SCP Secret Laboratory/PluginAPI/plugins/Global/PWProfiler.dll
  | - .config/SCP Secret Laboratory/PluginAPI/plugins/Global/dependencies/NewtonsoftJson.dll

Configs:
  | - .config/SCP Secret Laboratory/PluginAPI/plugins/Global/PWProfiler/config.yml

Default Logging Locations (Configurable): 
  | - .config/SCP Secret Laboratory/PluginAPI/plugins/Global/PWProfiler/Stats/Stats-{month}-{day}-{year}.txt
  | - .config/SCP Secret Laboratory/PluginAPI/plugins/Global/PWProfiler/LowTPS/LowTPS-{month}-{day}-{year}.txt
```
<br/>

## Configuration
```yml
    # Whether the plugin is enabled or not.
enabled: true

    # Whether debug mod is disabled or not.
debug: true

    # Whether the plugin will check memory stats.
check_memory: true

    # Whether the plugin will check cpu stats.
check_cpu: true

    # How often the stats will refresh.
stats_refresh_time: 5

    # How low the tps has to drop before the low-tps logger will begin triggering.
low_tps: 30

    # Whether or not the NetData Integration is enabled.
net_data_integration_enabled: true

    # Whether or not the plugin will log the stats and LowTps to the file.
file_logging_enabled: true

    # The location of the log files if File Logging is enabled.
file_logging_location: /home/container/.config/SCP Secret Laboratory/PluginAPI/plugins/global/PWProfiler/

    # How many frames will be sampled for the average tps.
average_frame_sample_amount: 120

    # The name of the server to show up in netdata.
server_name: Test Net
```

<br/>

## Example Stats File:

```
# Local Time          |  Epoch Time  |  Average TPS  |  Average Delta Time  |  Memory Usage  |  Cpu Usage  |  Players
31/01/2023 23:22:03   |  1675207323  |  60.50        |  0.0165290           |  1087          |  000.0000   |  0      
31/01/2023 23:22:08   |  1675207328  |  60.50        |  0.0165295           |  1087          |  017.8214   |  0      
31/01/2023 23:22:13   |  1675207333  |  60.50        |  0.0165279           |  1087          |  017.2263   |  0      
```


## Example LowFps File:
```
# Local Time          |  Epoch Time  |  Instance #  |  TPS       |  Delta Time  |  Players
31/01/2023 23:33:59   |  1675208039  |  0           | 7.54       |  0.1326389   |  1      
31/01/2023 23:34:06   |  1675208046  |  1           | 5.72       |  0.1747492   |  1      
31/01/2023 23:34:12   |  1675208052  |  1           | 4.61       |  0.2169288   |  1      
# Refresh, server caught back up. 
31/01/2023 23:34:18   |  1675208058  |  0           | 3.86       |  0.2591510   |  1      
# Refresh, server caught back up. 
```

<br/>

## Contributing

Contributions are always welcome!

Reach out to Redforce04#4091 on discord for ways to get started.

Please adhere to this project's `code of conduct`.


<br/>

## License

[MIT](https://choosealicense.com/licenses/mit/)

